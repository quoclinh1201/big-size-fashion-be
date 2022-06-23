using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class ProductDetailResponse
    {
        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }
        public SizeResponse Size { get; set; }
        public ColourResponse Colour { get; set; }
    }
}
