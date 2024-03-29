﻿using BigSizeFashion.Business.Dtos.Requests;
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
        //Task<Result<LoginResponse>> StaffLogin(UsernamePasswordLoginRequest request);
        Task<Result<CreateCustomerAccountResponse>> CreateCustomerAccount(CreateCustomerAccountRequest request);
        Task<Result<AccountResponse>> CreateStaffAccount(CreateStaffAccountRequest request);
        Task<PagedResult<GetListAccountsResponse>> GetListAccounts(GetListAccountsParameter param);
        Task<Result<GetDetailUserByUidResponse>> GetDetailUserByUid(int uid);
        Task<Result<bool>> DisableAccount(int uid);
        Task<Result<bool>> ActiveAccount(int uid);
        Task<Result<bool>> ChangePassword(string token, ChangePasswordRequest request);
        Task<Result<LoginResponse>> Login(UsernamePasswordLoginRequest request);
        Task<Result<AccountResponse>> CreateAccount(CreateAccountRequest request);
        Task<Result<bool>> ResetPassword(int uid, ResetPasswordRequest request);
    }
}
