using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Product
    {
        public Product()
        {
            CustomerCarts = new HashSet<CustomerCart>();
            Feedbacks = new HashSet<Feedback>();
            ImportInvoiceDetails = new HashSet<ImportInvoiceDetail>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImages = new HashSet<ProductImage>();
            PromotionDetails = new HashSet<PromotionDetail>();
            StoreWarehouses = new HashSet<StoreWarehouse>();
        }

        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int SizeId { get; set; }
        public int ColourId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool Gender { get; set; }
        public string SupplierName { get; set; }
        public bool Status { get; set; }

        public virtual Category Category { get; set; }
        public virtual Colour Colour { get; set; }
        public virtual Size Size { get; set; }
        public virtual ICollection<CustomerCart> CustomerCarts { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<ImportInvoiceDetail> ImportInvoiceDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<PromotionDetail> PromotionDetails { get; set; }
        public virtual ICollection<StoreWarehouse> StoreWarehouses { get; set; }
    }
}
