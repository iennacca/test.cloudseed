using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp {

    /*
        * An active subscription for a given user and product
    */
    [Table("subscriptions")]
    public class Subscription {

        [Column("user_id")]
        public Guid UserID { get; set; }

        [Column("product_id")]
        public string ProductID { get; set; }

        [Column("start_timestamp_utc")]
        public DateTimeOffset StartTimestamp { get; set; }

        [Column("expiration_timestamp_utc")]
        public DateTimeOffset ExpirationTimestamp { get; set; }

        [Column("data", TypeName = "jsonb")]
        public SubscriptionData Data { get; set; }

        public Subscription(
            Guid userId, // Note: pure camelCase for EF Core
            string productId, // Note: pure camelCase for EF Core
            DateTimeOffset startTimestamp,
            DateTimeOffset expirationTimestamp,
            SubscriptionData data
        ) {
            this.UserID = userId;
            this.ProductID = productId;
            this.StartTimestamp = startTimestamp;
            this.ExpirationTimestamp = expirationTimestamp;
            this.Data = data;
        }

        /*
            * For EntityFramework
        */
        private Subscription() {}

        public class SubscriptionData {
        }
    }
}