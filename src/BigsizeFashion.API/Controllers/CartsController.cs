using BigSizeFashion.Business.Dtos.RequestObjects;
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
    [Route("api/v1/carts")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _service;

        public CartsController(ICartService service)
        {
            _service = service;
        }

        [HttpPost("add-cart")]
        public async Task<IActionResult> CreateCart([FromBody] AddToCartRequest request, [FromHeader] string authorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.AddToCart(request, authorization.Substring(7));

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(CreateListCart),result);
        }

        [HttpPost("add-list-cart")]
        public async Task<IActionResult> CreateListCart([FromBody] List<AddToCartRequest> request, [FromHeader] string authorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.AddToListCart(request, authorization.Substring(7));

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(CreateListCart), result);

        }

        [HttpGet("get-list-cart")]
        public async Task<IActionResult> GetListProductFitWithCustomer([FromHeader] string authorization)
        {
            var result = await _service.getListCart(authorization.Substring(7));
            return Ok(result);
        }

        /// <summary>
        /// Get all product in cart
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> GetAllProductInCartByCustomerID([FromHeader] string authorization)
        //{
        //    var result = await _service.GetAllProductInCartByCustomerID(authorization.Substring(7));
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Add product to cart
        /// </summary>
        /// <remarks>
        /// Dùng để thêm sản phẩm mới vào giỏ hàng
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> AddProductToCart([FromHeader] string authorization, [FromBody] ManageProductInCartRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var result = await _service.AddProductToCart(authorization.Substring(7), request);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Increase product quantity in cart (+1 per request)
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpPut("increase-product")]
        //public async Task<IActionResult> IncreaseProductInCart([FromHeader] string authorization, [FromBody] ManageProductInCartRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var result = await _service.IncreaseProductInCart(authorization.Substring(7), request);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Decrease product quantity in cart (will remove if quantity = 0)
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpPut("decrease-product")]
        //public async Task<IActionResult> DecreaseProductInCart([FromHeader] string authorization, [FromBody] ManageProductInCartRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var result = await _service.DecreaseProductInCart(authorization.Substring(7), request);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        //[Authorize]
        //[HttpDelete("remove-product")]
        //public async Task<IActionResult> RemoveProductInCart([FromHeader] string authorization, [FromBody] ManageProductInCartRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var result = await _service.RemoveProductInCart(authorization.Substring(7), request);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}
    }
}
