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
    public class ProductTests : DatabaseTest
    {
        public ProductTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicProductRetrieval() {
            var productDataProvider = new ProductDataProvider(
                ProductConfiguration.GetTestProducts()
            );

            var subscriptionOneSuccess = productDataProvider.TryGetProductByID(
                ProductConfiguration.TestProductIDs.TestSubscriptionOne,
                out var subscriptionOneProduct
            );
            Assert.True(subscriptionOneSuccess);
            Assert.NotNull(subscriptionOneProduct);
        }

    }
}
