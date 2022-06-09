using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class ImportInvoice
    {
        public ImportInvoice()
        {
            ImportInvoiceDetails = new HashSet<ImportInvoiceDetail>();
        }

        public int InvoiceId { get; set; }
        public int StaffId { get; set; }
        public int MainWarehouseId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public byte Status { get; set; }

        public virtual MainWarehouse MainWarehouse { get; set; }
        public virtual staff Staff { get; set; }
        public virtual ICollection<ImportInvoiceDetail> ImportInvoiceDetails { get; set; }
    }
}
