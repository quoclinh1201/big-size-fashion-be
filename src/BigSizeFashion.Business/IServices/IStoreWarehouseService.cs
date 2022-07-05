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
    public interface IStoreWarehouseService
    {
        Task<Result<bool>> IncreaseOrDesceaseProductInStore(string v, IncreaseOrDesceaseProductRequest request);
        Task<Result<CheckWarehouseResponse>> CheckWarehouse(string v, CheckWarehouseRequest request);
        Task<Result<bool>> QuantityAdjustment(string v, List<QuantityAdjustmentRequest> request);
    }
}
