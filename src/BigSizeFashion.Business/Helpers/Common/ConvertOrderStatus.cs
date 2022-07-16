using BigSizeFashion.Business.Helpers.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Common
{
    public static class ConvertOrderStatus
    {
        public static string ConvertOrderStatusToString(byte status)
        {
            return status switch
            {
                0 => OrderStatusConstants.Cancel,
                1 => OrderStatusConstants.Pending,
                2 => OrderStatusConstants.Approved,
                3 => OrderStatusConstants.Packaged,
                4 => OrderStatusConstants.Delivery,
                5 => OrderStatusConstants.Received,
                _ => OrderStatusConstants.Reject,
            };
        }

        public static byte ConvertStringToOrderStatus(string status)
        {
            return status switch
            {
                "Cancel" => 0,
                "Pending" => 1,
                "Approved" => 2,
                "Packaged" => 3,
                "Delivery" => 4,
                "Received" => 5,
                "Reject" => 6,
                _ => 0
            };
        }
    }
}
