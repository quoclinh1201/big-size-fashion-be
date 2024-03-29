﻿using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.RequestObjects;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IPromotionDetailService
    {
        Task<Result<IEnumerable<ErrorProductWhenAddToPromotionResponse>>> AddProductsIntoPromotion(AddProductsIntoPromotionRequest request);
        Task<Result<bool>> RemoveProductOutOfPromotion(RemoveProductOutOfPromotionnRequest request);
        Task<PagedResult<GetListProductResponse>> GetListProductInPromotion(GetListProductInPromotionParameter param);
    }
}
