using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class ListExportProductResponse
    {
        public int DeliveryNoteId { get; set; }
        public string DeliveryNoteName { get; set; }
        public string RequestStore { get; set; }
        public string StoreAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
    }
}
