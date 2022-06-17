using BigSizeFashion.Business.Dtos.RequestObjects;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.RequestObjects;
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
    public class PromotionDetailService : IPromotionDetailService
    {
        private readonly IGenericRepository<PromotionDetail> _genericRepository;
        private readonly IGenericRepository<Promotion> _promotionRepository;

        public PromotionDetailService(IGenericRepository<PromotionDetail> genericRepository, IGenericRepository<Promotion> promotionRepository)
        {
            _genericRepository = genericRepository;
            _promotionRepository = promotionRepository;
        }

        public async Task<Result<bool>> AddProductsIntoPromotion(AddProductsIntoPromotionRequest request)
        {
            var result = new Result<bool>();
            try
            {
                if(request.ListProductId.Count > 0)
                {
                    var error = string.Empty;
                    var promoton = await _promotionRepository.FindAsync(p => p.PromotionId == request.PromotionId && p.Status == true);

                    if(promoton == null)
                    {
                        result.Content = false;
                        return result;
                    }
                    else
                    {
                        //var applyDate = promoton.ApplyDate;
                        //var expiredDate = promoton.ExpiredDate;

                        //var promotions = await _promotionRepository.FindByAsync(
                        //    d => (d.ApplyDate <= applyDate && applyDate < d.ExpiredDate)
                        //    || (expiredDate > d.ApplyDate && expiredDate <= d.ExpiredDate)
                        //    || (applyDate <= d.ApplyDate && expiredDate >= d.ExpiredDate)
                        //    && d.Status == true
                        //);

                        foreach (var productId in request.ListProductId)
                        {
                            var pd = new PromotionDetail { ProductId = productId, PromotionId = request.PromotionId };
                            await _genericRepository.InsertAsync(pd);
                        }
                        await _genericRepository.SaveAsync();
                        result.Content = true;
                        return result;
                    }  
                }
                result.Content = false;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
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
