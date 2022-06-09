using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class CustomerCart
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
        public virtual Store Store { get; set; }
    }
}
