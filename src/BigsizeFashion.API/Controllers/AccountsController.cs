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
        private readonly IAccountService _service;

        public AccountsController(IAccountService service)
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
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //[AllowAnonymous]
        //[HttpPost("staff-login")]
        //public async Task<IActionResult> StaffLogin([FromBody] UsernamePasswordLoginRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    var result = await _service.StaffLogin(request);

        //    if (!result.IsSuccess)
        //    {
        //        if (result.Error.Code == 404)
        //        {
        //            return NotFound(result);
        //        }
        //        else
        //        {
        //            return BadRequest(result);
        //        }
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// For Admin, Owner, Manager, Staff login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsernamePasswordLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.Login(request);

            if (!result.IsSuccess)
            {
                if (result.Error.Code == 404)
                {
                    return NotFound(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// Create Account for Customer
        /// </summary>
        /// <remarks>
        /// Gender có 2 giá trị là Male, Female
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

            if (!result.IsSuccess)
            {
                 return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create Account for Staff, Manager
        /// </summary>
        /// <remarks>
        /// RoleAccount có 2 giá trị là Staff và Manager
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("create-staff-account")]
        public async Task<IActionResult> CreateStaffAccount([FromBody] CreateStaffAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.CreateStaffAccount(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create account for admin and owner
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.CreateAccount(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
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
        [Authorize]
        [HttpGet("get-list-accounts")]
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
        [Authorize]
        [HttpGet("get-detail-by-uid/{uid}")]
        public async Task<IActionResult> GetDetailByUid(int uid)
        {
            if(uid < 1)
            {
                return BadRequest();
            }
            var result = await _service.GetDetailUserByUid(uid);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            if (result != null)
                return Ok(result);
            return NoContent();
        }

        /// <summary>
        /// Use for ban customer or disable staff account
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("disable-account/uid")]
        public async Task<IActionResult> DisableAccount(int uid)
        {
            if (uid < 1)
            {
                return BadRequest();
            }
            var result = await _service.DisableAccount(uid);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Use for unban customer or active staff account
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("active-account/uid")]
        public async Task<IActionResult> ActiveAccount(int uid)
        {
            if (uid < 1)
            {
                return BadRequest();
            }
            var result = await _service.ActiveAccount(uid);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromHeader] string authorization, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.ChangePassword(authorization.Substring(7), request);
            if (!result.IsSuccess)
            {
                if(result.Error.Code == 401)
                {
                    return Unauthorized(result);
                } else
                {
                    return BadRequest(result);
                }
            }
            return Ok(result);
        }
    }
}
