﻿using BigSizeFashion.Business.Dtos.Parameters;
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
        /// Get list product fit with customer by category
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("fit-with-customer")]
        public async Task<IActionResult> GetListProductFitWithCustomer([FromHeader] string authorization, [FromQuery] GetListProductFitWithCustomerParameters param)
        {
            var result = await _service.GetListProductFitWithCustomer(authorization.Substring(7), param);
            return Ok(result);
        }

        /// <summary>
        /// View detail fit product for customer
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("detail-fit-product/{id}")]
        public async Task<IActionResult> GetDetailFitProductWithCustomer([FromHeader] string authorization, int id)
        {
            var result = await _service.GetDetailFitProductWithCustomer(authorization.Substring(7), id);
            return Ok(result);
        }

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
        /// Staff and manager get quantity of product in store, admin get quantity of product in main warehouse
        /// </summary>
        /// <param name="authorization"></param>
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

        /// <summary>
        /// Get product detail by ID - Hiếu
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("detail/{productId}/{productDetailId}")]
        public async Task<IActionResult> GetProductByDetailID(int productId, int productDetailId)
        {
            var result = await _service.GetProductByDetailID(productId, productDetailId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all product to import
        /// </summary>
        /// <returns></returns>
        [HttpGet("all-product-to-import")]
        public async Task<IActionResult> GetAllProductToImport()
        {
            var result = await _service.GetAllProductToImport();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm để add vào promotion (response là id và product name)
        /// </summary>
        /// <returns></returns>
        [HttpGet("all-product-to-add-into-promotion")]
        public async Task<IActionResult> GetAllProductToAddIntoPromotion()
        {
            var result = await _service.GetAllProductToAddIntoPromotion();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy số lượng của sản phẩm trong toàn bộ chi nhánh (truyền product_detail_id xuống)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("quantity-of-prouduct-in-all-store/{id}")] 
        public async Task<IActionResult> GetQuantityOfProductInAllStore(int id)
        {
            var result = await _service.GetQuantityOfProductInAllStore(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
