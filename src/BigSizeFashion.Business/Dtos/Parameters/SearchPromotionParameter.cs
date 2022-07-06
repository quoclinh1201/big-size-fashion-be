using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Parameters
{
    public class SearchPromotionParameter : QueryStringParameters
    {
        public string PromotionName { get; set; }
        public bool? OrderByExpiredDate { get; set; }
        public bool? Status { get; set; }
    }
}
