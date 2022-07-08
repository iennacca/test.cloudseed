using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CloudSeedApp {

    public class StripeCheckoutProcessor {
        private CheckoutDataProvider _checkoutDataProvider;
        private CheckoutProcessor _checkoutProcessor;
        private CloudSeedAppDatabaseContext _dbContext;
        private ConfigurationProvider _configurationProvider;
        private INowProvider _nowProvider;
        private ProductDataProvider _productDataProvider;
        private SubscriptionDataProvider _subscriptionDataProvider;
        private UserDataProvider _userDataProvider;

        public StripeCheckoutProcessor(
            CheckoutDataProvider checkoutDataProvider,
            CheckoutProcessor checkoutProcessor,
            CloudSeedAppDatabaseContext dbContext,
            ConfigurationProvider configurationProvider,
            INowProvider nowProvider,
            ProductDataProvider productDataProvider,
            SubscriptionDataProvider subscriptionDataProvider,
            UserDataProvider userDataProvider
        ) {
            this._checkoutDataProvider = checkoutDataProvider;
            this._checkoutProcessor = checkoutProcessor;
            this._dbContext = dbContext;
            this._configurationProvider = configurationProvider;
            this._nowProvider = nowProvider;
            this._productDataProvider = productDataProvider;
            this._subscriptionDataProvider = subscriptionDataProvider;
            this._userDataProvider = userDataProvider;
        }


        /*
            * On checkoutSesison complete:
                * Mark checkout as complete
                * Set the user's stripeCustomerId if not set
        */
        public async Task HandleCheckoutSessionCompleted(
            string stripeCheckoutId,
            string stripeCustomerId
        ) {
            var checkoutMaybe = await this._checkoutDataProvider
                .TryGetCheckoutByStripeIDAsync(stripeCheckoutId);
            if(checkoutMaybe is null) {
                throw new InvalidOperationException($"Could not find checkout for Stripe Session ID {stripeCheckoutId}");
            }

            var user = await this._userDataProvider
                .TryGetUserAsync(
                    (Guid)checkoutMaybe.Data.UserID
                );
            if(user is null) {
                throw new InvalidOperationException($"No User corresponding to saved userid {checkoutMaybe.Data.UserID.ToString()} was found!");
            }

            if(user.Data.StripeCustomerId is null) {
                user.Data.StripeCustomerId = stripeCustomerId;
                this._dbContext.MarkDataAsModified(user);
                await this._dbContext.SaveChangesAsync();
                // explicitly remove from cache
                this._userDataProvider.Remove(user.Id);
            }

            checkoutMaybe.ExpirationTimestamp = this._nowProvider.GetNowDateTimeOffset().AddMinutes(-5);
            await this._dbContext.SaveChangesAsync();
        }

        public async Task HandleInvoicePaidAsync(
            string stripeCustomerId,
            string stripeInvoiceId,
            List<OrderProductItem> products
        ) {
            var userMaybe = await this._userDataProvider
                .TryGetUserByStripeCustomerIdAsync(stripeCustomerId);
            if(userMaybe is null) {
                throw new InvalidOperationException(
                    $"Could not find corresponding user for stripeCustomerId: {stripeCustomerId}"
                );
            }

            if(products.Any(p => !this._productDataProvider.TryGetProductByID(p.ItemID, out _))) {
                throw new InvalidOperationException(
                    "Unknown Product given!"
                );
            }

            var orderExists = (await this._dbContext
                .Orders 
                .Where(o => o.UserId == userMaybe.Id)
                .ToListAsync())
                .Any(o => o.Data.StripeInvoiceId == stripeInvoiceId);
            if(orderExists) {
                return;
            }

            // create order
            var order = new Order(
                userMaybe.Id,
                this._nowProvider.GetNowDateTimeOffset(),
                new Order.OrderData {
                    Products = products,
                    StripeInvoiceId = stripeInvoiceId
                }
            );
            await this._dbContext
                .Orders
                .AddAsync(order);
            await this._dbContext
                .SaveChangesAsync();

            var provisionSubscriptionAwaitables = products
                .Select(p => {
                    var _ = this._productDataProvider
                        .TryGetProductByID(p.ItemID, out var fullProduct);
                    return this._subscriptionDataProvider
                        .CreateOrUpdateSubscriptionAsync(
                            userMaybe.Id,
                            fullProduct
                        );
                }).ToList()
                .ToArray();
            Task.WaitAll(provisionSubscriptionAwaitables);
        }
    }
}