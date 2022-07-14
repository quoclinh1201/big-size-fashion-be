using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.RequestObjects;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.RequestObjects;
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
    public class PromotionDetailService : IPromotionDetailService
    {
        private readonly IGenericRepository<PromotionDetail> _genericRepository;
        private readonly IGenericRepository<Promotion> _promotionRepository;
        private readonly IGenericRepository<ProductImage> _imageRepository;
        private readonly IMapper _mapper;

        public PromotionDetailService(
            IGenericRepository<PromotionDetail> genericRepository,
            IGenericRepository<Promotion> promotionRepository,
            IGenericRepository<ProductImage> imageRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _promotionRepository = promotionRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<ErrorProductWhenAddToPromotionResponse>>> AddProductsIntoPromotion(AddProductsIntoPromotionRequest request)
        {
            var result = new Result<IEnumerable<ErrorProductWhenAddToPromotionResponse>>();
            var list = new List<ErrorProductWhenAddToPromotionResponse>();
            try
            {
                if(request.ListProductId.Count > 0)
                {
                    var error = string.Empty;
                    var promoton = await _promotionRepository.FindAsync(p => p.PromotionId == request.PromotionId && p.Status == true);

                    if(promoton == null)
                    {
                        result.Content = list;
                        return result;
                    }
                    else
                    {
                        var applyDate = promoton.ApplyDate;
                        var expiredDate = promoton.ExpiredDate;

                        var promotions = await _promotionRepository.FindByAsync(
                            d => (d.ApplyDate <= applyDate && applyDate < d.ExpiredDate)
                            || (expiredDate > d.ApplyDate && expiredDate <= d.ExpiredDate)
                            || (applyDate <= d.ApplyDate && expiredDate >= d.ExpiredDate)
                            && d.Status == true
                        );

                        //check existed product in other promotion at same time
                        if(promotions.Count > 0)
                        {
                            foreach (var promotion in promotions)
                            {
                                foreach (var productId in request.ListProductId)
                                {
                                    var pd = await _genericRepository.GetAllByIQueryable().Where(p => p.PromotionId == promotion.PromotionId && p.ProductId == productId).Include(p => p.Product).FirstOrDefaultAsync();
                                    if(pd != null)
                                    {
                                        var errorProduct = new ErrorProductWhenAddToPromotionResponse
                                        {
                                            ProductId = productId,
                                            ProductName = pd.Product.ProductName,
                                            PromotionId = promotion.PromotionId,
                                            PromotionName = promotion.PromotionName,
                                            ApplyDate = ConvertDateTime.ConvertDateToString(promotion.ApplyDate),
                                            ExpiredDate = ConvertDateTime.ConvertDateToString(promotion.ExpiredDate)
                                        };
                                        list.Add(errorProduct);
                                    }
                                }
                            }
                            if(list.Count > 0)
                            {
                                result.Content = list;
                                return result;
                            }
                        }

                        foreach (var productId in request.ListProductId)
                        {
                            var pd = new PromotionDetail { ProductId = productId, PromotionId = request.PromotionId };
                            await _genericRepository.InsertAsync(pd);
                        }
                        await _genericRepository.SaveAsync();
                        result.Content = list;
                        return result;
                    }  
                }
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, "Danh sách sản phẩm bị rỗng.");
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<PagedResult<GetListProductResponse>> GetListProductInPromotion(GetListProductInPromotionParameter param)
        {
            try
            {
                var response = new List<GetListProductResponse>();
                var promotionDetails = await _genericRepository.GetAllByIQueryable()
                                    .Include(p => p.Product)
                                    .Include(p => p.Promotion)
                                    .Where(p => p.PromotionId == param.PromotionId)
                                    .ToListAsync();

                if(promotionDetails.Count > 0)
                {
                    var query = promotionDetails.AsQueryable();
                    FilterProductByName(ref query, param.ProductName);
                    response = _mapper.Map<List<GetListProductResponse>>(query.ToList());

                    for (int i = 0; i < response.Count; i++)
                    {
                        var image = await _imageRepository.FindAsync(x => x.ProductId == response[i].ProductId && x.IsMainImage == true);
                        if (image != null)
                        {
                            response[i].ImageUrl = image.ImageUrl;
                        }
                        else
                        {
                            response[i].ImageUrl = CommonConstants.NoImageUrl;
                        }

                        var unroundPrice = ((decimal)(100 - promotionDetails[0].Promotion.PromotionValue) / 100) * response[i].Price;
                        response[i].PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
                        response[i].PromotionValue = promotionDetails[0].Promotion.PromotionValue + "%";
                    }
                }
                return PagedResult<GetListProductResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void FilterProductByName(ref IQueryable<PromotionDetail> query, string productName)
        {
            if (!query.Any() || String.IsNullOrEmpty(productName) || String.IsNullOrWhiteSpace(productName))
            {
                return;
            }

            query = query.Where(q => q.Product.ProductName.ToLower().Contains(productName.ToLower()));
        }

        public async Task<Result<bool>> RemoveProductOutOfPromotion(RemoveProductOutOfPromotionnRequest request)
        {
            var result = new Result<bool>();
            try
            {
                await _genericRepository.DeleteSpecificFieldByAsync(p => p.PromotionId == request.PromotionId && p.ProductId == request.ProductId);
                await _genericRepository.SaveAsync();
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
