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

        public async Task<CreateCustomerAccountResponse> CreateCustomerAccount(CreateCustomerAccountRequest request)
        {
            try
            {
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
                return new CreateCustomerAccountResponse { Token = token };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AccountResponse> CreateStaffAccount(CreateStaffAccountRequest request)
        {
            try
            {
                var account = _mapper.Map<Account>(request);
                var role = await _roleRepository.FindAsync(r => r.Role1.Equals(request.RoleAccount));
                account.RoleId = role.RoleId;
                await _accountRepository.InsertAsync(account);
                await _accountRepository.SaveAsync();


                var staff = _mapper.Map<staff>(request);
                staff.Uid = account.Uid;
                await _staffRepository.InsertAsync(staff);
                await _staffRepository.SaveAsync();

                var result = _mapper.Map<AccountResponse>(account);
                result.RoleAccount = request.RoleAccount;
                result.Fullname = staff.Fullname;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CustomerLoginResponse> CustomerLogin(CustomerLoginRequest request)
        {
            try
            {
                var account = _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Username.Equals(request.PhoneNumber))
                    .Include(a => a.Role).FirstOrDefault(); 
                if(account != null)
                {
                    var customer = await _customerRepository.FindAsync(c => c.Uid.Equals(account.Uid));
                    var token = GenerateJSONWebToken(customer.Uid.ToString(), customer.Fullname, account.Role.Role1);
                    return new CustomerLoginResponse { Token = token ,IsNewCustomer = false };
                }
                else
                {
                    return new CustomerLoginResponse { IsNewCustomer = true };
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<StaffLoginResponse> StaffLogin(StaffLoginRequest request)
        {
            try
            {
                var account = _accountRepository.GetAllByIQueryable()
                    .Where(a => a.Username.Equals(request.Username) && a.Password.Equals(request.Password))
                    .Include(a => a.Role).FirstOrDefault();
                if(account != null)
                {
                    var staff = await _staffRepository.FindAsync(c => c.Uid.Equals(account.Uid));
                    var token = GenerateJSONWebToken(staff.Uid.ToString(), staff.Fullname, account.Role.Role1);
                    return new StaffLoginResponse { Token = token, Role = account.Role.Role1 };
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
    }
}
