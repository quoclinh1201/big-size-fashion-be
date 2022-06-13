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
    [Route("api/v1/sizes")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _service;

        public SizesController(ISizeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all size
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllSize([FromQuery] SearchSizeParameter param)
        {
            var result = await _service.GetAllSize(param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get size by ID
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}", Name = "GetSizeByID")]
        public async Task<IActionResult> GetSizeByID(int id)
        {
            var result = await _service.GetSizeByID(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create new Size
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateSize([FromBody] SizeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreateSize(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return CreatedAtRoute(nameof(GetSizeByID), new { id = result.Content.SizeId }, result);
        }

        /// <summary>
        /// Update Size
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSize(int id, [FromBody] SizeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdateSize(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete Size
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(int id)
        {
            var result = await _service.DeleteSize(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
