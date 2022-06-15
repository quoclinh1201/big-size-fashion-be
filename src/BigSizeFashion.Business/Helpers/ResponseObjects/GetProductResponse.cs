using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.ResponseObjects
{
    public class GetProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal PromotionPrice { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
        public string Gender { get; set; }
        public string ImageUrl { get; set; }
        //public string SupplierName { get; set; }
        //public string Description { get; set; }
        public bool Status { get; set; }
    }
}
