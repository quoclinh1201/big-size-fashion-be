using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
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
        Task<PagedResult<ListOrderResponse>> GetListOrderForCustomer(string v, FilterOrderParameter param);
        Task<Result<bool>> ApproveOrder(int id);
        Task<PagedResult<ListOrderResponse>> GetListOrderOfStoreForManager(string v, FilterOrderParameter param);
        Task<Result<bool>> AssignOrder(AssignOrderRequest request);
        Task<PagedResult<ListOrderResponse>> GetListAssignedOrder(string v, QueryStringParameters param);
        Task<PagedResult<ListOrderResponse>> GetListOrderOfStoreForStaff(string v, FilterOrderParameter param);
        Task<Result<bool>> PackagedOrder(int id);
        Task<Result<bool>> DeliveryOrder(int id);
        Task<Result<bool>> ReceivedOrder(int id);
        Task<Result<bool>> RejectOrder(int id);
        Task<Result<bool>> CancelOrder(int id);
        Task<bool> AddOrder(string authorization, OrderRequest request);
    }
}
