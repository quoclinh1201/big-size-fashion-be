using BigSizeFashion.Business.Dtos.Parameters;
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
    [Route("api/v1/product-details")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        private readonly IProductDetailService _service;

        public ProductDetailsController(IProductDetailService service)
        {
            _service = service;
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDetailForProduct([FromBody] ProductDetailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.AddDetailForProduct(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

<<<<<<< HEAD
        //[HttpGet("get-product-detail")]
        //public async Task<IActionResult> GetProductDetail([FromQuery] GetProductDetailParameter param)
        //{
        //    var result = await _service.GetProductDetail(param);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

=======
        /// <summary>
        /// Get Product Detail ID
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProductDetailId([FromQuery] GetProductDetailIdParameter param)
        {
            var result = await _service.GetProductDetailId(param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
>>>>>>> c2916260d9bfd1c2db37e33dc2217d9409648e9a
    }
}
