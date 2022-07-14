using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class ErrorProductWhenAddToPromotionResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string ApplyDate { get; set; }
        public string ExpiredDate { get; set; }
    }
}
