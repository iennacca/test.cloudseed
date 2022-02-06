using System;
using System.Collections.Generic;

using CloudSeedApp;

namespace CloudSeedApp {

    public static class ProductConfiguration {
        /*
            * CHANGEME: Configure your products here
        */

        public static class ProductIDs {
            public const string TestSubscriptionOne = "REAL_SUBSCRIPTION_ONE";
        }

        public static List<Product> GetProductionProducts() {
            return new List<Product> {
                new Product(
                    ProductIDs.TestSubscriptionOne,
                    Product.ProductType.Subscription,
                    new Product.ProductData {
                        StripeProductID = "CHANGEME_stripeProductId"
                    },
                    new Product.SubscriptionProductData {
                        SubscriptionPeriod = Product.SubscriptionPeriods.Year
                    }
                ),
            };
        }

        public static class TestProductIDs {
            public const string TestSubscriptionOne = "TEST_SUBSCRIPTION_ONE";
        }

        public static List<Product> GetTestProducts() {
            return new List<Product> {
                new Product(
                    TestProductIDs.TestSubscriptionOne,
                    Product.ProductType.Subscription,
                    new Product.ProductData {
                        StripeProductID = "CHANGEME_stripeTestProductId"
                    },
                    new Product.SubscriptionProductData {
                        SubscriptionPeriod = Product.SubscriptionPeriods.Year
                    }
                ),
            };
        }
    }
}