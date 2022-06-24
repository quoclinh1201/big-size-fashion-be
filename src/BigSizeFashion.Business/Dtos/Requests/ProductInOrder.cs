using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class ProductInOrder
    {
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }
    }
}
