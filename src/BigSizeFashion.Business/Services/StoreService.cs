using AutoMapper;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using GoogleApi;
using GoogleApi.Entities.Interfaces;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.DistanceMatrix.Request;
using Microsoft.EntityFrameworkCore;
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
        private readonly IGenericRepository<ProductDetail> _productDetailRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IMapper _mapper;

        public StoreService(
            IGenericRepository<Store> genericRepository,
            IGenericRepository<StoreWarehouse> storeWarehouseRepository,
            IGenericRepository<ProductDetail> productDetailRepository,
            IGenericRepository<Account> accountRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _storeWarehouseRepository = storeWarehouseRepository;
            _productDetailRepository = productDetailRepository;
            _accountRepository = accountRepository;
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

                await InsertProductIntoStore(store.StoreId);

                result.Content = _mapper.Map<StoreResponse>(store);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private async Task InsertProductIntoStore(int id)
        {
            //var allProductId = await  _productRepository.GetAllByIQueryable().Where(p => p.Status == true).Select(p => p.ProductId).ToListAsync();
            var allProductId = await _productDetailRepository.GetAllByIQueryable()
                                    .Include(p => p.Product)
                                    .Where(p => p.Product.Status == true)
                                    .Select(p => p.ProductDetailId).ToListAsync();

            if (allProductId.Count > 0)
            {
                foreach (var productId in allProductId)
                {
                    var newItem = new AddNewProductIntoStoreRequest {
                        StoreId = id,
                        ProductDetailId = productId,
                        Quantity = 0
                    };
                    var storeWarehouse = _mapper.Map<StoreWarehouse>(newItem);
                    await _storeWarehouseRepository.InsertAsync(storeWarehouse);
                    await _storeWarehouseRepository.SaveAsync();
                }
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
                FilterStoreByStatus(ref query, param.Status);
                var list = _mapper.Map<List<StoreResponse>>(query.ToList());
                for (int i = 0; i < list.Count; i++)
                {
                    var acc = await _accountRepository.GetAllByIQueryable()
                        .Include(a => a.staff)
                        .Where(a => a.RoleId == 2 && a.staff.StoreId == list[i].StoreId && a.Status == true && a.staff.Status == true)
                        .Select(a => a.staff.Fullname)
                        .FirstOrDefaultAsync();
                    list[i].ManagerName = acc;
                }
                result.Content = list;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private void FilterStoreByStatus(ref IQueryable<Store> query, bool? status)
        {
            if (!query.Any() || status is null)
            {
                return;
            }

            if (status is true)
            {
                query = query.Where(x => x.Status == true);
            }
            else
            {
                query = query.Where(x => x.Status == false);
            }
        }

        public async Task<Result<StoreResponse>> GetStoreByID(int id)
        {
            var result = new Result<StoreResponse>();
            try
            {
                var store = await _genericRepository.FindAsync(s => s.StoreId == id);
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

                if(store.Status)
                {
                    store.Status = false;
                }
                else
                {
                    store.Status = true;
                }
                
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

        public async Task<Result<NearestStoreResponse>> GetNearestStore(string receiveAddress)
        {
            var result = new Result<NearestStoreResponse>();
            try
            {
                var storeAddressList = await _genericRepository.GetAllByIQueryable()
                        .Where(s => s.Status == true)
                        .ToListAsync();

                var list = new List<LocationEx>();
                var listAddress = new Dictionary<int, int?>();

                foreach (var item in storeAddressList)
                {
                    list.Add(new LocationEx(new GoogleApi.Entities.Common.Address(item.StoreAddress)));
                    listAddress.Add(item.StoreId, null);
                }
                
                var request = new DistanceMatrixRequest
                {
                    Key = "AIzaSyBR35E655MG-5NlhjtAT_ovbwQgk82OKAg",
                    Origins = new[]
                    {
                        new LocationEx(new GoogleApi.Entities.Common.Address(receiveAddress))
                    },
                    Destinations = list
                };

                var response = await GoogleMaps.DistanceMatrix.QueryAsync(request);
                var index = 0;
                var test = response.RawJson;

                foreach (var item in listAddress)
                {
                    listAddress[item.Key] = response.Rows.ElementAt(0).Elements.ElementAt(index).Distance.Value;
                    index++;
                }
                var storeId = listAddress.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
                decimal shippingFee = 0;

                if(listAddress[storeId] <= 3000)
                {
                    shippingFee = 15000;
                }
                else
                {
                    shippingFee = Math.Ceiling((decimal)(listAddress[storeId] / 1000)) * 5000;
                }

                var nearest = new NearestStoreResponse();
                nearest.StoreId = storeId;
                nearest.ShippingFee = shippingFee;

                result.Content = nearest;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
