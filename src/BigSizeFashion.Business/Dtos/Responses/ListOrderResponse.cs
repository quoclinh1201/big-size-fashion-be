using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class ListOrderResponse
    {
        public int OrderId { get; set; }
        public string CreateDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? TotalPriceAfterDiscount { get; set; }
        public string OrderType { get; set; }
        public string Status { get; set; }
    }
}
