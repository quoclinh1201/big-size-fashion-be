using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.ResponseObjects
{
    public class GetListProductForStaffResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PromotionPrice { get; set; }
        public string PromotionValue { get; set; }
        public string ImageUrl { get; set; }
        public bool Status { get; set; }
    }
}
