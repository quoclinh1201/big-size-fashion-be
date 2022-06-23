using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Constants
{
    public class OrderStatusConstants
    {
        public const string Cancel = "Đã hủy";
        public const string Pending = "Chờ xác nhận";
        public const string Approved = "Đã xác nhận";
        public const string Packaged = "Đã đóng gói";
        public const string Delivery = "Đang giao";
        public const string Received = "Đã nhận hàng";
        public const string Reject = "Từ chối";
    }
}
