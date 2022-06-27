using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IProductDetailService
    {
        Task<Result<bool>> AddDetailForProduct(ProductDetailRequest request);
        Task<Result<int>> GetProductDetailId(GetProductDetailIdParameter param);
    }
}
