using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class CartResponse
    {
        public string ProductName { get; set; }
        public int ProductDetailId { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal? ProductPromotion { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
    }
}
