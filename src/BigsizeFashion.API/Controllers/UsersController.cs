﻿using BigSizeFashion.Business.Helpers.RequestObjects;
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
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Admin and owner get own profile
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
        /// Admin and owner update own profile
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromHeader] string authorization, [FromBody] UpdateUserProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdateProfile(authorization.Substring(7), request);
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
    }
}
