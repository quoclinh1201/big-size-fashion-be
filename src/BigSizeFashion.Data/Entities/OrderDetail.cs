using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
