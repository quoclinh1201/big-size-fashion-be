using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface ICustomerService
    {
        Task<Result<CustomerProfileResponse>> GetOwnProfile(string token);
        Task<Result<CustomerProfileResponse>> UpdateProfile(string token, UpdateCustomerProfileRequest request);
        //Task<Result<bool>> CreatePINCode(string token, CreatePINCodeRequest request);
        //Task<Result<bool>> ChangePINCode(string token, ChangePINCodeRequest request);
        //Task<Result<bool>> ValidatePINCode(string token, ValidatePINCodeRequest request);
        //Task<Result<bool>> CheckPINCode(string token);
        Task<Result<CustomerProfileResponse>> GetCustomerByPhoneNumber(string phoneNumber);
        Task<Result<CustomerProfileResponse>> AddNewCustomer(AddNewCustomerRequest request);
        Task<Result<string>> UploadAvatar(string v, IFormFile file);
        Task<Result<string>> GetAvatar(string v);
    }
}
