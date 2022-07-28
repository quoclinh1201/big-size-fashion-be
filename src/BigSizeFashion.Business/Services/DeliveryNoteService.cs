using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
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
        private readonly IGenericRepository<StoreWarehouse> _storeWarehouseRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public DeliveryNoteService(IGenericRepository<DeliveryNote> genericRepository,
            IGenericRepository<DeliveryNoteDetail> noteDetailRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<Store> storeRepository,
            IGenericRepository<ProductDetail> productDetailRepository,
            IGenericRepository<ProductImage> productImageRepository,
            IGenericRepository<StoreWarehouse> storeWarehouseRepository,
            IGenericRepository<Account> accountRepository,
            IProductService productService,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _noteDetailRepository = noteDetailRepository;
            _staffRepository = staffRepository;
            _storeRepository = storeRepository;
            _productDetailRepository = productDetailRepository;
            _productImageRepository = productImageRepository;
            _productService = productService;
            _storeWarehouseRepository = storeWarehouseRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<NotEnoughProductResponse>>> ApproveRequestImportProduct(int id)
        {
            var result = new Result<IEnumerable<NotEnoughProductResponse>>();
            var list = new List<NotEnoughProductResponse>();
            try
            {
                //var uid = DecodeToken.DecodeTokenToGetUid(token);
                //var staff = await _staffRepository.FindAsync(s => s.Uid == uid && s.Status == true);
                var pn = await _genericRepository.FindAsync(p => p.DeliveryNoteId == id);

                var pnd = await _noteDetailRepository.FindByAsync(p => p.DeliveryNoteId == id);

                foreach (var item in pnd)
                {
                    var storeWarehouse = await _storeWarehouseRepository.GetAllByIQueryable()
                        .Where(s => s.StoreId == pn.FromStore && s.ProductDetailId == item.ProductDetailId)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Colour)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Product)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Size)
                        .FirstOrDefaultAsync();

                    if (storeWarehouse.Quantity < item.Quantity)
                    {
                        var cc = new NotEnoughProductResponse
                        {
                            ColourId = storeWarehouse.ProductDetail.ColourId,
                            ColourName = storeWarehouse.ProductDetail.Colour.ColourName,
                            ProductId = storeWarehouse.ProductDetail.ProductId,
                            ProductName = storeWarehouse.ProductDetail.Product.ProductName,
                            SizeId = storeWarehouse.ProductDetail.SizeId,
                            SizeName = storeWarehouse.ProductDetail.Size.SizeName,
                            QuantityInStore = storeWarehouse.Quantity,
                            RequiredQuantity = item.Quantity
                        };
                        list.Add(cc);
                    }
                }

                if(list.Count == 0)
                {
                    foreach (var item in pnd)
                    {
                        var from = await _storeWarehouseRepository.FindAsync(s => s.StoreId == pn.FromStore && s.ProductDetailId == item.ProductDetailId);
                        from.Quantity -= item.Quantity;
                        await _storeWarehouseRepository.UpdateAsync(from);

                        var to = await _storeWarehouseRepository.FindAsync(s => s.StoreId == pn.ToStore && s.ProductDetailId == item.ProductDetailId);
                        to.Quantity += item.Quantity;
                        await _storeWarehouseRepository.UpdateAsync(to);
                    }
                    pn.ApprovalDate = DateTime.UtcNow.AddHours(7);
                    pn.Status = 2;
                    await _genericRepository.UpdateAsync(pn);
                    result.Content = null;
                    return result;
                }
                else
                {
                    result.Content = list;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> CreateRequestImportProduct(string token, ImportProductRequest request)
        {
            var result = new Result<bool>();
            try
            {
                if(request.ListProducts.Count() > 1)
                {
                    var dupes = request.ListProducts.GroupBy(x => new { x.ProductId, x.SizeId, x.ColourId })
                            .Where(x => x.Skip(1).Any());
                    if (dupes != null)
                    {
                        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, "Trùng sản phẩm!");
                        return result;
                    }
                }

                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var staff = await _staffRepository.FindAsync(s => s.Uid == uid && s.Status == true);
                var import = new DeliveryNote();
                import.StaffId = uid;
                import.FromStore = request.FromStoreId;
                import.ToStore = staff.StoreId;
                import.DeliveryNoteName = request.DeliveryNoteName;
                import.CreateDate = DateTime.UtcNow.AddHours(7);
                import.Status = 1;
                decimal totalPrice = 0;

                foreach (var item in request.ListProducts)
                {
                    var price = await _productService.GetProductPrice(item.ProductId) * item.Quantity;
                    totalPrice += price;
                }
                import.TotalPrice = totalPrice;
                await _genericRepository.InsertAsync(import);
                await _genericRepository.SaveAsync();

                foreach (var item in request.ListProducts)
                {
                    var pd = await _productDetailRepository
                        .GetAllByIQueryable()
                        .Where(p => p.ProductId == item.ProductId && p.ColourId == item.ColourId && p.SizeId == item.SizeId)
                        .Select(p => p.ProductDetailId)
                        .FirstOrDefaultAsync();
                    var pnd = new DeliveryNoteDetail
                    {
                        DeliveryNoteId = import.DeliveryNoteId,
                        ProductDetailId = pd,
                        Price = await _productService.GetProductPrice(item.ProductId) * item.Quantity,
                        Quantity = item.Quantity
                    };
                    await _noteDetailRepository.InsertAsync(pnd);
                    await _noteDetailRepository.SaveAsync();
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

        public async Task<Result<DeliveryNoteDetailResponse>> GetDeliveryNoteDetail(int id)
        {
            var result = new Result<DeliveryNoteDetailResponse>();
            try
            {
                var m1 = "";
                var m2 = "";
                var dn = await _genericRepository.GetAllByIQueryable()
                    .Where(d => d.DeliveryNoteId == id)
                    .Include(d => d.Staff)
                    .Include(d => d.DeliveryNoteDetails)
                    .FirstOrDefaultAsync();
                var response = _mapper.Map<DeliveryNoteDetailResponse>(dn);
                var fromStore = await _storeRepository.FindAsync(s => s.StoreId == dn.FromStore);

                if(fromStore.IsMainWarehouse)
                {
                    m1 = await _accountRepository.GetAllByIQueryable()
                        .Include(a => a.User)
                        .Where(a => a.RoleId == 1 && a.User.Status == true && a.Status == true)
                        .Select(a => a.User.Fullname)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    m1 = await _accountRepository.GetAllByIQueryable()
                        .Include(a => a.staff)
                        .Where(a => a.RoleId == 2 && a.staff.StoreId == fromStore.StoreId && a.Status == true && a.staff.Status == true)
                        .Select(a => a.staff.Fullname)
                        .FirstOrDefaultAsync();
                }
                
                var toStore = await _storeRepository.FindAsync(s => s.StoreId == dn.ToStore);
                if(toStore.IsMainWarehouse)
                {
                    m2 = await _accountRepository.GetAllByIQueryable()
                        .Include(a => a.User)
                        .Where(a => a.RoleId == 1 && a.User.Status == true && a.Status == true)
                        .Select(a => a.User.Fullname)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    m2 = await _accountRepository.GetAllByIQueryable()
                        .Include(a => a.staff)
                        .Where(a => a.RoleId == 2 && a.staff.StoreId == toStore.StoreId && a.Status == true && a.staff.Status == true)
                        .Select(a => a.staff.Fullname)
                        .FirstOrDefaultAsync();
                }
                
                response.FromStore = _mapper.Map<StoreResponse>(fromStore);
                response.FromStore.ManagerName = m1;
                response.ToStore = _mapper.Map<StoreResponse>(toStore);
                response.ToStore.ManagerName = m2;

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
                    dndi.Quantity = item.Quantity;
                    dndi.PricePerOne = item.Price / item.Quantity;
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

        public async Task<PagedResult<ListExportProductResponse>> GetListExportProduct(string token, ImportProductParameter param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var fromStoreId = await _staffRepository.GetAllByIQueryable()
                    .Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();
                var dn = await _genericRepository.GetAllByIQueryable()
                        .Where(d => d.FromStore == fromStoreId)
                        .Include(d => d.ToStoreNavigation)
                        .ToListAsync();
                var query = dn.AsQueryable();
                FilterByStatus(ref query, param.Status);
                FilterByName(ref query, param.DeliveryNoteName);
                OrderByStatus(ref query, param.Status);
                var result = _mapper.Map<List<ListExportProductResponse>>(query.ToList());
                for (int i = 0; i < result.Count; i++)
                {
                    var date = GetDateOfDelivernote(query, result[i].DeliveryNoteId, param.Status);
                    result[i].CreateDate = ConvertDateTime.ConvertDateTimeToString(date);
                }
                return PagedResult<ListExportProductResponse>.ToPagedList(result, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PagedResult<ListExportProductResponse>> GetListExportProductForMainWarehouse(string token, ImportProductParameter param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var fromStores = await _storeRepository.FindByAsync(f => f.IsMainWarehouse == true);
                var list = new List<DeliveryNote>();

                foreach (var item in fromStores)
                {
                    var dn = await _genericRepository.GetAllByIQueryable()
                        .Where(d => d.FromStore == item.StoreId)
                        .Include(d => d.ToStoreNavigation)
                        .ToListAsync();
                    foreach (var i in dn)
                    {
                        list.Add(i);
                    }
                }
                
                var query = list.AsQueryable();
                FilterByStatus(ref query, param.Status);
                FilterByName(ref query, param.DeliveryNoteName);
                OrderByStatus(ref query, param.Status);
                var result = _mapper.Map<List<ListExportProductResponse>>(query.ToList());
                for (int i = 0; i < result.Count; i++)
                {
                    var date = GetDateOfDelivernote(query, result[i].DeliveryNoteId, param.Status);
                    result[i].CreateDate = ConvertDateTime.ConvertDateTimeToString(date);
                }
                return PagedResult<ListExportProductResponse>.ToPagedList(result, param.PageNumber, param.PageSize);
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
                var dn = await _genericRepository.GetAllByIQueryable()
                    .Include(s => s.FromStoreNavigation)
                    .Where(d => d.ToStore == toStoreId)
                    .ToListAsync();
                var query = dn.AsQueryable();
                FilterByStatus(ref query, param.Status);
                FilterByName(ref query, param.DeliveryNoteName);
                OrderByStatus(ref query, param.Status);
                var result = _mapper.Map<List<ListImportProductResponse>>(query.ToList());
                for (int i = 0; i < result.Count; i++)
                {
                    var date = GetDateOfDelivernote(query, result[i].DeliveryNoteId, param.Status);
                    result[i].CreateDate = ConvertDateTime.ConvertDateTimeToString(date);
                }
                return PagedResult<ListImportProductResponse>.ToPagedList(result, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void OrderByStatus(ref IQueryable<DeliveryNote> query, byte? status)
        {
            if (!query.Any())
            {
                return;
            }

            if(status == 2)
            {
                query = query.OrderByDescending(q => q.ApprovalDate);
            }
            else
            {
                query = query.OrderByDescending(q => q.CreateDate);
            }
        }

        private DateTime? GetDateOfDelivernote(IQueryable<DeliveryNote> query, int id, byte? status)
        {
            if (!query.Any())
            {
                return null;
            }

            if(status == 2)
            {
                return query.Where(q => q.DeliveryNoteId == id && q.Status == 2).Select(q => q.ApprovalDate).FirstOrDefault();
            }
            else
            {
                return query.Where(q => q.DeliveryNoteId == id).Select(q => q.CreateDate).FirstOrDefault();
            }
        }

        private void FilterByStatus(ref IQueryable<DeliveryNote> query, byte? status)
        {
            if(!query.Any() || status == null)
            {
                return;
            }
            query = query.Where(q => q.Status == status);
        }

        public async Task<Result<bool>> RejectRequestImportProduct(int id)
        {
            var result = new Result<bool>();
            try
            {
                var pn = await _genericRepository.FindAsync(p => p.DeliveryNoteId == id);
                pn.Status = 0;
                await _genericRepository.UpdateAsync(pn);

                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
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
