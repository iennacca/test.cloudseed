using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace CloudSeedApp {

    public class CheckoutProcessor {

        private CloudSeedAppDatabaseContext _dbContext;
        private INowProvider _nowProvider;

        public CheckoutProcessor(
            CloudSeedAppDatabaseContext dbContext,
            INowProvider nowProvider
        ) {
            this._dbContext = dbContext;
            this._nowProvider = nowProvider;
        }

        public async Task<Checkout> CreateCheckoutAsync(
            Checkout.CheckoutData checkoutData,
            TimeSpan? expirationDuration = null
        ) {
            var creationTime = this._nowProvider.GetNowDateTimeOffset();
            
            var effectiveExpirationDuration = expirationDuration ?? TimeSpan.FromDays(7);
            if(effectiveExpirationDuration.Ticks < 0) {
                throw new InvalidOperationException($"Expiration duration must be positive! Received {effectiveExpirationDuration.Ticks}");
            }
            
            var newCheckout = new Checkout(
                creationTime,
                creationTime.Add(effectiveExpirationDuration),
                checkoutData
            );
            await this._dbContext
                .Checkouts
                .AddAsync(newCheckout);
            await this._dbContext.SaveChangesAsync();

            return newCheckout;
        }

    }
}