using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IAccountService
    {
        Task<Result<CustomerLoginResponse>> CustomerLogin(CustomerLoginRequest request);
        Task<Result<StaffLoginResponse>> StaffLogin(StaffLoginRequest request);
        Task<Result<CreateCustomerAccountResponse>> CreateCustomerAccount(CreateCustomerAccountRequest request);
        Task<Result<AccountResponse>> CreateStaffAccount(CreateStaffAccountRequest request);
        Task<PagedResult<GetListAccountsResponse>> GetListAccounts(GetListAccountsParameter param);
        Task<Result<GetDetailUserByUidResponse>> GetDetailUserByUid(int uid);
        Task<Result<bool>> DisableAccount(int uid);
        Task<Result<bool>> ActiveAccount(int uid);
        Task<Result<string>> ChangePassword(string token, ChangePasswordRequest request);
    }
}
