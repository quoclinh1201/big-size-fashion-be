using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigsizeFashion.API.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCategory([FromQuery] SearchCategoryParameter param)
        {
            var result = await _service.GetAllCategory(param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}", Name = "GetCategoryByID")]
        public async Task<IActionResult> GetCategoryByID(int id)
        {
            var result = await _service.GetCategoryByID(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create new category
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreateCategory(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return CreatedAtRoute(nameof(GetCategoryByID), new { id = result.Content.CategoryId }, result);
        }

        /// <summary>
        /// Update category
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdateCategory(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _service.DeleteCategory(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
