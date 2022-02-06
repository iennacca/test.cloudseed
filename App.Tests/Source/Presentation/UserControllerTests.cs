using System;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using Xunit;
using Xunit.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using CloudSeedApp;


namespace AppTests
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public class UserControllerTests : DatabaseTest
    {
        public UserControllerTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicUserRegistration() {
            var userController = this.CreateUserController();

            var emailAddress = TestDataPopulationUtilities.CreateNewEmail();
            await userController.RegisterUserAsync(
                new UserController.RegisterUserPayload {
                    EmailAddress = emailAddress
                }
            );

            var user = await this.DbContext
                .Users
                .SingleAsync(u => u.Email == emailAddress);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("bademail", true)]
        [InlineData("noatsign.tld", true)]
        [InlineData("notld@somedomain", true)]
        // We omit a static working example so as not to run into db collisions
        public async Task TestRegistrationFailsForInvalidEmailAddress(
            string emailAddress,
            bool shouldThrow
        ) {
            var userController = this.CreateUserController();

            if(shouldThrow) {
                Assert.ThrowsAsync<InvalidOperationException>(
                    () => userController.RegisterUserAsync(
                        new UserController.RegisterUserPayload {
                            EmailAddress = emailAddress
                        }
                    )
                );
            } else {
                await userController.RegisterUserAsync(
                    new UserController.RegisterUserPayload {
                        EmailAddress = emailAddress
                    }
                );
            }
            
        }

        // [Fact]
        // public async Task TestRegistrationFailsEmptyEmailAddress() {
        //     var userController = this.CreateUserController();

        //     // var emailAddress = null;
        //     Assert.ThrowsAsync<InvalidOperationException>(
        //         () => userController.RegisterUserAsync(
        //             new UserController.RegisterUserPayload {
        //                 EmailAddress = string.Empty
        //             }
        //         )
        //     );
        // }

        private UserController CreateUserController() {
            var dataProvider = new UserDataProvider(
                this.DbContext,
                TestDataPopulationUtilities.GetMemoryCache<User>()
            );

            var userController = new UserController(
                this.DbContext,
                new Mock<ICurrentUserService>().Object,
                new Mock<ILogger<UserController>>().Object,
                new Mock<IMediator>().Object,
                dataProvider
            );
            return userController;
        }
    }
}
