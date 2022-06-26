using BigSizeFashion.Business.Helpers.Enums;
using BigSizeFashion.Business.Helpers.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Parameters
{
    public class FeeadbackParameter : QueryStringParameters
    {
        public FeedbackSortByEnum? SortBy { get; set; }
        public bool? IsDescendingOrder { get; set; }
    }
}
