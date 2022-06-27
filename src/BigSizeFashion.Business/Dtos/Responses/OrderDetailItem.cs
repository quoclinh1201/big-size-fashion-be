using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class OrderDetailItem
    {
        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal PricePerOne { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPricePerOne { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Quantity { get; set; }

    }
}
