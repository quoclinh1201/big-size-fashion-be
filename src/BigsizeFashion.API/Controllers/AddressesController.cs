using BigSizeFashion.Business.Dtos.Requests;
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
    [Route("api/v1/addresses")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _service;

        public AddressesController(IAddressService service)
        {
            _service = service;
        }

        /// <summary>
        /// Customer get all own address
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAddresses([FromHeader] string authorization)
        {
            var result = await _service.GetAllAddresses(authorization.Substring(7));
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Customer get own address by id
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}", Name = "GetAddressById")]
        public async Task<IActionResult> GetAddressById([FromHeader] string authorization, int id)
        {
            var result = await _service.GetAddressById(authorization.Substring(7), id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create address
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromHeader] string authorization, [FromBody] AddressRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreateAddress(authorization.Substring(7), request);
            if (!result.IsSuccess)
            {
                if(result.Error.Code == 401)
                {
                    return Unauthorized(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            return CreatedAtRoute(nameof(GetAddressById), new { id = result.Content.AddressId }, result);
        }

        /// <summary>
        /// Update address
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress([FromHeader] string authorization, [FromBody] AddressRequest request, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdateAddress(authorization.Substring(7), request, id);
            if (!result.IsSuccess)
            {
                if (result.Error.Code == 401)
                {
                    return Unauthorized(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            return Ok(result);
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress([FromHeader] string authorization, int id)
        {
            var result = await _service.DeleteAddress(authorization.Substring(7), id);
            if (!result.IsSuccess)
            {
                if (result.Error.Code == 401)
                {
                    return Unauthorized(result);
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
