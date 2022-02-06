using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSeedApp {
    public class ProductDataProvider {

        private Dictionary<string, Product> _productIDToProductLookup;
        public ProductDataProvider(
            List<Product> products
        ) {
            this._productIDToProductLookup = products
                .Select(p => p)
                .ToDictionary(
                    p => p.ID,
                    p => p);
        }

        public bool TryGetProductByID(string ID, out Product? product) {
            if(!this._productIDToProductLookup.ContainsKey(ID)) {
                product = null;
                return false;
            }

            product = this._productIDToProductLookup[ID];
            return true;
        }

        public bool TryGetProductByStripePriceId(string id, out Product? product) {
            product = this._productIDToProductLookup
                .Values
                .SingleOrDefault(p => p.Data.StripeProductID == id);
            
            return product is not null;
        }
    }
}