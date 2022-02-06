using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using Microsoft.EntityFrameworkCore;

using CloudSeedApp;


namespace AppTests
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public class CheckoutProcessorTests : DatabaseTest
    {
        public CheckoutProcessorTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicCheckoutProcessorCreation() {
            var checkoutProcessor = new CheckoutProcessor(
                this.DbContext,
                new UtcNowProvider()
            );

            var expectedProduct = new OrderProductItem(
                "iamanitemid",
                123
            );
            var products = new List<OrderProductItem> {
                expectedProduct
            };
            var checkoutData = new Checkout.CheckoutData {
                Products = products
            };
            var newCheckout = await checkoutProcessor
                .CreateCheckoutAsync(checkoutData);

            var actualCheckout = await this.DbContext
                .Checkouts 
                .SingleAsync(c => c.ID == newCheckout.ID);

            Assert.Equal(actualCheckout.Data.Products.Count, 1);
        }
    }
}
