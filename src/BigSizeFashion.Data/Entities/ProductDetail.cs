using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class ProductDetail
    {
        public ProductDetail()
        {
            CustomerCarts = new HashSet<CustomerCart>();
            DeliveryNoteDetails = new HashSet<DeliveryNoteDetail>();
            InventoryNoteDetails = new HashSet<InventoryNoteDetail>();
            OrderDetails = new HashSet<OrderDetail>();
            StoreWarehouses = new HashSet<StoreWarehouse>();
        }

        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }
        public int ColourId { get; set; }
        public int SizeId { get; set; }

        public virtual Colour Colour { get; set; }
        public virtual Product Product { get; set; }
        public virtual Size Size { get; set; }
        public virtual ICollection<CustomerCart> CustomerCarts { get; set; }
        public virtual ICollection<DeliveryNoteDetail> DeliveryNoteDetails { get; set; }
        public virtual ICollection<InventoryNoteDetail> InventoryNoteDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<StoreWarehouse> StoreWarehouses { get; set; }
    }
}
