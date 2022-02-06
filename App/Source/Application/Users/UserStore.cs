using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp {
    public class UserStore : IUserStore<User> {

        private CloudSeedAppDatabaseContext _dbContext;
        private UserDataProvider _userDataProvider;

        public UserStore(
            CloudSeedAppDatabaseContext dbContext,
            UserDataProvider userDataProvider) {
            this._dbContext = dbContext;
            this._userDataProvider = userDataProvider;
        }

        public void Dispose()
        {
            // no-op to fulfill interface
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            this._dbContext
                .Users 
                .Add(user);
            await this._dbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            this._dbContext.MarkDataAsModified(user);
            await this._dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            this._dbContext
                .Users 
                .Remove(user);
            await this._dbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        #region userid
        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if(userId is null) {
                throw new ArgumentNullException(nameof(userId));
            }

            if(!Guid.TryParse(userId, out var guidId)) {
                throw new ArgumentException($"Provided userId is not a valid Guid: {guidId}");
            }

            return await this._userDataProvider
                .TryGetUserAsync(guidId);
        }

        public async Task<string> GetUserIdAsync(
            User user,
            CancellationToken cancellationToken = default(CancellationToken)
        ) {
            cancellationToken.ThrowIfCancellationRequested();
            if(user is null) {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Id.ToString();
        }
        #endregion

        #region username
        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
        #endregion


    }
}