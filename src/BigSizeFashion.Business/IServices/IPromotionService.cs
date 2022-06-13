using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IPromotionService
    {
        Task<PagedResult<PromotionResponse>> GetAllPromotion(SearchPromotionParameter param);
        Task<Result<PromotionResponse>> GetPromotionByID(int id);
        Task<Result<PromotionResponse>> CreatePromotion(PromotionRequest request);
        Task<Result<bool>> ValidateTimeOfPromotion(ValidateTimeOfPromotionRequest request);
        Task<Result<PromotionResponse>> UpdatePromotion(int id, PromotionRequest request);
        Task<Result<bool>> DeletePromotion(int id);
    }
}
