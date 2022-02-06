using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;

namespace CloudSeedApp {
    public class OrderProductItem : IProductItem {
        public string ItemID { get; }
        public int Quantity { get; }

        public OrderProductItem(
            string itemID,
            int quantity
        ) {
            this.ItemID = itemID;
            this.Quantity = quantity;
        }
    }
}