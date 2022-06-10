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
        Task<CustomerLoginResponse> CustomerLogin(CustomerLoginRequest request);
        Task<StaffLoginResponse> StaffLogin(StaffLoginRequest request);
        Task<CreateCustomerAccountResponse> CreateCustomerAccount(CreateCustomerAccountRequest request);
    }
}
