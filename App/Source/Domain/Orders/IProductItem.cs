using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp
{
    public interface IProductItem
    {
        public string ItemID { get; }
        public int Quantity { get; }
    }
}