using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class AddToCartRequest
    {
        public int ProductDetailId { get; set; }
        public int StoreId { get; set; }
        public int Quantity { get; set; }
    }
}
