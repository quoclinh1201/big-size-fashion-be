﻿using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
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
        Task<PagedResult<GetListProductResponse>> GetListProductFitWithCustomer(string v, GetListProductFitWithCustomerParameters param);
        Task<decimal> GetProductPrice(int id);
        Task<decimal?> GetProductPromotionPrice(int id);
        Task<Result<GetQuantityOfProductResponse>> GetQuantityOfProduct(GetQuantityOfProductParameter param);
        Task<Result<GetQuantityOfProductResponse>> GetQuantityOfProductInStore(string v, GetQuantityOfProductInStoreParameter param);
        Task<Result<IEnumerable<ColourResponse>>> GetAllColourOfProduct(int id);
        Task<Result<IEnumerable<SizeResponse>>> GetAllSizeOfProduct(int productId, int colourId);
        Task<Result<IEnumerable<GetListProductResponse>>> GetTopTenBestSeller();
        Task<Result<IEnumerable<QuantityFitProductByCategoryResponse>>> GetQuantityFitProductByCategory(string v);
        Task<Result<GetDetailProductResponse>> GetProductByDetailID(int productId, int productDetailId);
        Task<Result<GetDetailProductResponse>> GetDetailFitProductWithCustomer(string v, int id);
        Task<Result<IEnumerable<GetAllProductToImportResponse>>> GetAllProductToImport();
        Task<Result<IEnumerable<GetAllProductToAddIntoPromotionResponse>>> GetAllProductToAddIntoPromotion();
        Task<Result<GetQuantityOfProductInAllStoreResponse>> GetQuantityOfProductInAllStore(int id);
    }
}
