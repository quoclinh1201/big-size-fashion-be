using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IStoreService
    {
        Task<Result<IEnumerable<StoreResponse>>> GetAllStore(SearchStoreParameter param);
        Task<Result<StoreResponse>> GetStoreByID(int id);
        Task<Result<StoreResponse>> CreateStore(CreateStoreRequest request);
        Task<Result<StoreResponse>> UpdateStore(int id, CreateStoreRequest request);
        Task<Result<bool>> DeleteStore(int id);
        public Task<int> GetNearestStore(string receiveAddress);

        
    }
}
