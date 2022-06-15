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
    public class OwnerService : IOwnerService
    {
        private readonly IGenericRepository<Owner> _genericRepository;
        private readonly IMapper _mapper;

        public OwnerService(IGenericRepository<Owner> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Result<OwnerProfileResponse>> GetOwnProfile(string token)
        {
            var result = new Result<OwnerProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var owner = await _genericRepository.FindAsync(s => s.Uid == uid && s.Status == true);

                if (owner is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var response = _mapper.Map<OwnerProfileResponse>(owner);
                response.Role = AccountRoleEnum.Owner.ToString();
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<OwnerProfileResponse>> UpdateProfile(string token, UpdateOwnerProfileRequest request)
        {
            var result = new Result<OwnerProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var owner = await _genericRepository.FindAsync(s => s.Uid == uid && s.Status == true);

                if (owner is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var model = _mapper.Map(request, owner);
                await _genericRepository.UpdateAsync(model);
                var response = _mapper.Map<OwnerProfileResponse>(model);
                response.Role = AccountRoleEnum.Owner.ToString();

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
