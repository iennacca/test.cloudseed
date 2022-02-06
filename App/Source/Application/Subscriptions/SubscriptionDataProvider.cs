using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp
{

    public class SubscriptionDataProvider
    {

        CloudSeedAppDatabaseContext _dbContext;
        INowProvider _nowProvider;
        UserDataProvider _userDataProvider;

        public SubscriptionDataProvider(
            CloudSeedAppDatabaseContext dbContext,
            INowProvider nowProvider,
            UserDataProvider userDataProvider
        )
        {
            this._dbContext = dbContext;
            this._nowProvider = nowProvider;
            this._userDataProvider = userDataProvider;
        }

        public async Task<Subscription> CreateOrUpdateSubscriptionAsync(
            Guid userId,
            Product product
        )
        {
            if (product.Type != Product.ProductType.Subscription)
            {
                throw new InvalidOperationException("Product is not a subscription!");
            }

            var user = await this._userDataProvider
                .TryGetUserAsync(userId);
            if (user is null)
            {
                throw new InvalidOperationException($"No user found! userId: {userId}");
            }

            var subscription = await this._dbContext
                .Subscriptions
                .SingleOrDefaultAsync(s => s.UserID == userId && s.ProductID == product.ID);
            var now = this._nowProvider.GetNowDateTimeOffset();
            if (subscription is null)
            {
                subscription = new Subscription(
                    userId,
                    product.ID,
                    now,
                    now,
                    new Subscription.SubscriptionData()
                );
                this._dbContext
                    .Subscriptions
                    .Add(subscription);
            }

            var latestExpiration = subscription is null
                ? now
                : now > subscription.ExpirationTimestamp
                    ? now 
                    : subscription.ExpirationTimestamp;

            subscription!.ExpirationTimestamp = Product.ExtendDateTimeOffsetForSubscriptionPeriod(
                    latestExpiration,
                    product.SubscriptionData.SubscriptionPeriod);
            await this._dbContext.SaveChangesAsync();
            return subscription;
        }
    }
}