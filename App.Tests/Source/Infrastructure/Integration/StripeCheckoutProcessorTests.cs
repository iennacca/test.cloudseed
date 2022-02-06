using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using Xunit;
using Xunit.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Stripe;

using CloudSeedApp;


namespace AppTests
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public class StripeCheckoutProcessorTests : DatabaseTest
    {
        public StripeCheckoutProcessorTests(DatabaseFixture fixture) : base(fixture) {

        }

        #region HandleCheckoutCompleted
        [Fact]
        public async Task TestHandleCheckoutSessionCompletedCreatesOrderForSession() {
            var stripeCheckoutProcessor = this.CreateProcessor();

            var stripeCheckoutId = Guid.NewGuid().ToString();
            var checkout = await this.CreateCheckoutWithStripeCheckoutIdAsync(stripeCheckoutId);

            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);
            checkout.Data.UserID = user.Id;
            this.DbContext.MarkDataAsModified(checkout);
            await this.DbContext.SaveChangesAsync();

            var stripeCustomerId = Guid.NewGuid().ToString();
            await stripeCheckoutProcessor.HandleCheckoutSessionCompleted(
                stripeCheckoutId,
                stripeCustomerId
            );

            Assert.Equal(stripeCustomerId, user.Data.StripeCustomerId);
            Assert.True(checkout.ExpirationTimestamp < (new UtcNowProvider()).GetNowDateTimeOffset());
        }

        [Fact]
        public async Task TestHandleCheckoutSessionCompletedThrowsOnNoMatchingCheckoutInDb() {
            var stripeCheckoutProcessor = this.CreateProcessor();

            var nonExistentStripeSessionId = Guid.NewGuid().ToString();
            Assert.ThrowsAsync<InvalidOperationException>(
                () => stripeCheckoutProcessor.HandleCheckoutSessionCompleted(
                    nonExistentStripeSessionId,
                    Guid.NewGuid().ToString()
                )
            );
        }

        [Fact]
        public async Task TestHandleCheckoutSessionCompletedThrowsOnNoMatchingUserInDb() {
            var stripeCheckoutProcessor = this.CreateProcessor();

            var stripeCheckoutId = Guid.NewGuid().ToString();
            var checkout = await this.CreateCheckoutWithStripeCheckoutIdAsync(stripeCheckoutId);
            Assert.Equal(stripeCheckoutId, checkout.Data.StripeCheckoutID);

            Assert.ThrowsAsync<InvalidOperationException>(
                () => stripeCheckoutProcessor.HandleCheckoutSessionCompleted(
                    stripeCheckoutId,
                    Guid.NewGuid().ToString()
                )
            );
        }
        #endregion HandleCheckoutCompleted

        #region HandleInvoicePaidAsync
        [Fact]
        public async Task TestHandleInvoicePaidSuccessfulProvisionsSubscriptionsAndCreatesOrders() {
            var stripeCheckoutProcessor = this.CreateProcessor();

            var stripeCustomerId = Guid.NewGuid().ToString();
            var user = await this.CreateUserWithStripeCustomerId(stripeCustomerId);
            await stripeCheckoutProcessor.HandleInvoicePaidAsync(
                stripeCustomerId,
                Guid.NewGuid().ToString(),
                new List<OrderProductItem> {
                    new OrderProductItem(
                        ProductConfiguration.GetTestProducts()[0].ID,
                        1
                    )
                }
            );

            await this.DbContext
                .Subscriptions
                .SingleAsync(s => s.UserID == user.Id);
            await this.DbContext
                .Orders
                .SingleAsync(o => o.UserId == user.Id);
        }

        [Fact]
        public async Task TestHandleInvoicePaidDedupesInvoices() {
            var stripeCheckoutProcessor = this.CreateProcessor();

            var stripeCustomerId = Guid.NewGuid().ToString();
            var user = await this.CreateUserWithStripeCustomerId(stripeCustomerId);
            var invoiceId = Guid.NewGuid().ToString();

            await stripeCheckoutProcessor.HandleInvoicePaidAsync(
                stripeCustomerId,
                invoiceId,
                new List<OrderProductItem> {
                    new OrderProductItem(
                        ProductConfiguration.GetTestProducts()[0].ID,
                        1
                    )
                }
            );

            // Run a second time
            await stripeCheckoutProcessor.HandleInvoicePaidAsync(
                stripeCustomerId,
                invoiceId,
                new List<OrderProductItem> {
                    new OrderProductItem(
                        ProductConfiguration.GetTestProducts()[0].ID,
                        1
                    )
                }
            );

            await this.DbContext
                .Orders
                .SingleAsync(o => o.UserId == user.Id);
        }

        [Fact]
        public async Task TestHandleInvoicePaidThrowsWhenNoUserFound() {
            var stripeCheckoutProcessor = this.CreateProcessor();

            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);
            var stripeCustomerId = Guid.NewGuid().ToString();

            Assert.ThrowsAsync<InvalidOperationException>(
                () => stripeCheckoutProcessor
                    .HandleInvoicePaidAsync(
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                        new List<OrderProductItem> {
                            new OrderProductItem(
                                ProductConfiguration.GetTestProducts()[0].ID,
                                1
                            )
                        }
                    )
            );
        }

        [Fact]
        public async Task TestHandleInvoicePaidCreatesOrder() {
            var stripeCheckoutProcessor = this.CreateProcessor();

            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);
            var stripeCustomerId = Guid.NewGuid().ToString();
            user.Data.StripeCustomerId = stripeCustomerId;
            this.DbContext.MarkDataAsModified(user);
            await this.DbContext.SaveChangesAsync();

            var orderProductItems = new List<OrderProductItem> {
                new OrderProductItem(
                    ProductConfiguration.GetTestProducts()[0].ID,
                    1
                )
            };

            await stripeCheckoutProcessor
                .HandleInvoicePaidAsync(
                    stripeCustomerId,
                    Guid.NewGuid().ToString(),
                    orderProductItems
                );

            var orderCount = await this.DbContext
                .Orders
                .CountAsync();
            Assert.True(orderCount > 0);

            var order = await this.DbContext
                .Orders
                .SingleAsync(o => o.UserId == user.Id);
            var products = order.Data.Products;
            Assert.Equal(orderProductItems, products);
        }
        #endregion HandleInvoicePaidAsync

        #region Utils
        private StripeCheckoutProcessor CreateProcessor() {
            return new StripeCheckoutProcessor(
                TestDataPopulationUtilities.CreateCheckoutDataProvider(this.DbContext),
                new CheckoutProcessor(
                    this.DbContext,
                    new UtcNowProvider()
                ),
                this.DbContext,
                TestDataPopulationUtilities.GetConfigurationProvider(),
                new UtcNowProvider(),
                TestDataPopulationUtilities.CreateProductDataProvider(),
                TestDataPopulationUtilities.CreateSubscriptionDataProvider(this.DbContext),
                TestDataPopulationUtilities.CreateUserDataProvider(this.DbContext)
            );
        }

        private async Task<Checkout> CreateCheckoutWithStripeCheckoutIdAsync(
            string stripeCheckoutId
        ) {
            var checkout = await TestDataPopulationUtilities.CreateCheckoutAsync(this.DbContext);
            checkout.Data.StripeCheckoutID = stripeCheckoutId;
            this.DbContext.MarkDataAsModified(checkout);
            await this.DbContext.SaveChangesAsync();
            return checkout;
        }

        private async Task<User> CreateUserWithStripeCustomerId(string stripeCustomerId) {
            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);
            user.Data.StripeCustomerId = stripeCustomerId;
            this.DbContext.MarkDataAsModified(user);
            await this.DbContext.SaveChangesAsync();
            return user;
        }
        #endregion
    }
}
