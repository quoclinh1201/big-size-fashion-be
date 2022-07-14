using AutoMapper;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Parameters;
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
    public class NotificationService : INotificationService
    {
        private readonly IGenericRepository<Notification> _genericRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IMapper _mapper;

        public NotificationService(
            IGenericRepository<Notification> genericRepository,
            IGenericRepository<Account> accountRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task CreateNotification(CreateNotificationRequest request)
        {
            try
            {
                var notify = _mapper.Map<Notification>(request);
                var acc = await _accountRepository.FindAsync(a => a.Username == request.Username);
                notify.AccountId = acc.Uid;
                await _genericRepository.InsertAsync(notify);
                await _genericRepository.SaveAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<bool>> DeleteNotification(int id)
        {
            var result = new Result<bool>();
            try
            {
                var notify = await _genericRepository.FindAsync(s => s.NotificationId == id);

                if (notify is null)
                {
                    result.Content = false;
                    return result;
                }

                notify.Status = false;
                await _genericRepository.UpdateAsync(notify);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<PagedResult<NotificationResponse>> GetAllNotifications(string token, QueryStringParameters param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var notifications = await _genericRepository.FindByAsync(n => n.AccountId == uid && n.Status == true);
                var query = notifications.AsQueryable();
                SortNotification(ref query);
                var response = _mapper.Map<List<NotificationResponse>>(query.ToList());
                return PagedResult<NotificationResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<NotificationResponse>> GetNotificationByID(int id)
        {
            var result = new Result<NotificationResponse>();
            try
            {
                var notify = await _genericRepository.FindAsync(s => s.NotificationId == id && s.Status == true);
                result.Content = _mapper.Map<NotificationResponse>(notify);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private void SortNotification(ref IQueryable<Notification> query)
        {
            if (!query.Any())
            {
                return;
            }
            query = query.OrderByDescending(x => x.CreateDate);
        }
    }
}
