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

namespace BigSizeFashion.Business.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private IConfiguration _config;

        public AccountsService(
            IGenericRepository<Account> accountRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<Role> roleRepository,
            IMapper mapper,
            IConfiguration config)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _config = config;
        }

        public async Task<Result<CreateCustomerAccountResponse>> CreateCustomerAccount(CreateCustomerAccountRequest request)
        {
            try
            {
                var result = new Result<CreateCustomerAccountResponse>();
                var account = _mapper.Map<Account>(request);
                var role = await _roleRepository.FindAsync(r => r.Role1.Equals("Customer"));
                account.RoleId = role.RoleId;
                await _accountRepository.InsertAsync(account);
                await _accountRepository.SaveAsync();

                var customer = _mapper.Map<Customer>(request);
                customer.Uid = account.Uid;
                await _customerRepository.InsertAsync(customer);
                await _customerRepository.SaveAsync();

                var token = GenerateJSONWebToken(account.Uid.ToString(), customer.Fullname, account.Role.Role1);
                result.Content = new CreateCustomerAccountResponse { Token = token };
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<AccountResponse>> CreateStaffAccount(CreateStaffAccountRequest request)
        {
            try
            {
                var result = new Result<AccountResponse>();
                var account = _mapper.Map<Account>(request);
                var role = await _roleRepository.FindAsync(r => r.Role1.Equals(request.RoleAccount));
                account.RoleId = role.RoleId;
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
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<CustomerLoginResponse>> CustomerLogin(CustomerLoginRequest request)
        {
            try
            {
                var result = new Result<CustomerLoginResponse>();
                var account = _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Username.Equals(request.PhoneNumber))
                    .Include(a => a.Role).FirstOrDefault(); 
                if(account != null)
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(account.Uid));
                    var token = GenerateJSONWebToken(customer.Uid.ToString(), customer.Fullname, account.Role.Role1);
                    result.Content = new CustomerLoginResponse { Token = token, IsNewCustomer = false };
                    return result;
                }
                else
                {
                    result.Content = new CustomerLoginResponse { IsNewCustomer = true };
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<PagedResult<GetListAccountsResponse>> GetListAccounts(GetListAccountsParameter param)
        {
            try
            {
                var role = await _roleRepository.FindAsync(r => r.Role1.Equals(param.Role.ToString()));
                ICollection<Account> accounts = null;
                if(param.Status.Equals(AccountStatusEnum.Both))
                {
                    accounts = await _accountRepository.FindByAsync(a => a.RoleId.Equals(role.RoleId));
                }
                else
                {
                    var stt = param.Status == AccountStatusEnum.Active ? true : false;
                    accounts = await _accountRepository
                        .FindByAsync(a => a.RoleId.Equals(role.RoleId) && a.Status.Equals(stt));
                }

                if(accounts == null)
                {
                    return null;
                }

                var response = _mapper.Map<List<GetListAccountsResponse>>(accounts);

                if(param.Role.ToString().Equals("Customer"))
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
                else if(param.Role.ToString().Equals("Manager") || param.Role.ToString().Equals("Staff"))
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
                return PagedResult<GetListAccountsResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<StaffLoginResponse>> StaffLogin(StaffLoginRequest request)
        {
            try
            {
                var result = new Result<StaffLoginResponse>();
                var account = _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Username.Equals(request.Username) && a.Password.Equals(request.Password))
                    .Include(a => a.Role).FirstOrDefault();
                if(account != null)
                {
                    var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(account.Uid));
                    var token = GenerateJSONWebToken(staff.Uid.ToString(), staff.Fullname, account.Role.Role1);

                    result.Content = new StaffLoginResponse { Token = token, Role = account.Role.Role1 };
                    return result;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

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
                                            expires: DateTime.Now.AddHours(24),
                                            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void FilterAccountByName(ref IQueryable<GetListAccountsResponse> query, string name)
        {
            if (!query.Any() || String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            query = query.Where(q => q.Fullname.ToLower().Contains(name.ToLower()));
        }

        public async Task<Result<GetDetailUserByUidResponse>> GetDetailUserByUid(int uid)
        {
            try
            {
                var result = new Result<GetDetailUserByUidResponse>();
                var account = await _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Uid.Equals(uid)).Include(a => a.Role)
                    .FirstOrDefaultAsync();
                if(account.Role.Role1.Equals(AccountRoleEnum.Customer.ToString()))
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(uid));
                    var response = _mapper.Map<GetDetailUserByUidResponse>(customer);
                    response.Role = account.Role.Role1;
                    result.Content = response;
                    return result;
                } 
                else if(account.Role.Role1.Equals(AccountRoleEnum.Manager.ToString()) || account.Role.Role1.Equals(AccountRoleEnum.Staff.ToString()))
                {
                    var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(uid));
                    var response = _mapper.Map<GetDetailUserByUidResponse>(staff);
                    response.Role = account.Role.Role1;
                    result.Content = response;
                    return result;
                } 
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<bool>> DisableAccount(int uid)
        {
            try
            {
                var result = new Result<bool>();
                var account = await _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Uid.Equals(uid)).Include(a => a.Role)
                    .FirstOrDefaultAsync();

                account.Status = false;
                await _accountRepository.SaveAsync();

                if (account.Role.Role1.Equals(AccountRoleEnum.Customer.ToString()))
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(uid));
                    customer.Status = false;
                    await _customerRepository.SaveAsync();
                }
                else if (account.Role.Role1.Equals(AccountRoleEnum.Manager.ToString()) || account.Role.Role1.Equals(AccountRoleEnum.Staff.ToString()))
                {
                    var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(uid));
                    staff.Status = false;
                    await _staffRepository.SaveAsync();
                }
                result.Content = true;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result<bool>> ActiveAccount(int uid)
        {
            try
            {
                var result = new Result<bool>();
                var account = await _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Uid.Equals(uid)).Include(a => a.Role)
                    .FirstOrDefaultAsync();

                account.Status = true;
                await _accountRepository.SaveAsync();

                if (account.Role.Role1.Equals(AccountRoleEnum.Customer.ToString()))
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(uid));
                    customer.Status = true;
                    await _customerRepository.SaveAsync();
                }
                else if (account.Role.Role1.Equals(AccountRoleEnum.Manager.ToString()) || account.Role.Role1.Equals(AccountRoleEnum.Staff.ToString()))
                {
                    var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(uid));
                    staff.Status = true;
                    await _staffRepository.SaveAsync();
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
