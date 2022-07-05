using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class QuantityAdjustmentRequest
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColourId { get; set; }
        public int DifferenceQuantity { get; set; }
    }
}
