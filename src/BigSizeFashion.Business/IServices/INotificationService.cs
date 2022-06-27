using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationResponse>> GetAllNotifications(string v, QueryStringParameters param);
        Task<Result<NotificationResponse>> GetNotificationByID(int id);
        Task<Result<bool>> DeleteNotification(int id);
        Task CreateNotification(CreateNotificationRequest request);
    }
}
