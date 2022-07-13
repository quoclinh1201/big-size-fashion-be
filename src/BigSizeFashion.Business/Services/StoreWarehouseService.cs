using AutoMapper;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
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
        private readonly IGenericRepository<ProductDetail> _productDetailRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<DeliveryNote> _deliveryNoteRepository;
        private readonly IGenericRepository<DeliveryNoteDetail> _deliveryNoteDetailRepository;
        private readonly IMapper _mapper;

        public StoreWarehouseService(IGenericRepository<StoreWarehouse> genericRepository,
            IGenericRepository<ProductDetail> productDetailRepository,
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<DeliveryNote> deliveryNoteRepository,
            IGenericRepository<DeliveryNoteDetail> deliveryNoteDetailRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _staffRepository = staffRepository;
            _productDetailRepository = productDetailRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _deliveryNoteRepository = deliveryNoteRepository;
            _deliveryNoteDetailRepository = deliveryNoteDetailRepository;
            _mapper = mapper;
        }

        public async Task<Result<CheckWarehouseResponse>> CheckWarehouse(string token, CheckWarehouseRequest request)
        {
            var result = new Result<CheckWarehouseResponse>();
            var response = new CheckWarehouseResponse();
            try
            {
                response.FromDate = request.FromDate;
                response.ToDate = request.ToDate;
                var from = ConvertDateTime.ConvertStringToDate(request.FromDate);
                var to = ConvertDateTime.ConvertStringToDate(request.ToDate);
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository.GetAllByIQueryable()
                    .Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();
                var map = new Dictionary<int, int>();

                var orders = await _orderRepository
                        .FindByAsync(o => o.StoreId == storeId
                        && o.ApprovalDate.Value.Day >= from.Value.Day
                        && o.ApprovalDate.Value.Month >= from.Value.Month
                        && o.ApprovalDate.Value.Year >= from.Value.Year
                        && o.ApprovalDate.Value.Day <= to.Value.Day
                        && o.ApprovalDate.Value.Month <= to.Value.Month
                        && o.ApprovalDate.Value.Year <= to.Value.Year
                        && o.Status != 0
                        && o.Status != 1
                        && o.Status != 6);

                var deliverynotes = await _deliveryNoteRepository
                        .FindByAsync(o => (o.ToStore == storeId || o.FromStore == storeId)
                        && o.ApprovalDate.Value.Day >= from.Value.Day
                        && o.ApprovalDate.Value.Month >= from.Value.Month
                        && o.ApprovalDate.Value.Year >= from.Value.Year
                        && o.ApprovalDate.Value.Day <= to.Value.Day
                        && o.ApprovalDate.Value.Month <= to.Value.Month
                        && o.ApprovalDate.Value.Year <= to.Value.Year
                        && o.Status == 2);

                foreach (var item in request.ListProducts)
                {
                    var cc = new Dtos.Responses.CheckWarehouseItem();
                    var eq = await _genericRepository
                        .GetAllByIQueryable()
                        .Include(e => e.ProductDetail.Colour)
                        .Include(e => e.ProductDetail.Product)
                        .Include(e => e.ProductDetail.Size)
                        .Include(e => e.ProductDetail)
                        .Where(e => e.ProductDetail.ProductId == item.productId
                                && e.ProductDetail.SizeId == item.SizeId
                                && e.ProductDetail.ColourId == item.ColourId)
                        .FirstOrDefaultAsync();

                    cc.productId = item.productId;
                    cc.ProductName = eq.ProductDetail.Product.ProductName;
                    cc.SizeId = item.SizeId;
                    cc.SizeName = eq.ProductDetail.Size.SizeName;
                    cc.ColourId = item.ColourId;
                    cc.ColourName = eq.ProductDetail.Colour.ColourName;
                    cc.RealQuantity = item.RealQuantity;
                    cc.EndingQuantityInSystem = eq.Quantity;

                    map.Add(eq.ProductDetailId, eq.Quantity);
                    response.ListProducts.Add(cc);
                }

                foreach (var order in orders)
                {
                    var odts = await _orderDetailRepository.FindByAsync(o => o.OrderId == order.OrderId);
                    foreach (var odt in odts)
                    {
                        map[odt.ProductDetailId] += odt.Quantity;
                    }
                }

                foreach (var deliverynote in deliverynotes)
                {
                    var dnds = await _deliveryNoteDetailRepository.FindByAsync(d => d.DeliveryNoteId == deliverynote.DeliveryNoteId);
                    foreach (var dnd in dnds)
                    {
                        if(deliverynote.FromStore == storeId)
                        {
                            map[dnd.ProductDetailId] += dnd.Quantity;
                        }
                        else if(deliverynote.ToStore == storeId)
                        {
                            map[dnd.ProductDetailId] -= dnd.Quantity;
                        }
                    }
                }

                foreach (var item in map)
                {
                    var pd = await _productDetailRepository.FindAsync(p => p.ProductDetailId == item.Key);
                    response.ListProducts.Find(p => p.productId == pd.ProductId && p.SizeId == pd.SizeId && p.ColourId == pd.ColourId).BeginningQuantity = item.Value;
                }

                for (int i = 0; i < response.ListProducts.Count; i++)
                {
                    response.ListProducts[i].DifferenceQuantity = response.ListProducts[i].RealQuantity - response.ListProducts[i].EndingQuantityInSystem;
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

        public async Task<Result<bool>> QuantityAdjustment(string token, List<QuantityAdjustmentRequest> request)
        {
            var result = new Result<bool>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository.GetAllByIQueryable()
                    .Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();

                foreach (var item in request)
                {
                    var tw = await _genericRepository
                        .GetAllByIQueryable()
                        .Include(t => t.ProductDetail)
                        .Where(t => t.ProductDetail.ProductId == item.ProductId
                                && t.ProductDetail.ColourId == item.ColourId
                                && t.ProductDetail.SizeId == item.SizeId)
                        .FirstOrDefaultAsync();

                    tw.Quantity += item.DifferenceQuantity;
                    await _genericRepository.UpdateAsync(tw);
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
