using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
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
    public class PromotionService : IPromotionService
    {
        private readonly IGenericRepository<Promotion> _genericRepository;
        private readonly IMapper _mapper;

        public PromotionService(IGenericRepository<Promotion> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PromotionResponse>> GetAllPromotion(SearchPromotionParameter param)
        {
            try
            {
                var promotions = await _genericRepository.GetAllAsync();
                var query = promotions.AsQueryable();
                FilterPromotionByName(ref query, param.PromotionName);
                OrderByCreatedDate(ref query, param.OrderByApplyDate);
                var response = _mapper.Map<List<PromotionResponse>>(query.ToList());
                return PagedResult<PromotionResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void FilterPromotionByName(ref IQueryable<Promotion> query, string name)
        {
            if (!query.Any() || String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            query = query.Where(q => q.PromotionName.ToLower().Contains(name.ToLower()));
        }

        private void OrderByCreatedDate(ref IQueryable<Promotion> query, bool? orderByApplydDate)
        {
            if (!query.Any() || orderByApplydDate is null)
            {
                return;
            }

            if (orderByApplydDate is true)
            {
                query = query.OrderByDescending(x => x.ApplyDate);
            }
            else
            {
                query = query.OrderBy(x => x.ApplyDate);
            }
        }

        public async Task<Result<PromotionResponse>> GetPromotionByID(int id)
        {
            var result = new Result<PromotionResponse>();
            try
            {
                var promotion = await _genericRepository.FindAsync(s => s.PromotionId == id && s.Status == true);
                result.Content = _mapper.Map<PromotionResponse>(promotion);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<PromotionResponse>> CreatePromotion(PromotionRequest request)
        {
            var result = new Result<PromotionResponse>();
            try
            {
                var promotion = _mapper.Map<Promotion>(request);
                await _genericRepository.InsertAsync(promotion);
                await _genericRepository.SaveAsync();
                result.Content = _mapper.Map<PromotionResponse>(promotion);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        //public async Task<Result<bool>> ValidateTimeOfPromotion(ValidateTimeOfPromotionRequest request)
        //{
        //    var result = new Result<bool>();
        //    try
        //    {
        //        var now = DateTime.UtcNow.AddHours(7);
        //        var applyDate = ConvertDateTime.ConvertStringToDate(request.ApplyDate);
        //        var expiredDate = ConvertDateTime.ConvertStringToDate(request.ExpiredDate);
                
        //        if(expiredDate < applyDate)
        //        {
        //            result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.ExpireDateLessThanApplyDate);
        //            return result;
        //        }

        //        if(now > applyDate)
        //        {
        //            result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.ApplyDateLessThanNow);
        //            return result;
        //        }

        //        var promotions = await _genericRepository.FindByAsync(
        //                d => (d.ApplyDate <= applyDate && applyDate < d.ExpiredDate)
        //                || (expiredDate > d.ApplyDate && expiredDate <= d.ExpiredDate)
        //                || (applyDate <= d.ApplyDate && expiredDate >= d.ExpiredDate)
        //                && d.Status == true
        //            );
        //        if(promotions.Count > 0)
        //        {
        //            result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.InvalidPromotionDatetime);
        //            return result;
        //        }

        //        result.Content = true;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        public async Task<Result<PromotionResponse>> UpdatePromotion(int id, PromotionRequest request)
        {
            var result = new Result<PromotionResponse>();
            try
            {
                var promotion = await _genericRepository.FindAsync(s => s.PromotionId == id);
                var model = _mapper.Map(request, promotion);
                await _genericRepository.UpdateAsync(model);
                result.Content = _mapper.Map<PromotionResponse>(model);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeletePromotion(int id)
        {
            var result = new Result<bool>();
            try
            {
                var promotion = await _genericRepository.FindAsync(s => s.PromotionId == id);

                if (promotion is null)
                {
                    result.Content = false;
                    return result;
                }

                promotion.Status = false;
                await _genericRepository.UpdateAsync(promotion);
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
