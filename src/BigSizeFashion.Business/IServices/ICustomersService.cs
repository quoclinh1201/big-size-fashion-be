using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface ICustomersService
    {
        Task<Result<CustomerProfileResponse>> GetOwnProfile(string token);
        Task<Result<CustomerProfileResponse>> UpdateProfile(string token, UpdateCustomerProfileRequest request);
        Task<Result<bool>> CreatePINCode(string token, CreatePINCodeRequest request);
        Task<Result<bool>> ChangePINCode(string token, ChangePINCodeRequest request);
        Task<Result<bool>> ValidatePINCode(string token, ValidatePINCodeRequest request);
        Task<Result<bool>> CheckPINCode(string token);
    }
}
