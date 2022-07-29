using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class InventoryNoteResponse
    {
        public int InventoryNoteId { get; set; }
        public string InventoryNoteName { get; set; }
        public StoreResponse Store { get; set; }
        public string StaffName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AdjustedDate { get; set; }
        public bool Status { get; set; }
        public List<InventoryNoteDetailResponse> InventoryNoteDetail { get; set; } = new List<InventoryNoteDetailResponse>();
    }
}
