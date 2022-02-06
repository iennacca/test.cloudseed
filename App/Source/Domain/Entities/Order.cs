using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp {

    /*
        * An order represents a completed Checkout
    */
    [Table("orders")]
    public class Order {
        [Column("id")]
        [Key]
        public Guid ID { get; }

        [Column("userid")]
        public Guid UserId { get; }

        [Column("order_timestamp_utc")]
        public DateTimeOffset OrderTimestamp { get; set; }

        [Column("data", TypeName = "jsonb")]
        public OrderData Data { get; }

        public Order(
            Guid userId, // Note: pure camelCase for EF Core
            DateTimeOffset orderTimestamp,
            OrderData data
        ) {
            this.UserId = userId;
            this.OrderTimestamp = orderTimestamp;
            this.Data = data;
        }

        /*
            * For EntityFramework
        */
        private Order() {}

        public class OrderData {
            [JsonProperty("products")]
            public List<OrderProductItem> Products { get; set; }

            [JsonProperty("stripe_invoice_id")]
            [JsonConverter(typeof(NullableTypeJsonConverter))]
            public string? StripeInvoiceId { get; set; }
        }
    }
}