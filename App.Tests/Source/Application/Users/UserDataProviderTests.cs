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
    public class UserDataProviderTests : DatabaseTest
    {
        public UserDataProviderTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicUserRetrieval() {
            var dataProvider = new UserDataProvider(
                this.DbContext,
                TestDataPopulationUtilities.GetMemoryCache<User>()
            );

            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);

            var actualUser = await dataProvider.TryGetUserAsync(
                user.Id
            );
            Assert.Equal(true, actualUser is not null);
            Assert.Equal(user.Id, actualUser.Id);
        }

        [Fact]
        public async Task TestBasicUserRetrievalByEmail() {
            var dataProvider = new UserDataProvider(
                this.DbContext,
                TestDataPopulationUtilities.GetMemoryCache<User>()
            );

            var user = await TestDataPopulationUtilities.CreateUserAsync(this.DbContext);

            var actualUser = await dataProvider.TryGetUserByEmailAsync(
                user.Email
            );
            Assert.Equal(true, actualUser is not null);
            Assert.Equal(user.Id, actualUser.Id);
        }

        [Fact]
        public async Task TestBasicUserRetrievalByStripeCustomerId() {
            var dataProvider = new UserDataProvider(
                this.DbContext,
                TestDataPopulationUtilities.GetMemoryCache<User>()
            );

            var user = await TestDataPopulationUtilities
                .CreateUserAsync(this.DbContext);

            var stripeCustomerId = Guid.NewGuid().ToString();
            user.Data.StripeCustomerId = stripeCustomerId;
            this.DbContext.MarkDataAsModified(user);
            await this.DbContext.SaveChangesAsync();

            var actualUser = await dataProvider.TryGetUserByStripeCustomerIdAsync(
                stripeCustomerId
            );
            Assert.Equal(true, actualUser is not null);
            Assert.Equal(user.Id, actualUser.Id);
        }
    }
}
