using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.ResponseObjects
{
    public class CustomerLoginResponse
    {
        public string Token { get; set; }
        public bool IsNewCustomer { get; set; }
        public bool IsHasWeightHeight { get; set; }
    }
}
