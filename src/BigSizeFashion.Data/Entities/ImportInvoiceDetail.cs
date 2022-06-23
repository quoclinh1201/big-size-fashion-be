using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class ImportInvoiceDetail
    {
        public int ImportInvoiceId { get; set; }
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual ImportInvoice ImportInvoice { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
    }
}
