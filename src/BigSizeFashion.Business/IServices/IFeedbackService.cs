using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IFeedbackService
    {
        Task<Result<double?>> GetRatingOfProduct(int id);
        Task<PagedResult<FeedbackResponse>> GetListFeedbackOfProduct(int id, FeeadbackParameter param);
        Task<Result<FeedbackResponse>> CreateFeedback(string v, FeedbackRequest request);
        Task<Result<FeedbackResponse>> UpdateFeedback(string v, FeedbackRequest request, int id);
        Task<Result<bool>> DeleteFeedback(int id);
    }
}
