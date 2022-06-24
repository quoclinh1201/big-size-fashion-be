using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigsizeFashion.API.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        /// <summary>
        /// Staff create order for customer (offline shop)
        /// </summary>
        /// <remarks>
        /// Tạo thành công sẽ trả về order id, lấy id đó gọi get order detail để lấy kết quả 
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("create-order-for-customer")]
        public async Task<IActionResult> CreateOrderForCustomer([FromHeader] string authorization, [FromBody] CreateOrderForCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreateOrderForCustomer(authorization.Substring(7), request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get order detail by order id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetOrderDetailById(int id)
        {
            var result = await _service.GetOrderDetailById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Customer get list order
        /// </summary>
        /// <remarks>
        /// - Order type = null => get mọi type
        /// - Order type = true => get đơn online
        /// - Order type = false => get đơn offlice
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpGet("customer")]
        //public async Task<IActionResult> GetListOrderForCustomer([FromHeader] string authorization, [FromQuery] FilterOrderParameter param)
        //{
        //    var result = await _service.GetListOrderForCustomer(authorization.Substring(7), param);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}
    }
}
