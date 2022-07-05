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
    [Route("api/v1/store-warehouses")]
    [ApiController]
    public class StoreWarehousesController : ControllerBase
    {
        private readonly IStoreWarehouseService _service;

        public StoreWarehousesController(IStoreWarehouseService service)
        {
            _service = service;
        }

        /// <summary>
        /// Manager increase or descrease product in warehouse
        /// </summary>
        /// <remarks>
        /// Action = true => increase
        /// Action = false => descrease
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> IncreaseOrDesceaseProductInStore([FromHeader] string authorization, [FromBody] IncreaseOrDesceaseProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.IncreaseOrDesceaseProductInStore(authorization.Substring(7), request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Kiểm kê kho
        /// </summary>
        /// <remarks>
        /// - Chọn từ ngày ... đến ngày ...
        /// - Chọn product, chọn size, colour, nhập số lượng thực tế có trong cửa hàng
        /// - Kq: số lượng đầu kỳ, số lượng cuối kỳ trên hệ thống, chênh lệch
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckWarehouse([FromHeader] string authorization, [FromBody] CheckWarehouseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CheckWarehouse(authorization.Substring(7), request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Điều chỉnh số lượng trong kho sau khi kiểm kê
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut]
        public async Task<IActionResult> QuantityAdjustment([FromHeader] string authorization, [FromBody] List<QuantityAdjustmentRequest> request)
        {
            var result = await _service.QuantityAdjustment(authorization.Substring(7), request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
