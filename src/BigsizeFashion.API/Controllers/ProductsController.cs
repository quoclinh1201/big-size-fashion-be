using BigSizeFashion.Business.Dtos.Parameters;
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
        /// - Use for all role
        /// -------------------------------------------------
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
        //[Authorize]
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
        /// Use for all role
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


        /// <summary>
        /// Get list product of store
        /// </summary>
        /// <remarks>
        /// - Get list product with quantity for staff and manager
        /// - Status must be true
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpGet("store")]
        //public async Task<IActionResult> GetListProductOfStore([FromHeader] string authorization, [FromQuery] SearchProductsParameter param)
        //{
        //    var result = await _service.GetListProductOfStore(authorization.Substring(7), param);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Get product by ID (Get detail)
        /// </summary>
        /// <remarks>
        /// - Get with quantity for manager and staff
        /// </remarks>
        /// <returns></returns>
        //[Authorize]
        //[HttpGet("store/{id}")]
        //public async Task<IActionResult> GetProductOfStoreByID([FromHeader] string authorization, int id)
        //{
        //    var result = await _service.GetProductOfStoreByID(authorization.Substring(7), id);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Update Product information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdateProduct(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _service.DeleteProduct(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        /// <summary>
        /// Get list product fit with customer
        /// </summary>
        /// <remarks>
        /// Sau khi đăng nhập thành công thì gọi api này để lấy danh sách product phù hợp, còn muốn search thì gọi api khác
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("fit-with-customer")]
        public async Task<IActionResult> GetListProductFitWithCustomer([FromHeader] string authorization, [FromQuery] QueryStringParameters param)
        {
            var result = await _service.GetListProductFitWithCustomer(authorization.Substring(7), param);
            return Ok(result);
        }

        /// <summary>
        /// Get quantity of product
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpGet("quantity")]
        //public async Task<IActionResult> GetQuantityOfProduct([FromQuery] GetQuantityOfProductParameter param)
        //{
        //    var result = await _service.GetQuantityOfProduct(param);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Staff and manager get quantity of product in store
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("quantity-of-store")]
        public async Task<IActionResult> GetQuantityOfProductInStore([FromHeader] string authorization, [FromQuery] GetQuantityOfProductInStoreParameter param)
        {
            var result = await _service.GetQuantityOfProductInStore(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all colour of product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("colours/{id}")]
        public async Task<IActionResult> GetAllColourOfProduct(int id)
        {
            var result = await _service.GetAllColourOfProduct(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// get list sizes of product based on colour
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="colourId"></param>
        /// <returns></returns>
        [HttpGet("sizes/{productId}/{colourId}")]
        public async Task<IActionResult> GetAllSizeOfProduct(int productId, int colourId)
        {
            var result = await _service.GetAllSizeOfProduct(productId, colourId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get top 10 best seller products of previous month
        /// </summary>
        /// <returns></returns>
        [HttpGet("best-seller")]
        public async Task<IActionResult> GetTopTenBestSeller()
        {
            var result = await _service.GetTopTenBestSeller();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("get-quantity-fit-product-by-category")]
        public async Task<IActionResult> GetQuantityFitProductByCategory([FromHeader] string authorization)
        {
            var result = await _service.GetQuantityFitProductByCategory(authorization.Substring(7));
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
