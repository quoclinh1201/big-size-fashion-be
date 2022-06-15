using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Parameters
{
    public class SearchProductsParameter : QueryStringParameters
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
        public string Gender { get; set; }
        public bool? OrderByPrice { get; set; }
        public bool? Status { get; set; }
    }
}
