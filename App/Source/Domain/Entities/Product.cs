using System;

namespace CloudSeedApp {
    public class Product {
        public string ID { get; }

        public ProductType Type { get; }

        public ProductData Data { get; }

        public SubscriptionProductData? SubscriptionData { get; }

        public Product(
            string id,
            ProductType type,
            ProductData? data = null,
            SubscriptionProductData? subscriptionData = null
        ) {
            if(type == ProductType.Subscription && subscriptionData is null) {
                throw new InvalidOperationException(
                    "Subscription data must be passed in for a subscription product!"
                );
            }

            this.ID = id;
            this.Type = type;
            this.Data = data ?? new ProductData();
            this.SubscriptionData = subscriptionData;
        }

        public class ProductData {
            // Note - this is a misnomer. It's actually the StripePriceID
            public string? StripeProductID { get; set; }
        }

        public class SubscriptionProductData {
            public SubscriptionPeriods SubscriptionPeriod { get; set; }
        }

        public enum ProductType {
            Undefined_DO_NOT_USE = 0, // do not use
            SingleOrder = 1,
            Subscription = 2
        }

        public enum SubscriptionPeriods {
            Undefined_DO_NOT_USE = 0,
            Year = 1,
            Month = 2
        }

        public static DateTimeOffset ExtendDateTimeOffsetForSubscriptionPeriod(
            DateTimeOffset toExtend,
            SubscriptionPeriods period) {
            switch(period) {
                case SubscriptionPeriods.Year:
                    return toExtend.AddYears(1);
                case SubscriptionPeriods.Month:
                    return toExtend.AddMonths(1);
                default:
                    throw new InvalidOperationException("Unknown Subscription Period provided!");
            }
        }
    }
}