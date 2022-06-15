using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Enums;
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
    public class AdminService : IAdminService
    {
        private readonly IGenericRepository<Admin> _genericRepository;
        private readonly IMapper _mapper;

        public AdminService(IGenericRepository<Admin> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Result<AdminProfileResponse>> GetOwnProfile(string token)
        {
            var result = new Result<AdminProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var admin = await _genericRepository.FindAsync(s => s.Uid == uid && s.Status == true);

                if (admin is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var response = _mapper.Map<AdminProfileResponse>(admin);
                response.Role = AccountRoleEnum.Admin.ToString();
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<AdminProfileResponse>> UpdateProfile(string token, UpdateAdminProfileRequest request)
        {
            var result = new Result<AdminProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var admin = await _genericRepository.FindAsync(s => s.Uid == uid && s.Status == true);

                if (admin is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var model = _mapper.Map(request, admin);
                await _genericRepository.UpdateAsync(model);
                var response = _mapper.Map<AdminProfileResponse>(model);
                response.Role = AccountRoleEnum.Admin.ToString();

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
