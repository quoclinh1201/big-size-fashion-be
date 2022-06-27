using BigSizeFashion.Business.Dtos.Requests;
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
    }
}
