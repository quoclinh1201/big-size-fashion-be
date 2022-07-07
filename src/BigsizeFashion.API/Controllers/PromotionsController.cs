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
    [Route("api/v1/promotions")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _service;

        public PromotionsController(IPromotionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all promotions
        /// </summary>
        /// <remarks>
        /// - Không nhập OrderByApplyDate hoặc OrderByApplyDate = null => Không sếp theo thứ tự
        /// - OrderByExpiredDate = true => giảm dần
        /// - OrderByExpiredDate = false => tăng dần
        /// -------------------------
        /// - Status = null => get all
        /// </remarks>
        /// <param name="param"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllPromotion([FromQuery] SearchPromotionParameter param)
        {
            var result = await _service.GetAllPromotion(param);
            return Ok(result);
        }

        /// <summary>
        /// Get promotion by ID
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}", Name = "GetPromotionByID")]
        public async Task<IActionResult> GetPromotionByID(int id)
        {
            var result = await _service.GetPromotionByID(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create new promotion
        /// </summary>
        /// <remarks>
        /// - Promotion value là % giảm giá. Range từ 1 -> 100
        /// </remarks>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] PromotionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreatePromotion(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return CreatedAtRoute(nameof(GetPromotionByID), new { id = result.Content.PromotionId }, result);
        }

        //[Authorize]
        //[HttpPost("validate")]
        //public async Task<IActionResult> ValidateTimeOfPromotion([FromBody] ValidateTimeOfPromotionRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var result = await _service.ValidateTimeOfPromotion(request);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Update promotion
        /// </summary>
        /// <remarks>
        /// - Promotion value là % giảm giá. Range từ 1 -> 100
        /// </remarks>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] PromotionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.UpdatePromotion(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete promotion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var result = await _service.DeletePromotion(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
