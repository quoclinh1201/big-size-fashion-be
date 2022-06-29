using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class ListImportProductResponse
    {
        public int DeliveryNoteId { get; set; }
        public int DeliveryNoteName { get; set; }
        public decimal TotalPrice { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
    }
}
