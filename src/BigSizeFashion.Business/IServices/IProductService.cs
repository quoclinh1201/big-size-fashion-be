using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.ResponseObjects;
using BigSizeFashion.Business.Dtos.Responses;
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
        //Task<PagedResult<GetListProductForStaffResponse>> GetListProductOfStore(string v, SearchProductsParameter param);
        //Task<Result<GetDetailProductForStaffResponse>> GetProductOfStoreByID(string v, int id);
        Task<Result<CreateProductResponse>> UpdateProduct(int id, CreateProductRequest request);
        Task<Result<bool>> DeleteProduct(int id);
        Task<PagedResult<GetListProductResponse>> GetListProductFitWithCustomer(string v, QueryStringParameters param);
        Task<decimal> GetProductPrice(int id);
        Task<decimal?> GetProductPromotionPrice(int id);
        Task<Result<GetQuantityOfProductResponse>> GetQuantityOfProduct(GetQuantityOfProductParameter param);
    }
}
