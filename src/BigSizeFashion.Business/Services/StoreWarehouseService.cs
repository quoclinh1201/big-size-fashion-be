using AutoMapper;
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
    public class StoreWarehouseService : IStoreWarehouseService
    {
        private readonly IGenericRepository<StoreWarehouse> _genericRepository;
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IMapper _mapper;

        public StoreWarehouseService(IGenericRepository<StoreWarehouse> genericRepository,
            IGenericRepository<staff> staffRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
        }

        public async Task<Result<bool>> IncreaseOrDesceaseProductInStore(string token, IncreaseOrDesceaseProductRequest request)
        {
            var result = new Result<bool>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository.GetAllByIQueryable()
                    .Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();

                if(request.Action)
                {
                    foreach (var item in request.ListProduct)
                    {
                        var product = await _genericRepository.FindAsync(p => p.ProductDetailId == item.ProductDetailId);
                        product.Quantity += item.Quantity;
                        await _genericRepository.UpdateAsync(product);
                    }
                }
                else
                {
                    foreach (var item in request.ListProduct)
                    {
                        var product = await _genericRepository.FindAsync(p => p.ProductDetailId == item.ProductDetailId);
                        product.Quantity -= item.Quantity;
                        await _genericRepository.UpdateAsync(product);
                    }
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
    }
}
