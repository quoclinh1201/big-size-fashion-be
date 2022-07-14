using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.RequestObjects;
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
    [Route("api/v1/promotion-details")]
    [ApiController]
    public class PromotionDetailsController : ControllerBase
    {
        private readonly IPromotionDetailService _service;

        public PromotionDetailsController(IPromotionDetailService service)
        {
            _service = service;
        }

        /// <summary>
        /// Add list product into promotion
        /// </summary>
        /// <remarks>
        /// - trả về 200 và content = [] là add thành công
        /// - content != null => danh sách sản phẩm bị trùng ngày
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProductsIntoPromotion([FromBody] AddProductsIntoPromotionRequest request)
        {
            var result = await _service.AddProductsIntoPromotion(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Remove product out of promotion
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveProductOutOfPromotion([FromBody] RemoveProductOutOfPromotionnRequest request)
        {
            var result = await _service.RemoveProductOutOfPromotion(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all product in promotion
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetListProductInPromotion([FromQuery] GetListProductInPromotionParameter param)
        {
            var result = await _service.GetListProductInPromotion(param);
            return Ok(result);
        }
    }
}
