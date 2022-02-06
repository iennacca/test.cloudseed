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
    public class CheckoutDataProviderTests : DatabaseTest
    {
        public CheckoutDataProviderTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicCheckoutRetrieval() {
            var dataProvider = new CheckoutDataProvider(
                this.DbContext,
                TestDataPopulationUtilities.GetMemoryCache<Checkout>()
            );

            var checkout = await TestDataPopulationUtilities.CreateCheckoutAsync(this.DbContext);

            var actualCheckout = await dataProvider.TryGetCheckoutAsync(
                checkout.ID
            );
            Assert.Equal(true, actualCheckout is not null);
            Assert.Equal(checkout.ID, actualCheckout.ID);
        }

        [Fact]
        public async Task TestCheckoutRetrievalByStripeID() {
            var dataProvider = new CheckoutDataProvider(
                this.DbContext,
                TestDataPopulationUtilities.GetMemoryCache<Checkout>()
            );

            var checkout = await TestDataPopulationUtilities.CreateCheckoutAsync(this.DbContext);

            var connectedCheckout = await this.DbContext
                .Checkouts
                .SingleAsync(c => c.ID == checkout.ID);
            var stripeID = Guid.NewGuid().ToString();
            connectedCheckout.Data.StripeCheckoutID = stripeID;
            this.DbContext.MarkDataAsModified<Checkout>(connectedCheckout);
            await this.DbContext.SaveChangesAsync();

            // Sanity check that it's in the db context
            // await this.DbContext
            //     .Checkouts
            //     .SingleAsync(c => c.Data.StripeCheckoutID == stripeID);
            var savedCheckout = await this.DbContext
                .Checkouts
                .SingleAsync(c => c.ID == connectedCheckout.ID);
            Assert.Equal(stripeID, savedCheckout.Data.StripeCheckoutID);

            var actualCheckout = await dataProvider.TryGetCheckoutByStripeIDAsync(
                stripeID
            );
            Assert.Equal(true, actualCheckout is not null);
            Assert.Equal(checkout.ID, actualCheckout.ID);
        }
    }
}
