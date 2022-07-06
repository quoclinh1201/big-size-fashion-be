using AutoMapper;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.Enums;
using BigSizeFashion.Business.Helpers.Constants;

namespace BigSizeFashion.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<User> _userRepository;
        //private readonly IGenericRepository<Admin> _adminRepository;
        //private readonly IGenericRepository<Owner> _ownerRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private IConfiguration _config;

        public AccountService(
            IGenericRepository<Account> accountRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<User> userRepository,
            //IGenericRepository<Admin> adminRepository,
            //IGenericRepository<Owner> ownerRepository,
            IGenericRepository<Role> roleRepository,
            IMapper mapper,
            IConfiguration config)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _userRepository = userRepository;
            //_adminRepository = adminRepository;
            //_ownerRepository = ownerRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _config = config;
        }

        public async Task<Result<CreateCustomerAccountResponse>> CreateCustomerAccount(CreateCustomerAccountRequest request)
        {
            var result = new Result<CreateCustomerAccountResponse>();
            try
            {
                var account = _mapper.Map<Account>(request);
                var role = await _roleRepository.FindAsync(r => r.RoleName.Equals("Customer"));
                account.RoleId = role.RoleId;
                await _accountRepository.InsertAsync(account);
                await _accountRepository.SaveAsync();

                var customer = _mapper.Map<Customer>(request);
                customer.Uid = account.Uid;
                await _customerRepository.InsertAsync(customer);
                await _customerRepository.SaveAsync();

                var token = GenerateJSONWebToken(account.Uid.ToString(), customer.Fullname, account.Role.RoleName);
                result.Content = new CreateCustomerAccountResponse { Token = token };
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<AccountResponse>> CreateStaffAccount(CreateStaffAccountRequest request)
        {
            var result = new Result<AccountResponse>();
            try
            {
                var account = _mapper.Map<Account>(request);
                var role = await _roleRepository.FindAsync(r => r.RoleName.Equals(request.RoleAccount));
                account.RoleId = role.RoleId;

                if (role.RoleName.Equals("Manager"))
                {
                    var checkExistedManager = await _accountRepository.GetAllByIQueryable()
                        .Include(a => a.staff)
                        .Where(a => a.staff.StoreId == request.StoreId && a.RoleId == role.RoleId && a.Status == true)
                        .FirstOrDefaultAsync();
                    if (checkExistedManager != null)
                    {
                        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.ExistedManagerInStore);
                        return result;
                    }
                }

                await _accountRepository.InsertAsync(account);
                await _accountRepository.SaveAsync();


                var staff = _mapper.Map<staff>(request);
                staff.Uid = account.Uid;
                await _staffRepository.InsertAsync(staff);
                await _staffRepository.SaveAsync();

                var response = _mapper.Map<AccountResponse>(account);
                response.RoleAccount = request.RoleAccount;
                response.Fullname = staff.Fullname;
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<CustomerLoginResponse>> CustomerLogin(CustomerLoginRequest request)
        {
            var result = new Result<CustomerLoginResponse>();
            try
            {
                var account = _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Username.Equals(request.PhoneNumber))
                    .Include(a => a.Role).FirstOrDefault();
                if (account != null)
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(account.Uid));
                    var token = GenerateJSONWebToken(customer.Uid.ToString(), customer.Fullname, account.Role.RoleName);
                    result.Content = new CustomerLoginResponse { Token = token, IsNewCustomer = false };
                    return result;
                }
                else
                {
                    result.Content = new CustomerLoginResponse { IsNewCustomer = true };
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }

        }

        public async Task<PagedResult<GetListAccountsResponse>> GetListAccounts(GetListAccountsParameter param)
        {
            try
            {
                var role = await _roleRepository.FindAsync(r => r.RoleName.Equals(param.Role.ToString()));
                ICollection<Account> accounts = null;
                if (param.Status.Equals(StatusEnum.Both))
                {
                    accounts = await _accountRepository.FindByAsync(a => a.RoleId.Equals(role.RoleId));
                }
                else
                {
                    var stt = param.Status == StatusEnum.Active ? true : false;
                    accounts = await _accountRepository
                        .FindByAsync(a => a.RoleId.Equals(role.RoleId) && a.Status.Equals(stt));
                }

                if (accounts == null)
                {
                    return null;
                }

                var response = _mapper.Map<List<GetListAccountsResponse>>(accounts);

                if (param.Role.ToString().Equals("Customer"))
                {
                    for (int i = 0; i < response.Count; i++)
                    {
                        var customer = await _customerRepository.FindAsync(m => m.Uid.Equals(response[i].Uid));
                        response[i].Fullname = customer.Fullname;
                    }
                    var query = response.AsQueryable();
                    FilterAccountByName(ref query, param.Fullname);
                    response = query.ToList();
                }
                else if (param.Role.ToString().Equals("Manager") || param.Role.ToString().Equals("Staff"))
                {
                    for (int i = 0; i < response.Count; i++)
                    {
                        var staff = await _staffRepository.FindAsync(m => m.Uid.Equals(response[i].Uid));
                        response[i].Fullname = staff.Fullname;
                    }
                    var query = response.AsQueryable();
                    FilterAccountByName(ref query, param.Fullname);
                    response = query.ToList();
                }
                else if (param.Role.ToString().Equals("Admin") || param.Role.ToString().Equals("Owner"))
                {
                    for (int i = 0; i < response.Count; i++)
                    {
                        var user = await _userRepository.FindAsync(m => m.Uid.Equals(response[i].Uid));
                        response[i].Fullname = user.Fullname;
                    }
                    var query = response.AsQueryable();
                    FilterAccountByName(ref query, param.Fullname);
                    response = query.ToList();
                }
                //else if(param.Role.ToString().Equals("Owner"))
                //{
                //    for (int i = 0; i < response.Count; i++)
                //    {
                //        var owner = await _ownerRepository.FindAsync(m => m.Uid.Equals(response[i].Uid));
                //        response[i].Fullname = owner.Fullname;
                //    }
                //    var query = response.AsQueryable();
                //    FilterAccountByName(ref query, param.Fullname);
                //    response = query.ToList();
                //}
                return PagedResult<GetListAccountsResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public async Task<Result<LoginResponse>> StaffLogin(UsernamePasswordLoginRequest request)
        //{
        //    var result = new Result<LoginResponse>();
        //    try
        //    {
        //        var account = _accountRepository.GetAllByIQueryable()
        //            .Where(a => a.Username.Equals(request.Username) && a.Password.Equals(request.Password))
        //            .Include(a => a.Role).FirstOrDefault();
        //        if(account != null)
        //        {
        //            if(account.Status == false)
        //            {
        //                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.BlockedAccount);
        //                return result;
        //            }

        //            var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(account.Uid));
        //            var token = GenerateJSONWebToken(staff.Uid.ToString(), staff.Fullname, account.Role.Role1);

        //            result.Content = new LoginResponse { Token = token, Role = account.Role.Role1 };
        //            return result;
        //        }
        //        result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.WrongUsernameOrPassword);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        private string GenerateJSONWebToken(string uid, string fullname, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Uid", uid),
                    new Claim("Fullname", fullname),
                    new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                                            _config["Jwt:Issuer"],
                                            claims,
                                            expires: DateTime.Now.AddMonths(3),
                                            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static void FilterAccountByName(ref IQueryable<GetListAccountsResponse> query, string name)
        {
            if (!query.Any() || String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            query = query.Where(q => q.Fullname.ToLower().Contains(name.ToLower()));
        }

        public async Task<Result<GetDetailUserByUidResponse>> GetDetailUserByUid(int uid)
        {
            var result = new Result<GetDetailUserByUidResponse>();
            try
            {
                var account = await _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Uid.Equals(uid)).Include(a => a.Role)
                    .FirstOrDefaultAsync();
                if (account.Role.RoleName.Equals(AccountRoleEnum.Customer.ToString()))
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(uid));
                    var response = _mapper.Map<GetDetailUserByUidResponse>(customer);
                    response.Role = account.Role.RoleName;
                    result.Content = response;
                    return result;
                }
                else if (account.Role.RoleName.Equals(AccountRoleEnum.Manager.ToString()) || account.Role.RoleName.Equals(AccountRoleEnum.Staff.ToString()))
                {
                    var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(uid));
                    var response = _mapper.Map<GetDetailUserByUidResponse>(staff);
                    response.Role = account.Role.RoleName;
                    result.Content = response;
                    return result;
                }
                else if (account.Role.RoleName.Equals(AccountRoleEnum.Admin.ToString()) || account.Role.RoleName.Equals(AccountRoleEnum.Owner.ToString()))
                {
                    var user = await _userRepository.FindAsync(c => c.Uid.Equals(uid));
                    var response = _mapper.Map<GetDetailUserByUidResponse>(user);
                    response.Role = account.Role.RoleName;
                    result.Content = response;
                    return result;
                }
                //else if (account.Role.RoleName.Equals(AccountRoleEnum.Owner.ToString()))
                //{
                //    var owner = await _ownerRepository.FindAsync(c => c.Uid.Equals(uid));
                //    var response = _mapper.Map<GetDetailUserByUidResponse>(owner);
                //    response.Role = account.Role.RoleName;
                //    result.Content = response;
                //    return result;
                //}
                return null;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DisableAccount(int uid)
        {
            var result = new Result<bool>();
            try
            {
                var account = await _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Uid.Equals(uid)).Include(a => a.Role)
                    .FirstOrDefaultAsync();

                if (account.Status == true)
                {
                    account.Status = false;
                }
                else
                {
                    account.Status = true;
                }
                await _accountRepository.SaveAsync();

                if (account.Role.RoleName.Equals(AccountRoleEnum.Customer.ToString()))
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(uid));
                    if (customer.Status == true)
                    {
                        customer.Status = false;
                    }
                    else
                    {
                        customer.Status = true;
                    }
                    await _customerRepository.SaveAsync();
                }
                else if (account.Role.RoleName.Equals(AccountRoleEnum.Manager.ToString()) || account.Role.RoleName.Equals(AccountRoleEnum.Staff.ToString()))
                {
                    var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(uid));
                    if (staff.Status == true)
                    {
                        staff.Status = false;
                    }
                    else
                    {
                        staff.Status = true;
                    }
                    await _staffRepository.SaveAsync();
                }
                else if (account.Role.RoleName.Equals(AccountRoleEnum.Admin.ToString()) || account.Role.RoleName.Equals(AccountRoleEnum.Owner.ToString()))
                {
                    var admin = await _userRepository.FindAsync(c => c.Uid.Equals(uid));
                    if (admin.Status == true)
                    {
                        admin.Status = false;
                    }
                    else
                    {
                        admin.Status = true;
                    }
                    await _userRepository.SaveAsync();
                }
                //else if (account.Role.RoleName.Equals(AccountRoleEnum.Owner.ToString()))
                //{
                //    var owner = await _ownerRepository.FindAsync(c => c.Uid.Equals(uid));
                //    owner.Status = false;
                //    await _ownerRepository.SaveAsync();
                //}

                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> ActiveAccount(int uid)
        {
            var result = new Result<bool>();
            try
            {
                var account = await _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Uid.Equals(uid)).Include(a => a.Role)
                    .FirstOrDefaultAsync();

                account.Status = true;
                await _accountRepository.SaveAsync();

                if (account.Role.RoleName.Equals(AccountRoleEnum.Customer.ToString()))
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(uid));
                    customer.Status = true;
                    await _customerRepository.SaveAsync();
                }
                else if (account.Role.RoleName.Equals(AccountRoleEnum.Manager.ToString()) || account.Role.RoleName.Equals(AccountRoleEnum.Staff.ToString()))
                {
                    var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(uid));
                    staff.Status = true;
                    await _staffRepository.SaveAsync();
                }
                else if (account.Role.RoleName.Equals(AccountRoleEnum.Admin.ToString()) || account.Role.RoleName.Equals(AccountRoleEnum.Owner.ToString()))
                {
                    var admin = await _userRepository.FindAsync(c => c.Uid.Equals(uid));
                    admin.Status = true;
                    await _userRepository.SaveAsync();
                }
                //else if (account.Role.RoleName.Equals(AccountRoleEnum.Owner.ToString()))
                //{
                //    var owner = await _ownerRepository.FindAsync(c => c.Uid.Equals(uid));
                //    owner.Status = true;
                //    await _ownerRepository.SaveAsync();
                //}
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> ChangePassword(string token, ChangePasswordRequest request)
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

                var account = await _accountRepository.FindAsync(a => a.Uid.Equals(uid) && a.Password.Equals(request.OldPassword));

                if (account == null)
                {
                    result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.WrongOldPassword);
                    return result;
                }

                account.Password = request.ConfirmNewPassword;
                await _accountRepository.SaveAsync();
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<LoginResponse>> Login(UsernamePasswordLoginRequest request)
        {
            var result = new Result<LoginResponse>();
            try
            {
                var account = _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Username.Equals(request.Username) && a.Password.Equals(request.Password))
                    .Include(a => a.Role).FirstOrDefault();
                if (account != null)
                {
                    if (account.Status == false)
                    {
                        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.BlockedAccount);
                        return result;
                    }

                    var token = String.Empty;

                    if (account.Role.RoleName.Equals("Admin") || account.Role.RoleName.Equals("Owner"))
                    {
                        var user = await _userRepository.FindAsync(c => c.Uid.Equals(account.Uid));
                        token = GenerateJSONWebToken(user.Uid.ToString(), user.Fullname, account.Role.RoleName);
                    }
                    else if (account.Role.RoleName.Equals("Manager") || account.Role.RoleName.Equals("Staff"))
                    {
                        var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(account.Uid));
                        token = GenerateJSONWebToken(staff.Uid.ToString(), staff.Fullname, account.Role.RoleName);
                    }
                    //else if (account.Role.RoleName.Equals("Owner"))
                    //{
                    //    var owner = await _ownerRepository.FindAsync(c => c.Uid.Equals(account.Uid));
                    //    token = GenerateJSONWebToken(owner.Uid.ToString(), owner.Fullname, account.Role.RoleName);
                    //}

                    result.Content = new LoginResponse { Token = token, Role = account.Role.RoleName };
                    return result;
                }
                result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.WrongUsernameOrPassword);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<AccountResponse>> CreateAccount(CreateAccountRequest request)
        {
            var result = new Result<AccountResponse>();
            try
            {
                var account = _mapper.Map<Account>(request);
                var role = await _roleRepository.FindAsync(r => r.RoleName.Equals(request.RoleAccount));
                account.RoleId = role.RoleId;

                await _accountRepository.InsertAsync(account);
                await _accountRepository.SaveAsync();

                
                //if (role.RoleName.Equals("Admin"))
                //{
                var user = _mapper.Map<User>(request);
                user.Uid = account.Uid;
                await _userRepository.InsertAsync(user);
                await _userRepository.SaveAsync();
                var fullname = user.Fullname;
                //}
                //else if (role.RoleName.Equals("Owner"))
                //{
                //    var owner = _mapper.Map<Owner>(request);
                //    owner.Uid = account.Uid;
                //    await _ownerRepository.InsertAsync(owner);
                //    await _ownerRepository.SaveAsync();
                //    fullname = owner.Fullname;
                //}

                var response = _mapper.Map<AccountResponse>(account);
                response.RoleAccount = request.RoleAccount;
                response.Fullname = fullname;
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
