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
    public class UserTests : DatabaseTest
    {
        public UserTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestBasicUserCreation() {
            var guid = Guid.NewGuid();
            var email = $"{guid.ToString()}@cloudseed.xyz";
            var user = new User(email, null);
            this.DbContext
                .Users
                .Add(user);
            await this.DbContext.SaveChangesAsync();
            
            var savedUser = await this.DbContext
                .Users
                .SingleAsync(u => u.Email == email);
            
            Assert.Equal(savedUser.Email, email);
            Assert.NotEqual(savedUser.Id.ToString(), string.Empty);
        }

        [Fact]
        public async Task TestUserCreationWithGitUsernames() {
            var guid = Guid.NewGuid();
            var email = $"{guid.ToString()}@cloudseed.xyz";
            var userData = new User.UserData {
                    GitLabUsername = Guid.NewGuid().ToString(),
                    GitHubUsername = Guid.NewGuid().ToString()
                };
            var user = new User(email, userData);
            this.DbContext
                .Users
                .Add(user);
            await this.DbContext.SaveChangesAsync();
            
            var savedUser = await this.DbContext
                .Users
                .SingleAsync(u => u.Id == user.Id);
            
            Assert.Equal(savedUser.Data.GitHubUsername, userData.GitHubUsername);
            Assert.Equal(savedUser.Data.GitLabUsername, userData.GitLabUsername);
        }
    }
}
