using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class OrderDetailRequest
    {
        public int OrderId { get; set; }
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerOne { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPricePerOne { get; set; }
        public decimal? DiscountPrice { get; set; }
    }
}
