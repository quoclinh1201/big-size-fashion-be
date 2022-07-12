using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class StaffPerformanceOfStoreResponse
    {
        public int Uid { get; set; }
        public string Fullname { get; set; }
        public int QuantityOfOrders { get; set; }
        public decimal? Revenue { get; set; }
    }
}
