using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class DeliveryNote
    {
        public DeliveryNote()
        {
            DeliveryNoteDetails = new HashSet<DeliveryNoteDetail>();
        }

        public int DeliveryNoteId { get; set; }
        public int StaffId { get; set; }
        public int FromStore { get; set; }
        public int ToStore { get; set; }
        public string DeliveryNoteName { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public bool Status { get; set; }

        public virtual Store FromStoreNavigation { get; set; }
        public virtual staff Staff { get; set; }
        public virtual Store ToStoreNavigation { get; set; }
        public virtual ICollection<DeliveryNoteDetail> DeliveryNoteDetails { get; set; }
    }
}
