using AutoMapper;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
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
    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository<Customer> _genericRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public CustomerService(IGenericRepository<Customer> genericRepository,
            IGenericRepository<Account> accountRepository,
            IGenericRepository<Role> roleRepository,
            IMapper mapper)
        {
            _genericRepository = genericRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<Result<CustomerProfileResponse>> AddNewCustomer(AddNewCustomerRequest request)
        {
            var result = new Result<CustomerProfileResponse>();
            try
            {
                var role = await _roleRepository.FindAsync(r => r.RoleName.Equals("Customer"));
                var account = new Account { Username = request.PhoneNumber, RoleId = role.RoleId, CreateAt = DateTime.UtcNow.AddHours(7), Status = true };
                await _accountRepository.InsertAsync(account);
                await _accountRepository.SaveAsync();

                var customer = _mapper.Map<Customer>(request);
                customer.Uid = account.Uid;
                await _genericRepository.InsertAsync(customer);
                await _genericRepository.SaveAsync();
                var response = _mapper.Map<CustomerProfileResponse>(customer);
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> ChangePINCode(string token, ChangePINCodeRequest request)
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
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.PinCode.Equals(request.OldPinCode) && c.Status == true);

                if (customer is null)
                {
                    result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.NotExistedPINCode);
                    return result;
                }

                customer.PinCode = request.ConfirmNewPinCode;
                await _genericRepository.UpdateAsync(customer);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> CheckPINCode(string token)
        {
            var result = new Result<bool>();
            try
            {
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
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> CreatePINCode(string token, CreatePINCodeRequest request)
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
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.PinCode == null && c.Status == true);

                if(customer is null)
                {
                    result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.ExistedPINCode);
                    return result;
                }

                customer.PinCode = request.PinCode;
                await _genericRepository.UpdateAsync(customer);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<CustomerProfileResponse>> GetCustomerByPhoneNumber(string phoneNumber)
        {
            var result = new Result<CustomerProfileResponse>();
            try
            {
                var customer = await _genericRepository.FindAsync(c => c.PhoneNumber.Equals(phoneNumber) && c.Status == true);
                if (customer == null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }
                var response = _mapper.Map<CustomerProfileResponse>(customer);
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<CustomerProfileResponse>> GetOwnProfile(string token)
        {
            var result = new Result<CustomerProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.Status == true);
                if (customer == null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }
                var response = _mapper.Map<CustomerProfileResponse>(customer);
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<CustomerProfileResponse>> UpdateProfile(string token, UpdateCustomerProfileRequest request)
        {
            var result = new Result<CustomerProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var customer = await _genericRepository.FindAsync(c => c.Uid == uid && c.Status == true);

                if (customer == null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var model = _mapper.Map(request, customer);
                await _genericRepository.UpdateAsync(model);
                result.Content = _mapper.Map<CustomerProfileResponse>(model);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> ValidatePINCode(string token, ValidatePINCodeRequest request)
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
                var customer = await _genericRepository.FindAsync(c => c.Uid.Equals(uid) && c.PinCode.Equals(request.PinCode) && c.Status == true);

                if (customer is null)
                {
                    result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.WrongPINCode);
                    return result;
                }

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
