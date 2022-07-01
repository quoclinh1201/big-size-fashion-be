using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Constants
{
    public class VNPayConstants
    {
        public const string Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public const string ReturnUrl = "https://20.211.17.194/api/v1/vnpay/IPN";
        public const string TmnCode = "DW02ZMIM";
        public const string HashSecret = "PHHMWSAVQQWKIEBTUHDCBWECCEMCWLDH";
    }
}
