﻿using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class CustomerCart
    {
        public int CustomerId { get; set; }
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
    }
}
