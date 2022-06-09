using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class MainWarehouse
    {
        public MainWarehouse()
        {
            ImportInvoices = new HashSet<ImportInvoice>();
        }

        public int MainWarehouseId { get; set; }
        public string Address { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<ImportInvoice> ImportInvoices { get; set; }
    }
}
