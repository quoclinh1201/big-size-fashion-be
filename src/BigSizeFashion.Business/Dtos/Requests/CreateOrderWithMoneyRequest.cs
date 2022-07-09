using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class CreateOrderWithMoneyRequest
    {
        public decimal TotalPrice { get; set; }
    }
}
