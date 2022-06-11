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
    public interface IAccountsService
    {
        Task<Result<CustomerLoginResponse>> CustomerLogin(CustomerLoginRequest request);
        Task<Result<StaffLoginResponse>> StaffLogin(StaffLoginRequest request);
        Task<Result<CreateCustomerAccountResponse>> CreateCustomerAccount(CreateCustomerAccountRequest request);
        Task<Result<AccountResponse>> CreateStaffAccount(CreateStaffAccountRequest request);
        Task<PagedResult<GetListAccountsResponse>> GetListAccounts(GetListAccountsParameter param);
    }
}
