using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services
{
    public class ProductDetailService : IProductDetailService
    {
        private readonly IGenericRepository<ProductDetail> _genericRepository;
        private readonly IGenericRepository<Store> _storeRepository;
        private readonly IGenericRepository<StoreWarehouse> _storeWarehouseRepository;
        private readonly IMapper _mapper;

        public ProductDetailService(IGenericRepository<ProductDetail> genericRepository,
            IGenericRepository<Store> storeRepository,
            IGenericRepository<StoreWarehouse> storeWarehouseRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _storeRepository = storeRepository;
            _storeWarehouseRepository = storeWarehouseRepository;
            _mapper = mapper;
        }

        public async Task<Result<bool>> AddDetailForProduct(ProductDetailRequest request)
        {
            var result = new Result<bool>();
            try
            {
                foreach (var sizeId in request.SizeIdList)
                {
                    var pd = new ProductDetail
                    {
                        ProductId = request.ProductId,
                        ColourId = request.ColourId,
                        SizeId = sizeId
                    };
                    await _genericRepository.InsertAsync(pd);
                    await _genericRepository.SaveAsync();
                    await AddNewProductIntoAllStore(pd.ProductDetailId);
                }
                
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<int>> GetProductDetailId(GetProductDetailIdParameter param)
        {
            var result = new Result<int>();
            try
            {
                var productDetailId = await _genericRepository.GetAllByIQueryable()
                    .Where(p => p.ProductId == param.ProductId && p.SizeId == param.SizeId && p.ColourId == param.ColourId)
                    .Select(p => p.ProductDetailId).FirstOrDefaultAsync();

                result.Content = productDetailId;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private async Task AddNewProductIntoAllStore(int productDetailId)
        {
            var allStoreId = await _storeRepository.GetAllByIQueryable().Where(s => s.Status == true).Select(s => s.StoreId).ToListAsync();
            if (allStoreId.Count > 0)
            {
                foreach (var id in allStoreId)
                {
                    var storeWarehoust = new StoreWarehouse { ProductDetailId = productDetailId, StoreId = id, Quantity = 0 };
                    await _storeWarehouseRepository.InsertAsync(storeWarehoust);
                }
                await _storeWarehouseRepository.SaveAsync();
            }
        }
    }
}
