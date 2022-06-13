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
    public interface ISizeService
    {
        Task<Result<IEnumerable<SizeResponse>>> GetAllSize(SearchSizeParameter param);
        Task<Result<SizeResponse>> GetSizeByID(int id);
        Task<Result<SizeResponse>> CreateSize(SizeRequest request);
        Task<Result<SizeResponse>> UpdateSize(int id, SizeRequest request);
        Task<Result<bool>> DeleteSize(int id);
    }
}
