using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
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
    public class CustomersService : ICustomersService
    {
        private readonly IGenericRepository<Customer> _genericRepository;
        private readonly IMapper _mapper;

        public CustomersService(IGenericRepository<Customer> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Result<bool>> ChangePINCode(string token, ChangePINCodeRequest request)
        {
            try
            {
                var result = new Result<bool>();
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.PinCode.Equals(request.OldPinCode) && c.Status == true);

                if (customer is null)
                {
                    result.Content = false;
                    return result;
                }

                customer.PinCode = request.ConfirmNewPinCode;
                await _genericRepository.UpdateAsync(customer);
                result.Content = true;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<bool>> CheckPINCode(string token)
        {
            try
            {
                var result = new Result<bool>();
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.PinCode != null && c.Status == true);

                if (customer is null)
                {
                    result.Content = false;
                    return result;
                }

                result.Content = true;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<bool>> CreatePINCode(string token, CreatePINCodeRequest request)
        {
            try
            {
                var result = new Result<bool>();
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.PinCode == null && c.Status == true);

                if(customer is null)
                {
                    result.Content = false;
                    return result;
                }

                customer.PinCode = request.PinCode;
                await _genericRepository.UpdateAsync(customer);
                result.Content = true;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<CustomerProfileResponse>> GetOwnProfile(string token)
        {
            try
            {
                var result = new Result<CustomerProfileResponse>();
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.Status == true);
                var response = _mapper.Map<CustomerProfileResponse>(customer);
                result.Content = response;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<CustomerProfileResponse>> UpdateProfile(string token, UpdateCustomerProfileRequest request)
        {
            try
            {
                var result = new Result<CustomerProfileResponse>();
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _genericRepository.FindAsync(c => c.Uid == uid && c.Status == true);

                if (customer == null)
                    return null;

                var model = _mapper.Map(request, customer);
                await _genericRepository.UpdateAsync(model);
                result.Content = _mapper.Map<CustomerProfileResponse>(model);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<bool>> ValidatePINCode(string token, ValidatePINCodeRequest request)
        {
            try
            {
                var result = new Result<bool>();
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.PinCode.Equals(request.PinCode) && c.Status == true);

                if (customer is null)
                {
                    result.Content = false;
                    return result;
                }

                result.Content = true;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
