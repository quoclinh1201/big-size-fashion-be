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
    public interface IProductService
    {
        Task<PagedResult<GetListProductResponse>> GetListProductsWithAllStatus(SearchProductsParameter param);
        Task<Result<CreateProductResponse>> CreateProduct(CreateProductRequest request);
        Task<Result<GetDetailProductResponse>> GetProductByID(int id);
    }
}
