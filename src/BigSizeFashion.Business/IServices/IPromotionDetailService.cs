using BigSizeFashion.Business.Dtos.RequestObjects;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.RequestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IPromotionDetailService
    {
        Task<Result<bool>> AddProductsIntoPromotion(AddProductsIntoPromotionRequest request);
        Task<Result<bool>> RemoveProductOutOfPromotion(RemoveProductOutOfPromotionnRequest request);
    }
}
