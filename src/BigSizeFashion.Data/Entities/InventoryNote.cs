using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class InventoryNote
    {
        public InventoryNote()
        {
            InventoryNoteDetails = new HashSet<InventoryNoteDetail>();
        }

        public int InventoryNoteId { get; set; }
        public int StoreId { get; set; }
        public int StaffId { get; set; }
        public string InventoryNoteName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime? AdjustedDate { get; set; }
        public bool Status { get; set; }

        public virtual staff Staff { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<InventoryNoteDetail> InventoryNoteDetails { get; set; }
    }
}
