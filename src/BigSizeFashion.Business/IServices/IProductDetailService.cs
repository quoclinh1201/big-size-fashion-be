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
<<<<<<< HEAD
        //Task<Result<int>> GetProductDetail(GetProductDetailParameter request);
=======
        Task<Result<int>> GetProductDetailId(GetProductDetailIdParameter param);
>>>>>>> c2916260d9bfd1c2db37e33dc2217d9409648e9a
    }
}
