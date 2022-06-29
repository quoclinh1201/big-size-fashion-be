using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class QuantityFitProductByCategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int QuantityFitProduct { get; set; }
    }
}
