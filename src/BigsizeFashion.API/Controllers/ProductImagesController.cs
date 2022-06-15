using BigSizeFashion.Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigsizeFashion.API.Controllers
{
    [Route("api/product-images")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImagesController(IProductImageService service)
        {
            _service = service;
        }

        /// <summary>
        /// Add images for product
        /// </summary>
        /// <remarks>
        /// - Nếu sản phẩm chưa có bắt ảnh nào. Thì khi upload list ảnh, ảnh đầu tiên sẽ là ảnh main cho sản phẩm đó, những ảnh khác sẽ là ảnh phụ
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("add-image/{id}")]
        public async Task<IActionResult> AddImagesForProduct(int id, IFormFileCollection files)
        {
            var result = await _service.AddImagesForProduct(id, files);
            return Ok(result);
        }
    }
}
