using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int? DeliveryAddress { get; set; }
        public int StoreId { get; set; }
        public int? StaffId { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? TotalPriceAfterDiscount { get; set; }
        public bool OrderType { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? PackagedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public byte Status { get; set; }
    }
}
