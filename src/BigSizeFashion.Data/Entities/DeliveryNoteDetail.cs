using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class DeliveryNoteDetail
    {
        public int DeliveryNoteId { get; set; }
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual DeliveryNote DeliveryNote { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
    }
}
