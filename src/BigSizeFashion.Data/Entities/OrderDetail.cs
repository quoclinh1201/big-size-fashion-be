using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerOne { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPricePerOne { get; set; }
        public decimal? DiscountPrice { get; set; }

        public virtual Order Order { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
    }
}
