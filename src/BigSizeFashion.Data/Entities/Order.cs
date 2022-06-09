using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int DeliveryAddress { get; set; }
        public int StoreId { get; set; }
        public int? StaffId { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? TotalPriceAfterDiscount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? PackagedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public byte Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Address DeliveryAddressNavigation { get; set; }
        public virtual staff Staff { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
