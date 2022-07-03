using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class CreateZaloPayOrderResponse
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public int SubReturnCode { get; set; }
        public string SubReturnMessage { get; set; }
        public string OrderUrl { get; set; }
        public string ZpTransToken { get; set; }
    }
}
