using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using Microsoft.EntityFrameworkCore;

using CloudSeedApp;


namespace AppTests
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public class SubscriptionTests : DatabaseTest
    {
        public SubscriptionTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicSubscriptionCreation() {
            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);
            var nowProvider = new UtcNowProvider();

            var subscribedProductID = ProductConfiguration.TestProductIDs.TestSubscriptionOne;
            var subscription = new Subscription(
                user.Id,
                subscribedProductID,
                nowProvider.GetNowDateTimeOffset(),
                nowProvider.GetNowDateTimeOffset().AddDays(1),
                new Subscription.SubscriptionData()
            );
            await this.DbContext
                .Subscriptions
                .AddAsync(subscription);
            await this.DbContext.SaveChangesAsync();

            var savedSubscription = await this.DbContext
                .Subscriptions 
                .SingleAsync(s => s.ProductID == subscribedProductID && s.UserID == user.Id);
        }

        #region CreateOrUpdateSubscription
        [Fact]
        public async Task TestCreateOrUpdateSubscriptionCreatesWhenNoSubscriptionPresent() {
            var subscriptionDataProvider = GetNewSubscriptionDataProvider(this.DbContext);
            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);

            var product = this.GetNewTestSubscriptionProduct(
                Product.SubscriptionPeriods.Year
            );
            await subscriptionDataProvider
                .CreateOrUpdateSubscriptionAsync(
                    user.Id,
                    product
                );

            var subscription = await this.DbContext
                .Subscriptions
                .SingleAsync(s => s.UserID == user.Id && s.ProductID == product.ID);
        }

        [Fact]
        public async Task TestCreateOrUpdateSubscriptionUpdatesExpirationDateWhenSubscriptionPresent() {
            var subscriptionDataProvider = GetNewSubscriptionDataProvider(this.DbContext);
            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);

            var product = this.GetNewTestSubscriptionProduct(
                Product.SubscriptionPeriods.Year
            );
            await subscriptionDataProvider
                .CreateOrUpdateSubscriptionAsync(
                    user.Id,
                    product
                );

            var initialSubscription = await this.DbContext
                .Subscriptions
                .SingleAsync(s => s.UserID == user.Id && s.ProductID == product.ID);
            
            await subscriptionDataProvider
                .CreateOrUpdateSubscriptionAsync(
                    user.Id,
                    product
                );

            var latestSubscription = await this.DbContext
                .Subscriptions
                .SingleAsync(s => s.UserID == user.Id && s.ProductID == product.ID);
        }

        [Fact]
        public async Task TestCreateOrUpdateSubscriptionThrowsOnNonSubscriptionProduct() {
            var subscriptionDataProvider = GetNewSubscriptionDataProvider(this.DbContext);
            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);

            Assert.ThrowsAsync<InvalidOperationException>(
                () => subscriptionDataProvider
                    .CreateOrUpdateSubscriptionAsync(
                        user.Id,
                        this.GetNewTestSubscriptionProduct(
                            Product.SubscriptionPeriods.Undefined_DO_NOT_USE
                        )
                    )
            );
        }

        [Fact]
        public async Task TestCreateOrUpdateSubscriptionThrowsWhenNoUser() {
            var subscriptionDataProvider = GetNewSubscriptionDataProvider(this.DbContext);

            Assert.ThrowsAsync<InvalidOperationException>(
                () => subscriptionDataProvider
                    .CreateOrUpdateSubscriptionAsync(
                        Guid.NewGuid(),
                        this.GetNewTestSubscriptionProduct(
                            Product.SubscriptionPeriods.Year
                        )
                    )
            );
        }
        #endregion CreateOrUpdateSubscription

        private Product GetNewTestSubscriptionProduct(Product.SubscriptionPeriods period) {
            return new Product(
                Guid.NewGuid().ToString(),
                Product.ProductType.Subscription,
                new Product.ProductData(),
                new Product.SubscriptionProductData {
                    SubscriptionPeriod = period
                }
            );
        }

        private static SubscriptionDataProvider GetNewSubscriptionDataProvider(
            CloudSeedAppDatabaseContext dbContext
        ) {
            return new SubscriptionDataProvider(
                dbContext,
                new UtcNowProvider(),
                TestDataPopulationUtilities.CreateUserDataProvider(dbContext)
            );
        }
    }
}
