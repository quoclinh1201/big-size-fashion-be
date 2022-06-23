using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class StoreWarehouse
    {
        public int StoreId { get; set; }
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }

        public virtual ProductDetail ProductDetail { get; set; }
        public virtual Store Store { get; set; }
    }
}
