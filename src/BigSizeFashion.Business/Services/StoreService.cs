using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services
{
    public class StoreService : IStoreService
    {
        private readonly IGenericRepository<Store> _genericRepository;
        private readonly IGenericRepository<StoreWarehouse> _storeWarehouseRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public StoreService(
            IGenericRepository<Store> genericRepository,
            IGenericRepository<StoreWarehouse> storeWarehouseRepository,
            IGenericRepository<Product> productRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _storeWarehouseRepository = storeWarehouseRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Result<StoreResponse>> CreateStore(CreateStoreRequest request)
        {
            var result = new Result<StoreResponse>();
            try
            {
                var store = _mapper.Map<Store>(request);
                await _genericRepository.InsertAsync(store);
                await _genericRepository.SaveAsync();

                InsertProductIntoStore(store.StoreId);

                result.Content = _mapper.Map<StoreResponse>(store);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private async void InsertProductIntoStore(int id)
        {
            var allProductId =  _productRepository.GetAllByIQueryable().Where(p => p.Status == true).Select(p => p.ProductId).ToList();
            
            if(allProductId.Count > 0)
            {
                foreach (var productId in allProductId)
                {
                    var storeWarehouse = new StoreWarehouse
                    {
                        StoreId = id,
                        ProductId = productId,
                        Quantity = 0
                    };
                    await _storeWarehouseRepository.InsertAsync(storeWarehouse);
                }
                await _storeWarehouseRepository.SaveAsync();
            }
        }

        public async Task<Result<IEnumerable<StoreResponse>>> GetAllStore(SearchStoreParameter param)
        {
            var result = new Result<IEnumerable<StoreResponse>>();
            try
            {
                var stores = await _genericRepository.GetAllAsync();
                var query = stores.AsQueryable();
                FilterStoreByAddress(ref query, param.StoreAddress);
                result.Content = _mapper.Map<List<StoreResponse>>(query.ToList());
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<StoreResponse>> GetStoreByID(int id)
        {
            var result = new Result<StoreResponse>();
            try
            {
                var store = await _genericRepository.FindAsync(s => s.StoreId == id && s.Status == true);
                result.Content = _mapper.Map<StoreResponse>(store);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private void FilterStoreByAddress(ref IQueryable<Store> query, string storeAddress)
        {
            if (!query.Any() || String.IsNullOrEmpty(storeAddress) || String.IsNullOrWhiteSpace(storeAddress))
            {
                return;
            }

            query = query.Where(q => q.StoreAddress.ToLower().Contains(storeAddress.ToLower()));
        }

        public async Task<Result<StoreResponse>> UpdateStore(int id, CreateStoreRequest request)
        {
            var result = new Result<StoreResponse>();
            try
            {
                var store = await _genericRepository.FindAsync(s => s.StoreId == id);
                var model = _mapper.Map(request, store);
                await _genericRepository.UpdateAsync(model);
                result.Content = _mapper.Map<StoreResponse>(model);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeleteStore(int id)
        {
            var result = new Result<bool>();
            try
            {
                var store = await _genericRepository.FindAsync(s => s.StoreId == id);

                if (store is null)
                {
                    result.Content = false;
                    return result;
                }

                store.Status = false;
                await _genericRepository.UpdateAsync(store);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }
    }
}
