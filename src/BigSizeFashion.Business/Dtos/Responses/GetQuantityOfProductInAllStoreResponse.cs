using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class GetQuantityOfProductInAllStoreResponse
    {
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }
    }
}
