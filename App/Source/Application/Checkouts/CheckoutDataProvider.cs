using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp {

    public class CheckoutDataProvider {

        private CloudSeedAppDatabaseContext _dbContext;
        private TSimpleMemoryCache<Checkout> _cache;

        public CheckoutDataProvider(
            CloudSeedAppDatabaseContext dbContext,
            TSimpleMemoryCache<Checkout> memoryCache) {
            this._dbContext = dbContext;
            this._cache = memoryCache;
        }

        public async Task<Checkout?> TryGetCheckoutAsync(
            Guid id
        ) {
            if(!this._cache.TryGetValue(
                GetCacheKey(id),
                out var checkoutMaybe
            )) {
                checkoutMaybe = await this.TryGetCheckoutInternalAsync(id);
                if(checkoutMaybe is not null) {
                    this._cache.Set(GetCacheKey(id), checkoutMaybe);
                }
            }
            return checkoutMaybe;
        }

        public async Task<Checkout?> TryGetCheckoutByStripeIDAsync(
            string stripeCheckoutID
        ) {
            if(!this._cache.TryGetValue(
                GetCacheKeyForStripeID(stripeCheckoutID),
                out var checkoutMaybe
            )) {
                checkoutMaybe = await this.TryGetCheckoutByStripeIDInternalAsync(
                    stripeCheckoutID);
                if(checkoutMaybe is not null) {
                    this._cache.Set(
                        GetCacheKeyForStripeID(stripeCheckoutID),
                        checkoutMaybe);
                }
            }
            return checkoutMaybe;
        }

        private async Task<Checkout?> TryGetCheckoutInternalAsync(Guid id) {
            return await this._dbContext
                .Checkouts
                .SingleOrDefaultAsync(c => c.ID == id);
        }

        private async Task<Checkout?> TryGetCheckoutByStripeIDInternalAsync(string stripeCheckoutID) {
            return await this._dbContext
                .Checkouts 
                .SingleOrDefaultAsync(c => c.Data.StripeCheckoutID == stripeCheckoutID);
        }

        private static string GetCacheKey(Guid id) {
            return id.ToString();
        }

        private static string GetCacheKeyForStripeID(string stripeCheckoutID) {
            return $"stripe_id_{stripeCheckoutID}";
        }
    }
}