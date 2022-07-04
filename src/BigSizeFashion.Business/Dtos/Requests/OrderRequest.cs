using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class OrderRequest
    {
        public int DeliveryAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PromotionPrice { get; set; }
        public bool OrderType { get; set; }
        public string PaymentMethod { get; set; }
        public decimal? ShippingFee { get; set; }
        public int StoreId { get; set; }
    }
}
