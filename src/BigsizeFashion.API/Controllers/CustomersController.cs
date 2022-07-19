using BigSizeFashion.Business.Dtos.Requests;
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
    [Route("api/v1/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        /// <summary>
        /// Allow customer get their own profile
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-own-profile")]
        public async Task<IActionResult> GetOwnProfile([FromHeader] string authorization)
        {
            var result = await _service.GetOwnProfile(authorization.Substring(7));
            if (!result.IsSuccess)
            {
                if (result.Error.Code == 401)
                {
                    return Unauthorized(result);
                }
                else if (result.Error.Code == 404)
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
        /// Allow customer update their own profile
        /// </summary>
        /// <remarks>
        /// - Birthday is string and must have format is dd/MM/yyyy
        /// - Example: 01/01/2022
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromHeader] string authorization, [FromBody] UpdateCustomerProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdateProfile(authorization.Substring(7), request);
            if (!result.IsSuccess)
            {
                if(result.Error.Code == 401)
                {
                    return Unauthorized(result);
                }
                else if (result.Error.Code == 404)
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

        //[Authorize]
        //[HttpPost("create-pin-code")]
        //public async Task<IActionResult> CreatePINCode([FromHeader] string authorization, [FromBody] CreatePINCodeRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var result = await _service.CreatePINCode(authorization.Substring(7), request);
        //    if (!result.IsSuccess)
        //    {
        //        if(result.Error.Code == 401)
        //        {
        //            return Unauthorized(result);
        //        }
        //        else
        //        {
        //            return BadRequest(result);
        //        }
        //    }
        //    return Ok(result);
        //}

        //[Authorize]
        //[HttpPut("change-pin-code")]
        //public async Task<IActionResult> ChangePINCode([FromHeader] string authorization, [FromBody] ChangePINCodeRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var result = await _service.ChangePINCode(authorization.Substring(7), request);
        //    if (!result.IsSuccess)
        //    {
        //        if (result.Error.Code == 401)
        //        {
        //            return Unauthorized(result);
        //        }
        //        else
        //        {
        //            return BadRequest(result);
        //        }
        //    }
        //    return Ok(result);
        //}

        //[Authorize]
        //[HttpGet("check-pin-code")]
        //public async Task<IActionResult> CheckPINCode([FromHeader] string authorization)
        //{
        //    var result = await _service.CheckPINCode(authorization.Substring(7));
        //    if(!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}


        //[Authorize]
        //[HttpPost("validate-pin-code")]
        //public async Task<IActionResult> ValidatePINCode([FromHeader] string authorization, [FromBody] ValidatePINCodeRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var result = await _service.ValidatePINCode(authorization.Substring(7), request);
        //    if (!result.IsSuccess)
        //    {
        //        if (result.Error.Code == 401)
        //        {
        //            return Unauthorized(result);
        //        }
        //        else
        //        {
        //            return BadRequest(result);
        //        }
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Get customer informaion by phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{phoneNumber}")]
        public async Task<IActionResult> GetCustomerByPhoneNumber(string phoneNumber)
        {
            var result = await _service.GetCustomerByPhoneNumber(phoneNumber);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //[Authorize]
        [HttpPost("add-new-customer")]
        public async Task<IActionResult> AddNewCustomer([FromBody] AddNewCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.AddNewCustomer(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
