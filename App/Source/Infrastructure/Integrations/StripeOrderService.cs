using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace CloudSeedApp;

public class StripeOrderService : IOrderService {
    private readonly string ACCOUNT_PAGE_URL;

    private readonly CloudSeedAppDatabaseContext _context;
    private readonly ConfigurationProvider _configuration;
    private readonly IMediator _mediator;
    private readonly ProductDataProvider _productDataProvider;

    public StripeOrderService(
        CloudSeedAppDatabaseContext context,
        ConfigurationProvider configuration,
        IMediator mediator,
        ProductDataProvider productProvider
    ) {
        this._context = context;
        this._configuration = configuration;
        this._mediator = mediator;
        this._productDataProvider = productProvider;
        StripeConfiguration.ApiKey = this._configuration.STRIPE_API_KEY;
        ACCOUNT_PAGE_URL = $"{this._configuration.WEB_BASE_URL}/account";
    }

    public async Task<string> GetCheckoutPortalUrlForCheckoutAsync(Guid checkoutId) {
        var checkoutQuery = new GetCheckoutQuery { CheckoutId = checkoutId.ToString() };
        var checkout = await this._mediator.Send(checkoutQuery);
        if(checkout is null) {
            throw new InvalidOperationException(
                $"No checkout found for Id: {checkoutId}"
            );
        }

        var productIDToProductLookup = checkout.Data.Products
                .Select(p => {
                    var productExists = this._productDataProvider.TryGetProductByID(
                        p.ItemID,
                        out var product
                    );
                    if(!productExists) {
                        return null;
                    }
                    return product;
                }).Where(p => p is not null)
                .ToDictionary(
                    p => p.ID,
                    p => p
                );

            var sessionLineItems = checkout.Data.Products
                .Select(p => {
                    var product = productIDToProductLookup[p.ItemID];

                    if(product is null) {
                        throw new InvalidOperationException("Product is null!");
                    }

                    return new SessionLineItemOptions {
                        Price = product.Data.StripeProductID,
                        Quantity = p.Quantity
                    };
                }).ToList();

            var sessionOptions = new SessionCreateOptions 
            {
                SuccessUrl = ACCOUNT_PAGE_URL,
                CancelUrl = ACCOUNT_PAGE_URL,
                PaymentMethodTypes = new List<string> {
                    "card"
                },
                Mode = "subscription",
                LineItems = sessionLineItems,
            };

            // if(this._client is null) {
            //     throw new InvalidOperationException("SripeAPI client is null!");
            // }

            // var sessionService = new SessionService(this._client);
            var sessionService = new SessionService();

            if(sessionService is null) {
                throw new InvalidOperationException("StripeAPI sessionService is null!");
            }

            var checkoutSession = await sessionService.CreateAsync(sessionOptions);

            if(checkoutSession is null) {
                throw new InvalidOperationException(
                    $"Failed to create Checkout Portal for checkout: {checkoutId}"
                );
            }

            checkout.Data.StripeCheckoutID = checkoutSession.Id;
            this._context.MarkDataAsModified(checkout);
            await this._context.SaveChangesAsync();

            return checkoutSession.Url;
    }

    public async Task<string> GetOrderManagementPortalUrlForUserAsync(Guid userId) {
        var getUserQuery = new GetUserQuery { UserId = userId };
        var user = await this._mediator.Send(getUserQuery);

        if(user is null) {
            throw new InvalidOperationException(
                $"User does not exist: {userId}"
            );
        }

        var stripeCustomerId = user.Data.StripeCustomerId;
        if(stripeCustomerId is null) {
            throw new InvalidOperationException(
                $"No valid customer id found for user: {userId}"
            );
        }

        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = stripeCustomerId,
            ReturnUrl = ACCOUNT_PAGE_URL
        };
        var service = new Stripe.BillingPortal.SessionService();
        var session = service.Create(options);
        if(session is null) {
            throw new InvalidOperationException(
                $"Failed to create BillingPortal for user: {userId}"
            );
        }

        return session.Url;
    }
}