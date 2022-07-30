using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services
{
    public class InventoryNoteService : IInventoryNoteService
    {
        private readonly IGenericRepository<InventoryNote> _inventoryNoteRepository;
        private readonly IGenericRepository<InventoryNoteDetail> _inventoryNoteDetailRepository;
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<ProductDetail> _productDetailRepository;
        private readonly IGenericRepository<StoreWarehouse> _storeWarehouseRepository;
        private readonly IStoreService _storeService;
        private readonly IMapper _mapper;

        public InventoryNoteService(
            IGenericRepository<InventoryNote> inventoryNoteRepository,
            IGenericRepository<InventoryNoteDetail> inventoryNoteDetailRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<ProductDetail> productDetailRepository,
            IGenericRepository<StoreWarehouse> storeWarehouseRepository,
            IStoreService storeService,
            IMapper mapper)
        {
            _inventoryNoteRepository = inventoryNoteRepository;
            _inventoryNoteDetailRepository = inventoryNoteDetailRepository;
            _staffRepository = staffRepository;
            _productDetailRepository = productDetailRepository;
            _storeWarehouseRepository = storeWarehouseRepository;
            _storeService = storeService;
            _mapper = mapper;
        }

        public async Task<Result<InventoryNoteResponse>> CreateInventoryNote(string token, CreateInventoryNoteRequest request)
        {
            var result = new Result<InventoryNoteResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var staff = await _staffRepository.FindAsync(s => s.Uid == uid);
                var note = _mapper.Map<InventoryNote>(request);
                note.StaffId = staff.Uid;
                note.StoreId = staff.StoreId;
                await _inventoryNoteRepository.InsertAsync(note);
                await _inventoryNoteRepository.SaveAsync();

                var nearestNote = await _inventoryNoteRepository.GetAllByIQueryable()
                    .Where(n => n.StoreId == note.StoreId && n.ToDate.Date <= note.FromDate && n.Status == true)
                    .OrderByDescending(n => n.ToDate).Take(1).Skip(0).ToListAsync();
                var allproduct = await _storeWarehouseRepository.FindByAsync(p => p.StoreId == note.StoreId);
                foreach (var item in allproduct)
                {
                    var q = 0;
                    if(nearestNote.Count > 0)
                    {
                        var ccc = _inventoryNoteDetailRepository.GetAllByIQueryable().Where(n => n.InventoryNoteId == nearestNote[0].InventoryNoteId).FirstOrDefault();
                        if (ccc.EndingQuantityAfterAdjusted != null)
                        {
                            q = (int)ccc.EndingQuantityAfterAdjusted;
                        }
                        else
                        {
                            q = ccc.EndingQuantity;
                        }
                    }

                    var product = new InventoryNoteDetail
                    {
                        InventoryNoteId = note.InventoryNoteId,
                        ProductDetailId = item.ProductDetailId,
                        BeginningQuantity = q,
                        EndingQuantity = item.Quantity,
                        EndingQuantityAfterAdjusted = null
                    };
                    await _inventoryNoteDetailRepository.InsertAsync(product);
                    await _inventoryNoteDetailRepository.SaveAsync();
                }
                return await GetInventoryNoteById(note.InventoryNoteId);
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeleteInventoryNote(int id)
        {
            var result = new Result<bool>();
            try
            {
                var note = await _inventoryNoteRepository.FindAsync(n => n.InventoryNoteId == id);
                if(note.Status == true)
                {
                    note.Status = false;
                }
                else
                {
                    note.Status = true;
                }
                await _inventoryNoteRepository.UpdateAsync(note);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
                throw;
            }
        }

        public async Task<Result<Stream>> ExportExcel(int id)
        {
            var result = new Result<Stream>();
            try
            {
                result.Content = await CreateExcelFile(id);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private async Task<Stream> CreateExcelFile(int id, Stream stream = null)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
                {

                    excelPackage.Workbook.Worksheets.Add("Phiếu kiểm kê #" + id);
                    var workSheet = excelPackage.Workbook.Worksheets.First();
                    await BindingFormatForExcel(workSheet, id);
                    excelPackage.Save();
                    return excelPackage.Stream;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task BindingFormatForExcel(ExcelWorksheet worksheet, int id)
        {
            try
            {
                var note = GetInventoryNoteById(id).Result.Content;

                worksheet.DefaultColWidth = 10;
                worksheet.Cells.Style.WrapText = true;
                worksheet.Column(1).Width = 11;
                worksheet.Column(5).Width = 20;

                worksheet.Cells["A1:K1"].Value = "PHIẾU KIỂM KÊ HÀNG HÓA";
                worksheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:K1"].Style.Font.Bold = true;
                worksheet.Cells["A1:K1"].Merge = true;

                worksheet.Cells["A3"].Value = "Từ ngày:";
                worksheet.Cells["A3"].Style.Font.Bold = true;
                worksheet.Cells["B3:C3"].Value = note.FromDate;
                worksheet.Cells["B3:C3"].Merge = true;

                worksheet.Cells["A4"].Value = "Đến ngày:";
                worksheet.Cells["A4"].Style.Font.Bold = true;
                worksheet.Cells["B4:C4"].Value = note.ToDate;
                worksheet.Cells["B4:C4"].Merge = true;

                worksheet.Cells["A5"].Value = "Nhân viên:";
                worksheet.Cells["A5"].Style.Font.Bold = true;
                worksheet.Cells["B5:D5"].Value = note.StaffName;
                worksheet.Cells["B5:D5"].Merge = true;

                worksheet.Cells["A6"].Value = "Chi nhánh:";
                worksheet.Cells["A6"].Style.Font.Bold = true;
                worksheet.Cells["B6:K6"].Value = note.Store.StoreAddress;
                worksheet.Cells["B6:K6"].Merge = true;

                worksheet.Cells["A8:A9"].Value = "STT";
                worksheet.Cells["A8:A9"].Style.Font.Bold = true;
                worksheet.Cells["A8:A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A8:A9"].Merge = true;
                worksheet.Cells["A8:A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells["B8:E9"].Value = "Sản phẩm";
                worksheet.Cells["B8:E9"].Style.Font.Bold = true;
                worksheet.Cells["B8:E9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B8:E9"].Merge = true;
                worksheet.Cells["B8:E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells["F8:I8"].Value = "Số lượng trên hệ thống";
                worksheet.Cells["F8:I8"].Style.Font.Bold = true;
                worksheet.Cells["F8:I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F8:I8"].Merge = true;
                worksheet.Cells["F8:I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells["F9:G9"].Value = "Đầu kỳ";
                worksheet.Cells["F9:G9"].Style.Font.Bold = true;
                worksheet.Cells["F9:G9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F9:G9"].Merge = true;
                worksheet.Cells["F9:G9"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells["H9:I9"].Value = "Cuối kỳ";
                worksheet.Cells["H9:I9"].Style.Font.Bold = true;
                worksheet.Cells["H9:I9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["H9:I9"].Merge = true;
                worksheet.Cells["H9:I9"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells["J8:K9"].Value = "Số lượng cuối kỳ thực tế";
                worksheet.Cells["J8:K9"].Style.Font.Bold = true;
                worksheet.Cells["J8:K9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["J8:K9"].Merge = true;
                worksheet.Cells["J8:K9"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                var index = 10;
                for (int i = 0; i < note.InventoryNoteDetail.Count; i++)
                {
                    worksheet.Cells["A" + index].Value = i + 1;
                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells["B" + index + ":E" + index].Value = note.InventoryNoteDetail[i].ProductName + "-Màu " + note.InventoryNoteDetail[i].Colour + "-Size " + note.InventoryNoteDetail[i].Size;
                    worksheet.Cells["B" + index + ":E" + index].Merge = true;
                    worksheet.Cells["B" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["B" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells["F" + index + ":G" + index].Value = note.InventoryNoteDetail[i].BeginningQuantity;
                    worksheet.Cells["F" + index + ":G" + index].Merge = true;
                    worksheet.Cells["F" + index + ":G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["F" + index + ":G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells["H" + index + ":I" + index].Value = note.InventoryNoteDetail[i].EndingQuantity;
                    worksheet.Cells["H" + index + ":I" + index].Merge = true;
                    worksheet.Cells["H" + index + ":I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H" + index + ":I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    worksheet.Cells["J" + index + ":K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    index += 1;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<InventoryNoteResponse>> GetInventoryNoteById(int id)
        {
            var result = new Result<InventoryNoteResponse>();
            try
            {
                var note = await _inventoryNoteRepository
                    .GetAllByIQueryable()
                    .Include(n => n.Staff)
                    .Where(n => n.InventoryNoteId == id)
                    .FirstOrDefaultAsync();

                var response = new InventoryNoteResponse { 
                    InventoryNoteId = note.InventoryNoteId,
                    InventoryNoteName = note.InventoryNoteName,
                    FromDate = ConvertDateTime.ConvertDateToString(note.FromDate),
                    ToDate = ConvertDateTime.ConvertDateToString(note.ToDate),
                    AdjustedDate = ConvertDateTime.ConvertDateToString(note.AdjustedDate),
                    Status = note.Status,
                    StaffName = note.Staff.Fullname,
                    Store = _storeService.GetStoreByID(note.StoreId).Result.Content
                };

                var noteDetail = await _inventoryNoteDetailRepository
                    .GetAllByIQueryable()
                    .Include(n => n.ProductDetail)
                    .ThenInclude(n => n.Product)
                    .Include(n => n.ProductDetail)
                    .ThenInclude(n => n.Size)
                    .Include(n => n.ProductDetail)
                    .ThenInclude(n => n.Colour)
                    .Where(n => n.InventoryNoteId == note.InventoryNoteId)
                    .ToListAsync();

                foreach (var item in noteDetail)
                {
                    response.InventoryNoteDetail.Add(new InventoryNoteDetailResponse
                    {
                        ProductDetailId = item.ProductDetailId,
                        ProductName = item.ProductDetail.Product.ProductName,
                        Colour = item.ProductDetail.Colour.ColourName,
                        Size = item.ProductDetail.Size.SizeName,
                        BeginningQuantity = item.BeginningQuantity,
                        EndingQuantity = item.EndingQuantity,
                        EndingQuantityAfterAdjusted = item.EndingQuantityAfterAdjusted
                    });
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

        public async Task<PagedResult<GetListInventoryNoteResponse>> GetListInventoryNote(string token, GetListInventoryNoteParameter param)
        {
            var response = new List<GetListInventoryNoteResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var staff = await _staffRepository.FindAsync(s => s.Uid == uid);
                var notes = await _inventoryNoteRepository
                    .GetAllByIQueryable()
                    .Include(n => n.Staff)
                    .Where(n => n.StoreId == staff.StoreId)
                    .ToListAsync();
                var query = notes.AsQueryable();
                query = query.OrderByDescending(n => n.ToDate);
                FilterInventoryNoteByName(ref query, param.InventoryNoteName);
                response = _mapper.Map<List<GetListInventoryNoteResponse>>(query.ToList());
                return PagedResult<GetListInventoryNoteResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void FilterInventoryNoteByName(ref IQueryable<InventoryNote> query, string inventoryNoteName)
        {
            if (!query.Any() || String.IsNullOrEmpty(inventoryNoteName) || String.IsNullOrWhiteSpace(inventoryNoteName))
            {
                return;
            }

            query = query.Where(q => q.InventoryNoteName.ToLower().Contains(inventoryNoteName.ToLower()));
        }

        public async Task<Result<CheckWarehouseResponse>> CheckWarehouse(string v, CheckWarehouseRequest request)
        {
            var result = new Result<CheckWarehouseResponse>();
            var response = new CheckWarehouseResponse();
            try
            {
                if(request.ListProducts.Count > 1)
                {
                    if (request.ListProducts.Select(p => p.ProductDetailId).Count() != request.ListProducts.Select(p => p.ProductDetailId).Distinct().Count())
                    {
                        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, "Trùng sản phẩm!");
                        return result;
                    }
                }

                foreach (var item in request.ListProducts)
                {
                    var product = await _inventoryNoteDetailRepository
                        .GetAllByIQueryable()
                        .Include(n => n.ProductDetail)
                        .ThenInclude(n => n.Product)
                        .Include(n => n.ProductDetail)
                        .ThenInclude(n => n.Size)
                        .Include(n => n.ProductDetail)
                        .ThenInclude(n => n.Colour)
                        .Where(n => n.InventoryNoteId == request.InventoryNoteId)
                        .FirstOrDefaultAsync();
                    var cc = new Dtos.Responses.CheckWarehouseItem
                    {
                        BeginningQuantity = product.BeginningQuantity,
                        ColourId = product.ProductDetail.ColourId,
                        ColourName = product.ProductDetail.Colour.ColourName,
                        EndingQuantityInSystem = product.EndingQuantity,
                        productId = product.ProductDetail.ProductId,
                        ProductName = product.ProductDetail.Product.ProductName,
                        RealQuantity = item.RealQuantity,
                        SizeId = product.ProductDetail.Size.SizeId,
                        SizeName = product.ProductDetail.Size.SizeName,
                        DifferenceQuantity = item.RealQuantity - product.EndingQuantity
                    };
                    response.ListProducts.Add(cc);
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
                    var tw = await _storeWarehouseRepository.FindAsync(t => t.ProductDetailId == item.ProductDetailId && t.StoreId == storeId);

                    tw.Quantity += item.DifferenceQuantity;
                    await _storeWarehouseRepository.UpdateAsync(tw);

                    var note = await _inventoryNoteDetailRepository.FindAsync(t => t.ProductDetailId == item.ProductDetailId && t.InventoryNoteId == item.InventoryNoteId);
                    note.EndingQuantityAfterAdjusted = tw.Quantity;
                    await _inventoryNoteDetailRepository.UpdateAsync(note);
                }

                var n = await _inventoryNoteRepository.FindAsync(s => s.InventoryNoteId == request[0].InventoryNoteId);
                n.AdjustedDate = DateTime.Now;
                await _inventoryNoteRepository.UpdateAsync(n);

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
