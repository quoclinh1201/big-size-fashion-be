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
        /// - FE tiến hành cho user tạo tài khoản mới.
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("customer-login")]
        public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginRequest request)
        {
            var result = await _service.CustomerLogin(request);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest();
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
            var result = await _service.StaffLogin(request);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
