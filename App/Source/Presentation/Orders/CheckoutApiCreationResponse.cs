using System.Text.Json.Serialization;

namespace CloudSeedApp {

    public class CheckoutApiCreationResponse {
        [JsonPropertyName("checkoutUrl")]
        public string CheckoutUrl { get; set; }
    }
}