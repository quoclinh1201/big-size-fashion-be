using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IOrderService
    {
        Task<Result<OrderIdResponse>> CreateOrderForCustomer(string v, CreateOrderForCustomerRequest request);
        Task<Result<GetOrderDetailResponse>> GetOrderDetailById(int id);
        //Task<PagedResult<ListOrderResponse>> GetListOrderForCustomer(string v, FilterOrderParameter param);
    }
}
