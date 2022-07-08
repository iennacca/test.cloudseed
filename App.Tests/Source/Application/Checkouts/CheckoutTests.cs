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
    public class CheckoutTests : DatabaseTest
    {
        public CheckoutTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicCheckoutCreation() {
            var guid = Guid.NewGuid();

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

            var savedCheckout = await this.DbContext
                .Checkouts 
                .SingleAsync(c => c.ID == checkout.ID);

            Assert.Equal(savedCheckout.CreationTimestamp, nowDateTimeOffset);
            Assert.Equal(savedCheckout.ExpirationTimestamp, twoDaysFromNowDateTimeOffset);
        }

        [Fact]
        public async Task TestBasicCheckoutUpdate() {
            var guid = Guid.NewGuid();

            var nowProvider = new UtcNowProvider();
            var nowDateTimeOffset = nowProvider.GetNowDateTimeOffset();
            var twoDaysFromNowDateTimeOffset = nowDateTimeOffset.AddDays(2);
            
            var checkout = new Checkout(
                nowDateTimeOffset,
                twoDaysFromNowDateTimeOffset,
                new Checkout.CheckoutData {
                    StripeCheckoutID = guid.ToString()
                }
            );
            await this.DbContext
                .Checkouts 
                .AddAsync(checkout);
            await this.DbContext.SaveChangesAsync();

            var newGuid = Guid.NewGuid();
            checkout.Data.StripeCheckoutID = newGuid.ToString();
            await this.DbContext.SaveChangesAsync();

            var savedCheckout = await this.DbContext
                .Checkouts 
                .SingleAsync(c => c.ID == checkout.ID);

            Assert.Equal(
                newGuid.ToString(),
                savedCheckout.Data.StripeCheckoutID
            );
        }

        [Fact]
        public async Task TestCheckoutCreationWithStripeID() {
            var expectedStripeCheckoutID = Guid.NewGuid().ToString();

            var nowProvider = new UtcNowProvider();
            var nowDateTimeOffset = nowProvider.GetNowDateTimeOffset();
            var twoDaysFromNowDateTimeOffset = nowDateTimeOffset.AddDays(2);
            
            var checkoutData = new Checkout.CheckoutData {
                StripeCheckoutID = expectedStripeCheckoutID
            };
            var checkout = new Checkout(
                nowDateTimeOffset,
                twoDaysFromNowDateTimeOffset,
                checkoutData
            );
            await this.DbContext
                .Checkouts 
                .AddAsync(checkout);
            await this.DbContext.SaveChangesAsync();

            var savedCheckout = await this.DbContext
                .Checkouts 
                .SingleAsync(c => c.ID == checkout.ID);

            Assert.Equal(savedCheckout.Data.StripeCheckoutID, expectedStripeCheckoutID);
        }

        [Fact]
        public async Task TestCheckoutLookupWithStripeID() {
            var expectedStripeCheckoutID = Guid.NewGuid().ToString();

            var nowProvider = new UtcNowProvider();
            var nowDateTimeOffset = nowProvider.GetNowDateTimeOffset();
            var twoDaysFromNowDateTimeOffset = nowDateTimeOffset.AddDays(2);
            
            var checkoutData = new Checkout.CheckoutData {
                StripeCheckoutID = expectedStripeCheckoutID
            };
            var checkout = new Checkout(
                nowDateTimeOffset,
                twoDaysFromNowDateTimeOffset,
                checkoutData
            );
            await this.DbContext
                .Checkouts 
                .AddAsync(checkout);
            await this.DbContext.SaveChangesAsync();

            var savedCheckout = await this.DbContext
                .Checkouts 
                .SingleAsync(c => c.Data.StripeCheckoutID == expectedStripeCheckoutID);

            Assert.Equal(checkout.ID, savedCheckout.ID);
        }

        [Fact]
        public async Task TestCheckoutLookupWithStripeIDWithTestDataPopulation() {
            var expectedStripeCheckoutID = Guid.NewGuid().ToString();

            var nowProvider = new UtcNowProvider();
            var nowDateTimeOffset = nowProvider.GetNowDateTimeOffset();
            var twoDaysFromNowDateTimeOffset = nowDateTimeOffset.AddDays(2);
            
            var checkout = await TestDataPopulationUtilities
                .CreateCheckoutAsync(this.DbContext);

            var connectedCheckout = await this.DbContext
                .Checkouts
                .SingleAsync(c => c.ID == checkout.ID);

            connectedCheckout.Data.StripeCheckoutID = expectedStripeCheckoutID;
            this.DbContext.Entry(connectedCheckout)
                .Property(e => e.Data).IsModified = true;
            await this.DbContext.SaveChangesAsync();

            var savedCheckout = await this.DbContext
                .Checkouts 
                .SingleAsync(c => c.Data.StripeCheckoutID == expectedStripeCheckoutID);

            Assert.Equal(checkout.ID, savedCheckout.ID);
        }
    }
}
