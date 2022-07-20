using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class GetRevenueAndNumberOrdersResponse
    {
        public IEnumerable<GetRevenueResponse> Revenues { get; set; } = new List<GetRevenueResponse>();
        public StatisticOrderResponse NumberOrders { get; set; }
    }
}
