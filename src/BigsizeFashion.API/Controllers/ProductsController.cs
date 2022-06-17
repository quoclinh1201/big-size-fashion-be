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
    [Route("api/v1/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get list product
        /// </summary>
        /// <remarks>
        /// - orderByPrice = true => Sắp xếp theo giá giảm dần
        /// - orderByPrice = false => Sắp xếp theo giá tăng
        /// - orderByPrice = null => Không sắp xếp
        /// --------------------------------------------------
        /// - status = null => get with all status
        /// </remarks>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetListProductsWithAllStatus([FromQuery] SearchProductsParameter param)
        {
            var result = await _service.GetListProductsWithAllStatus(param);
            return Ok(result);
        }

        /// <summary>
        /// Create product
        /// </summary>
        /// <remarks>
        /// - Gender = true => Male
        /// - Gender = false => Female
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreateProduct(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return CreatedAtRoute(nameof(GetProductByID), new { id = result.Content.ProductId }, result);
        }

        /// <summary>
        /// Get product by ID (Get detail)
        /// </summary>
        /// <remarks>
        /// For Admin and Owner
        /// </remarks>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}", Name = "GetProductByID")]
        public async Task<IActionResult> GetProductByID(int id)
        {
            var result = await _service.GetProductByID(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        
    }
}
