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
    [Route("api/v1/colour")]
    [ApiController]
    public class ColoursController : ControllerBase
    {
        private readonly IColourService _service;

        public ColoursController(IColourService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all colour
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllColour([FromQuery] SearchColourParameter param)
        {
            var result = await _service.GetAllColour(param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get colour by ID
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}", Name = "GetColourByID")]
        public async Task<IActionResult> GetColourByID(int id)
        {
            var result = await _service.GetColourByID(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create new Colour
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateColour([FromBody] ColourRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreateColour(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return CreatedAtRoute(nameof(GetColourByID), new { id = result.Content.ColourId }, result);
        }

        /// <summary>
        /// Update Colour
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateColour(int id, [FromBody] ColourRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdateColour(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete Colour
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColour(int id)
        {
            var result = await _service.DeleteColour(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
