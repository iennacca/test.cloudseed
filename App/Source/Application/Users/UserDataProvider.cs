using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp {

    public class UserDataProvider {

        private CloudSeedAppDatabaseContext _dbContext;
        private TSimpleMemoryCache<User> _cache;
        private Dictionary<string, Guid> _emailToUserIDLookup;
        private Dictionary<string, Guid> _stripeCustomerIdToUserIdLookup;

        public UserDataProvider(
            CloudSeedAppDatabaseContext dbContext,
            TSimpleMemoryCache<User> memoryCache) {
            this._dbContext = dbContext;
            this._cache = memoryCache;
            this._emailToUserIDLookup = new Dictionary<string, Guid>();
            this._stripeCustomerIdToUserIdLookup = new Dictionary<string, Guid>();
        }

        public async Task<User?> TryGetUserAsync(
            Guid id
        ) {
            if(!this._cache.TryGetValue(
                GetCacheKey(id),
                out var userMaybe
            )) {
                userMaybe = await this.TryGetUserInternalAsync(id);
                if(userMaybe is not null) {
                    this._cache.Set(GetCacheKey(id), userMaybe);
                }
            }
            return userMaybe;
        }

        public async Task<User?> TryGetUserByEmailAsync(
            string email
        ) {
            if(this._emailToUserIDLookup.ContainsKey(email)) {
                return await this.TryGetUserAsync(
                    this._emailToUserIDLookup[email]
                );
            }

            var userMaybe = await this._dbContext
                .Users
                .SingleOrDefaultAsync(u => u.Email == email);
            if(userMaybe is null) {
                return null;
            }

            this._emailToUserIDLookup[email] = userMaybe.Id;
            this._cache.Set(GetCacheKey(userMaybe.Id), userMaybe);
            return userMaybe;
        }

        public async Task<User?> TryGetUserByStripeCustomerIdAsync(
            string stripeCustomerId
        ) {
            if(this._stripeCustomerIdToUserIdLookup.ContainsKey(stripeCustomerId)) {
                return await this.TryGetUserAsync(
                    this._stripeCustomerIdToUserIdLookup[stripeCustomerId]
                );
            }

            var userMaybe = await this._dbContext
                .Users 
                .SingleOrDefaultAsync(u => u.Data.StripeCustomerId == stripeCustomerId);
            if(userMaybe is null) {
                return null;
            }

            this._stripeCustomerIdToUserIdLookup[stripeCustomerId] = userMaybe.Id;
            this._cache.Set(GetCacheKey(userMaybe.Id), userMaybe);
            return userMaybe;
        }

        public void Remove(Guid id) {
            this._cache.Remove(GetCacheKey(id));
        }

        private async Task<User?> TryGetUserInternalAsync(Guid id) {
            return await this._dbContext
                .Users
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        private static string GetCacheKey(Guid id) {
            return id.ToString();
        }
    }
}