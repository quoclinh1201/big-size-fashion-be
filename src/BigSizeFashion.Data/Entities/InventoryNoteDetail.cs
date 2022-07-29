using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class InventoryNoteDetail
    {
        public int InventoryNoteId { get; set; }
        public int ProductDetailId { get; set; }
        public int BeginningQuantity { get; set; }
        public int EndingQuantity { get; set; }
        public int? EndingQuantityAfterAdjusted { get; set; }

        public virtual InventoryNote InventoryNote { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
    }
}
