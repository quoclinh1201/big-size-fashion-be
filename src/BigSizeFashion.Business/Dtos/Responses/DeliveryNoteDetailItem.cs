using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class DeliveryNoteDetailItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        //public List<ProductDeliveryNoteDetail> DetailList { get; set; } = new List<ProductDeliveryNoteDetail>();
        public string Colour { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
    }
}
