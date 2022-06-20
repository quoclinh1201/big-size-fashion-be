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
    public class StaffService : IStaffService
    {
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IMapper _mapper;

        public StaffService(
            IGenericRepository<staff> staffRepository,
            IGenericRepository<Account> accountRepository,
            IMapper mapper)
        {
            _staffRepository = staffRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Result<StaffProfileResponse>> GetOwnProfile(string token)
        {
            var result = new Result<StaffProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var staff = await _staffRepository.FindAsync(s => s.Uid == uid && s.Status == true);

                if (staff is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var response = _mapper.Map<StaffProfileResponse>(staff);
                var account = await _accountRepository.GetAllByIQueryable().Include(a => a.Role).Where(s => s.Uid == uid).FirstOrDefaultAsync();
                response.Role = account.Role.RoleName;
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<StaffProfileResponse>> UpdateProfile(string token, UpdateStaffProfileRequest request)
        {
            var result = new Result<StaffProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var staff = await _staffRepository.GetAllByIQueryable().Include(s => s.Store).Where(s => s.Uid == uid && s.Status == true).FirstOrDefaultAsync();

                if (staff is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var model = _mapper.Map(request, staff);
                await _staffRepository.UpdateAsync(model);

                var account = await _accountRepository.GetAllByIQueryable().Include(a => a.Role).Where(s => s.Uid == uid).FirstOrDefaultAsync();
                var response = _mapper.Map<StaffProfileResponse>(model);
                response.Role = account.Role.RoleName;

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
