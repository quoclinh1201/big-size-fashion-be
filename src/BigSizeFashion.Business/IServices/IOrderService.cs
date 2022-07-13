using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
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
        Task<Result<IEnumerable<NotEnoughProductResponse>>> ApproveOrder(int id);
        Task<PagedResult<ListOrderResponse>> GetListOrderOfStoreForManager(string v, FilterOrderParameter param);
        Task<Result<bool>> AssignOrder(AssignOrderRequest request);
        Task<Result<IEnumerable<ListOrderForStaffResponse>>> GetListAssignedOrder(string v, FilterOrderForStaffParameter param);
        //Task<PagedResult<ListOrderForStaffResponse>> GetListOrderOfStoreForStaff(string v, FilterOrderParameter param);
        Task<Result<IEnumerable<ListOrderForStaffResponse>>> GetListOrderOfStoreForStaff(string v, FilterOrderForStaffParameter param);
        Task<Result<bool>> PackagedOrder(int id);
        Task<Result<bool>> DeliveryOrder(int id);
        Task<Result<bool>> ReceivedOrder(int id);
        Task<Result<bool>> UpdateReceivedOrder(TrackingOrderRequest request);
        Task<Result<bool>> RejectOrder(int id);
        Task<Result<OrderResponse>> AddOrder(string authorization, OrderRequest request);
        Task<Result<bool>> CancelOrder(string v, int id);
        Task<Result<IEnumerable<GetRevenueResponse>>> GetRevenueOfOwnStore(string v, GetRevenueParameter param);
        Task<Result<IEnumerable<GetRevenueResponse>>> GetRevenueByStoreId(int id, GetRevenueParameter param);
        Task<Result<IEnumerable<NotEnoughProductResponse>>> ApproveOfflineOrder(int id);
        Task<Result<IEnumerable<StaffPerformanceResponse>>> GetStaffPerformance(string v);
        Task<Result<IEnumerable<StaffPerformanceOrderResponse>>> GetStaffPerformanceOrder(string v);
        Task<Result<Stream>> ExportBill(int id);
        Task<Result<IEnumerable<StaffPerformanceOfStoreResponse>>> GetPerformanceOfAllStaff(string v, GetRevenueParameter param);
    }
}
