using AutoMapper;
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
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _genericRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IMapper _mapper;

        public UserService(IGenericRepository<User> genericRepository,
            IGenericRepository<Account> accountRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserProfileResponse>> GetOwnProfile(string token)
        {
            var result = new Result<UserProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var user = await _genericRepository.FindAsync(s => s.Uid == uid && s.Status == true);

                if (user is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }
                var acc = await _accountRepository.GetAllByIQueryable().Include(a => a.Role).Where(a => a.Uid == uid).FirstOrDefaultAsync();
                var response = _mapper.Map<UserProfileResponse>(user);
                response.Role = acc.Role.RoleName;
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<UserProfileResponse>> UpdateProfile(string token, UpdateUserProfileRequest request)
        {
            var result = new Result<UserProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var user = await _genericRepository.FindAsync(s => s.Uid == uid && s.Status == true);

                if (user is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var model = _mapper.Map(request, user);
                await _genericRepository.UpdateAsync(model);
                var acc = await _accountRepository.GetAllByIQueryable().Include(a => a.Role).Where(a => a.Uid == uid).FirstOrDefaultAsync();
                var response = _mapper.Map<UserProfileResponse>(user);
                response.Role = acc.Role.RoleName;

                result.Content = response;
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
