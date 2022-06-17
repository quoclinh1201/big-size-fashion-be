using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.ResponseObjects
{
    public class PromotionResponse
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionValue { get; set; }
        public string ApplyDate { get; set; }
        public string ExpiredDate { get; set; }
        public bool Status { get; set; }
    }
}
