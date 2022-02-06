using System;
using System.Collections.Generic;

using CloudSeedApp;

namespace CloudSeedApp {

    public static class ProductConfiguration {

        public static class ProductIDs {
            public const string SingleProjectOneYear = "SINGLE_PROJECT_ONE_YEAR";
            public const string UnlimitedProjectOneYear = "UNLIMITED_PROJECT_ONE_YEAR";
        }

        public static List<Product> GetProductionProducts() {
            return new List<Product> {
                new Product(
                    ProductIDs.SingleProjectOneYear,
                    Product.ProductType.Subscription,
                    new Product.ProductData {
                        StripeProductID = "price_1K4I2iCDoQLx7JZGaL0rYLUB"
                    },
                    new Product.SubscriptionProductData {
                        SubscriptionPeriod = Product.SubscriptionPeriods.Year
                    }
                ),
                new Product(
                    ProductIDs.UnlimitedProjectOneYear,
                    Product.ProductType.Subscription,
                    new Product.ProductData {
                        StripeProductID = "price_1K4I3XCDoQLx7JZGBlCZp5kt"
                    },
                    new Product.SubscriptionProductData {
                        SubscriptionPeriod = Product.SubscriptionPeriods.Year
                    }
                )
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
                        StripeProductID = "price_1K9FwwCDoQLx7JZGhHDvnagk"
                    },
                    new Product.SubscriptionProductData {
                        SubscriptionPeriod = Product.SubscriptionPeriods.Year
                    }
                ),
            };
        }
    }
}