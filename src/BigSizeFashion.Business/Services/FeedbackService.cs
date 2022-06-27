using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Enums;
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
    public class FeedbackService : IFeedbackService
    {
        private readonly IGenericRepository<Feedback> _genericRepository;
        private readonly IMapper _mapper;

        public FeedbackService(IGenericRepository<Feedback> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<FeedbackResponse>> GetListFeedbackOfProduct(int id, FeeadbackParameter param)
        {
            try
            {
                var feedbacks = await _genericRepository.GetAllByIQueryable()
                    .Where(f => f.ProductId == id && f.Status == true)
                    .Include(f => f.Customer).ToListAsync();
                var query = feedbacks.AsQueryable();
                SortBy(ref query, param);
                var response = _mapper.Map<List<FeedbackResponse>>(query.ToList());
                return PagedResult<FeedbackResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SortBy(ref IQueryable<Feedback> query, FeeadbackParameter param)
        {
            if (!query.Any())
            {
                return;
            }

            if(param.SortBy == null || param.SortBy == FeedbackSortByEnum.Rating)
            {
                if(param.IsDescendingOrder == null || param.IsDescendingOrder == true)
                {
                    query = query.OrderByDescending(q => q.Rate);
                }
                else
                {
                    query = query.OrderBy(q => q.CreateDate);
                }
            }
            else
            {
                if (param.IsDescendingOrder == null || param.IsDescendingOrder == true)
                {
                    query = query.OrderByDescending(q => q.CreateDate);
                }
                else
                {
                    query = query.OrderBy(q => q.CreateDate);
                }
            }
        }

        public async Task<Result<double?>> GetRatingOfProduct(int id)
        {
            var result = new Result<double?>();
            try
            {
                var listRatings = await _genericRepository.GetAllByIQueryable().Where(s => s.ProductId == id).Select(s => s.Rate).ToListAsync();

                if (listRatings is null)
                {
                    result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.NotExistedProduct);
                    return result;
                }
                double? avg = Math.Round((double)listRatings.Average(x => x), 1);
                result.Content = avg;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<FeedbackResponse>> CreateFeedback(string v, FeedbackRequest request)
        {
            var result = new Result<FeedbackResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(v);
                var feedback = _mapper.Map<Feedback>(request);
                feedback.CustomerId = uid;

                await _genericRepository.InsertAsync(feedback);
                await _genericRepository.SaveAsync();
                result.Content = _mapper.Map<FeedbackResponse>(feedback);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<FeedbackResponse>> UpdateFeedback(string v, FeedbackRequest request, int id)
        {
            var result = new Result<FeedbackResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(v);
                var feedback = await _genericRepository.FindAsync(f => f.FeedbackId == id);
                feedback = _mapper.Map<Feedback>(request);
                feedback.CustomerId = uid;

                await _genericRepository.UpdateAsync(feedback);
                result.Content = _mapper.Map<FeedbackResponse>(feedback);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeleteFeedback(int id)
        {
            var result = new Result<bool>();
            try
            {
                var feedback = await _genericRepository.FindAsync(f => f.FeedbackId == id);
                feedback.Status = false;

                await _genericRepository.UpdateAsync(feedback);
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
