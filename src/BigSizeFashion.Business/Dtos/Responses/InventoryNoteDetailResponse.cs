using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class InventoryNoteDetailResponse
    {
        public int ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public int BeginningQuantity { get; set; }
        public int EndingQuantity { get; set; }
        public int? EndingQuantityAfterAdjusted { get; set; }
    }
}
