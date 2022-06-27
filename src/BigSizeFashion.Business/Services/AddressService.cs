using AutoMapper;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
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
    public class AddressService : IAddressService
    {
        private readonly IGenericRepository<Address> _genericRepository;
        private readonly IMapper _mapper;

        public AddressService(IGenericRepository<Address> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Result<AddressResponse>> CreateAddress(string token, AddressRequest request)
        {
            var result = new Result<AddressResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if(uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }

                var address = _mapper.Map<Address>(request);
                address.CustomerId = uid;
                await _genericRepository.InsertAsync(address);
                await _genericRepository.SaveAsync();
                result.Content = _mapper.Map<AddressResponse>(address);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeleteAddress(string token, int id)
        {
            var result = new Result<bool>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }

                var address = await _genericRepository.FindAsync(a => a.AddressId == id && a.CustomerId == uid);
                address.Status = false;
                await _genericRepository.UpdateAsync(address);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<AddressResponse>> GetAddressById(string token, int id)
        {
            var result = new Result<AddressResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var address = await _genericRepository.FindAsync(a => a.CustomerId == uid && a.AddressId == id && a.Status == true);
                result.Content = _mapper.Map<AddressResponse>(address);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<AddressResponse>>> GetAllAddresses(string token)
        {
            var result = new Result<IEnumerable<AddressResponse>>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var addresses = await _genericRepository.FindByAsync(a => a.CustomerId == uid && a.Status == true);
                result.Content = _mapper.Map<List<AddressResponse>>(addresses);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<AddressResponse>> UpdateAddress(string token, AddressRequest request, int id)
        {
            var result = new Result<AddressResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var address = await _genericRepository.FindAsync(a => a.AddressId == id && a.CustomerId == uid);
                address = _mapper.Map<Address>(request);
                await _genericRepository.UpdateAsync(address);
                result.Content = _mapper.Map<AddressResponse>(address);
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
