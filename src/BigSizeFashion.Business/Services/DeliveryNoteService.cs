using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.ResponseObjects;
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
    public class DeliveryNoteService : IDeliveryNoteService
    {
        private readonly IGenericRepository<DeliveryNote> _genericRepository;
        private readonly IGenericRepository<DeliveryNoteDetail> _noteDetailRepository;
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<Store> _storeRepository;
        private readonly IGenericRepository<ProductDetail> _productDetailRepository;
        private readonly IGenericRepository<ProductImage> _productImageRepository;
        private readonly IMapper _mapper;

        public DeliveryNoteService(IGenericRepository<DeliveryNote> genericRepository,
            IGenericRepository<DeliveryNoteDetail> noteDetailRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<Store> storeRepository,
            IGenericRepository<ProductDetail> productDetailRepository,
            IGenericRepository<ProductImage> productImageRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _noteDetailRepository = noteDetailRepository;
            _staffRepository = staffRepository;
            _storeRepository = storeRepository;
            _productDetailRepository = productDetailRepository;
            _productImageRepository = productImageRepository;
            _mapper = mapper;
        }

        public async Task<Result<DeliveryNoteDetailResponse>> GetDeliveryNoteDetail(int id)
        {
            var result = new Result<DeliveryNoteDetailResponse>();
            try
            {
                var dn = await _genericRepository.GetAllByIQueryable()
                    .Where(d => d.DeliveryNoteId == id)
                    .Include(d => d.Staff)
                    .Include(d => d.DeliveryNoteDetails)
                    .FirstOrDefaultAsync();
                var response = _mapper.Map<DeliveryNoteDetailResponse>(dn);
                var fromStore = await _storeRepository.FindAsync(s => s.StoreId == dn.FromStore);
                var toStore = await _storeRepository.FindAsync(s => s.StoreId == dn.ToStore);
                response.FromStore = _mapper.Map<StoreResponse>(fromStore);
                response.ToStore = _mapper.Map<StoreResponse>(toStore);

                foreach (var item in dn.DeliveryNoteDetails)
                {
                    var product = await _productDetailRepository.GetAllByIQueryable()
                        .Where(p => p.ProductDetailId == item.ProductDetailId)
                        .Include(p => p.Size)
                        .Include(p => p.Colour)
                        .Include(p => p.Product)
                        .ThenInclude(p => p.Category).FirstOrDefaultAsync();

                    var imageUrl = await _productImageRepository.GetAllByIQueryable().Where(i => i.ProductId == product.Product.ProductId && i.IsMainImage == true).Select(i => i.ImageUrl).FirstOrDefaultAsync();
                    var dndi = _mapper.Map<DeliveryNoteDetailItem>(product);
                    dndi.ImageUrl = imageUrl ?? CommonConstants.NoImageUrl;
                    dndi.Price = item.Price;
                    response.ProductList.Add(dndi);
                }
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<PagedResult<ListImportProductResponse>> GetListExportProduct(string token, ImportProductParameter param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var fromStoreId = await _staffRepository.GetAllByIQueryable()
                    .Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();
                var dn = await _genericRepository.FindByAsync(d => d.FromStore == fromStoreId);
                var query = dn.AsQueryable();
                FilterByName(ref query, param.DeliveryNoteName);
                query.OrderByDescending(q => q.CreateDate);
                var result = _mapper.Map<List<ListImportProductResponse>>(query.ToList());
                return PagedResult<ListImportProductResponse>.ToPagedList(result, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PagedResult<ListImportProductResponse>> GetListRequestImportProduct(string token, ImportProductParameter param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var toStoreId = await _staffRepository.GetAllByIQueryable()
                    .Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();
                var dn = await _genericRepository.FindByAsync(d => d.ToStore == toStoreId);
                var query = dn.AsQueryable();
                FilterByName(ref query, param.DeliveryNoteName);
                query.OrderByDescending(q => q.CreateDate);
                var result = _mapper.Map<List<ListImportProductResponse>>(query.ToList());
                return PagedResult<ListImportProductResponse>.ToPagedList(result, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void FilterByName(ref IQueryable<DeliveryNote> query, string deliveryNoteName)
        {
            if(!query.Any() || String.IsNullOrEmpty(deliveryNoteName) || String.IsNullOrWhiteSpace(deliveryNoteName)) {
                return;
            }

            query = query.Where(q => q.DeliveryNoteName.ToLower().Contains(deliveryNoteName.ToLower()));
        }
    }
}
