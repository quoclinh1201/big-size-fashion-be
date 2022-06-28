using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class DeliveryNoteDetailResponse
    {
        public int DeliveryNoteId { get; set; }
        public int ReceiveStaffId { get; set; }
        public string ReceiveStaffName { get; set; }
        public string DeliveryNoteName { get; set; }
        public StoreResponse FromStore { get; set; }
        public StoreResponse ToStore { get; set; }
        public List<DeliveryNoteDetailItem> ProductList { get; set; } = new List<DeliveryNoteDetailItem>();
        public decimal TotalPrice { get; set; }
        public string CreateDate { get; set; }
        public string ApprovalDate { get; set; }
        public string Status { get; set; }
    }
}
