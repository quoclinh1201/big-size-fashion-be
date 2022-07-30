using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class QuantityAdjustmentRequest
    {
        public int InventoryNoteId { get; set; }
        public int ProductDetailId { get; set; }
        public int DifferenceQuantity { get; set; }
    }
}
