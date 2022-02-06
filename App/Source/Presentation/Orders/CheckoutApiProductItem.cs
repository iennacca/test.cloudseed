using System.Text.Json.Serialization;

namespace CloudSeedApp {

    public class CheckoutApiProductItem : IProductItem {

        [JsonPropertyName("itemId")]
        public string ItemID { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}