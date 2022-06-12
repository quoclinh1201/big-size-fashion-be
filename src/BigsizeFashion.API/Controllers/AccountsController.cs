using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigsizeFashion.API.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _service;

        public AccountsController(IAccountsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Customer Login
        /// </summary>
        /// <remarks>
        /// - Nếu người dùng chưa có tài khoản trong bảng Account thì IsNewCustomer sẽ là true.
        /// - FE tiến hành cho user nhập các thông tin cần thiết để tạo tài khoản mới.
        /// - Sau đó gọi API CreateCustomerAccount
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("customer-login")]
        public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.CustomerLogin(request);
            return Ok(result);
        }

        /// <summary>
        /// Staff Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("staff-login")]
        public async Task<IActionResult> StaffLogin([FromBody] StaffLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.StaffLogin(request);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        /// <summary>
        /// Create Account for Customer
        /// </summary>
        /// <remarks>
        /// Gender có 3 giá trị là Male, Female, Unknow
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("create-customer-account")]
        public async Task<IActionResult> CreateCustomerAccount([FromBody] CreateCustomerAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.CreateCustomerAccount(request);
            return Ok(result);
        }

        /// <summary>
        /// [Admin] Create Account for Staff, Manager
        /// </summary>
        /// <remarks>
        /// RoleAccount có 2 giá trị là Staff và Manager
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("create-staff-account")]
        public async Task<IActionResult> CreateStaffAccount([FromBody] CreateStaffAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.CreateStaffAccount(request);
            if(result != null)
                return Ok(result);
            return BadRequest("The manager is existed in this store");
        }

        /// <summary>
        /// Get list accounts
        /// </summary>
        /// <remarks>
        /// - Có thể search by Fullname
        /// - Bỏ trống field Fullname thì là get all
        /// </remarks>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("get-list-manager-accounts")]
        public async Task<IActionResult> GetListAccounts([FromQuery] GetListAccountsParameter param)
        {
            var result = await _service.GetListAccounts(param);
            return Ok(result);
        }

        /// <summary>
        /// For admin or manager view detail account
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("get-detail-by-uid/{uid}")]
        public async Task<IActionResult> GetDetailByUid(int uid)
        {
            
            var result = await _service.GetDetailUserByUid(uid);
            if(result != null)
                return Ok(result);
            return NoContent();
        }

        /// <summary>
        /// Use for ban customer or disable staff account
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete("disable-account/uid")]
        public async Task<IActionResult> DisableAccount(int uid)
        {
            var result = await _service.DisableAccount(uid);
            return Ok(result);
        }

        /// <summary>
        /// Use for unban customer or active staff account
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("active-account/uid")]
        public async Task<IActionResult> ActiveAccount(int uid)
        {
            var result = await _service.ActiveAccount(uid);
            return Ok(result);
        }

        //[Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromHeader] string authorization, [FromBody] ChangePasswordRequest request)
        {
            var result = await _service.ChangePassword(authorization.Substring(7), request);
            return Ok(result);
        }
    }
}
