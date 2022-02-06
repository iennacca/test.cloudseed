using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CloudSeedApp {

    public class CheckoutApiMetadata {

        [JsonPropertyName("products")]
        public List<CheckoutApiProductItem> Products { get; set; }
    }
}