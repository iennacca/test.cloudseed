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
    public class OrderTests : DatabaseTest
    {
        public OrderTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicOrderCreation() {
            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);
            await this.DbContext.SaveChangesAsync();
            var nowProvider = new UtcNowProvider();

            Assert.NotNull(user.Id);
            var order = new Order(
                user.Id,
                nowProvider.GetNowDateTimeOffset(),
                new Order.OrderData()
            );
            await this.DbContext 
                .Orders 
                .AddAsync(order);
            await this.DbContext.SaveChangesAsync();

            var savedOrder = await this.DbContext
                .Orders 
                .SingleAsync(c => c.ID == order.ID);

            Assert.Equal(savedOrder.UserId, user.Id);
        }

        private async Task<Checkout> CreateCheckoutAsync() {
            var nowProvider = new UtcNowProvider();
            var nowDateTimeOffset = nowProvider.GetNowDateTimeOffset();
            var twoDaysFromNowDateTimeOffset = nowDateTimeOffset.AddDays(2);
            
            var checkout = new Checkout(
                nowDateTimeOffset,
                twoDaysFromNowDateTimeOffset,
                null
            );
            await this.DbContext
                .Checkouts 
                .AddAsync(checkout);
            await this.DbContext.SaveChangesAsync();

            return checkout;
        }
    }
}
