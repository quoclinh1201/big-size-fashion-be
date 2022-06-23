using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class GetOrderDetailResponse
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DeliveryAddressResponse DeliveryAddress { get; set; }
        public StoreResponse Store { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string CreateDate { get; set; }
        public List<OrderDetailItem> ProductList { get; set; } = new List<OrderDetailItem>();
        public decimal TotalPrice { get; set; }
        public decimal? TotalPriceAfterDiscount { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderType { get; set; }
        public string ApprovalDate { get; set; }
        public string PackagedDate { get; set; }
        public string DeliveryDate { get; set; }
        public string ReceivedDate { get; set; }
        public string RejectedDate { get; set; }
        public string Status { get; set; }
    }
}
