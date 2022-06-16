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
    [Route("api/v1/stores")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _service;

        public StoresController(IStoreService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all store
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllStore([FromQuery] SearchStoreParameter param)
        {
            var result = await _service.GetAllStore(param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get store by ID
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}", Name = "GetStoreByID")]
        public async Task<IActionResult> GetStoreByID(int id)
        {
            var result = await _service.GetStoreByID(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create new store
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreateStore(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return CreatedAtRoute(nameof(GetStoreByID), new { id = result.Content.StoreId }, result);
        }

        /// <summary>
        /// Update store
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] CreateStoreRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdateStore(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var result = await _service.DeleteStore(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
