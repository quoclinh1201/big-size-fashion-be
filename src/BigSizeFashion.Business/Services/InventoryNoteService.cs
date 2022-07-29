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
                    var product = new InventoryNoteDetail
                    {
                        InventoryNoteId = note.InventoryNoteId,
                        ProductDetailId = item.ProductDetailId,
                        BeginningQuantity = nearestNote.Count > 0 ? (int)_inventoryNoteDetailRepository.GetAllByIQueryable().Where(n => n.InventoryNoteId == nearestNote[0].InventoryNoteId).Select(n => n.EndingQuantityAfterAdjusted).FirstOrDefault() : 0,
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
    }
}
