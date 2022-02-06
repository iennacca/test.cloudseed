using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp {

    /*
        * Represents a Checkout session in our db
    */
    [Table("checkouts")]
    public class Checkout {
        [Column("id")]
        [Key]
        public Guid ID { get; }

        // public set for EFCore compatibility
        [Column("creation_timestamp_utc")]
        public DateTimeOffset CreationTimestamp { get; set; }

        // public set for EFCore compatibility
        [Column("expiration_timestamp_utc")]
        public DateTimeOffset ExpirationTimestamp { get; set; }

        [Column("data", TypeName = "jsonb")]
        public CheckoutData Data { get; private set; }

        // public Order Order { get; set; }

        public Checkout(
            DateTimeOffset creationTimestamp,
            DateTimeOffset expirationTimestamp,
            CheckoutData data
        ) {
            this.CreationTimestamp = creationTimestamp;
            this.ExpirationTimestamp = expirationTimestamp;
            this.Data = data;
        }

        // For EF Core
        private Checkout() {}

        public class CheckoutData {
            [JsonProperty("products")]
            public List<OrderProductItem> Products { get; set; }

            [JsonProperty("stripe_checkout_id")]
            public string StripeCheckoutID { get; set; }

            [JsonProperty("user_id")]
            [JsonConverter(typeof(NullableTypeJsonConverter))]
            public Guid UserID { get; set; }
        } 
    }
}