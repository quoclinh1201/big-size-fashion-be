using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Parameters;
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
        /// ---------------------------------
        /// - OrderByCreateDate = true hoặc null => đơn hàng mới nhắt nằm trên
        /// - OrderByCreateDate = false => đơn hàng cũ nhắt nằm trên
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("customer")]
        public async Task<IActionResult> GetListOrderForCustomer([FromHeader] string authorization, [FromQuery] FilterOrderParameter param)
        {
            var result = await _service.GetListOrderForCustomer(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Manager get list orders of store
        /// </summary>
        /// <remarks>
        /// - Order type = null => get mọi type
        /// - Order type = true => get đơn online
        /// - Order type = false => get đơn offlice
        /// ---------------------------------
        /// - OrderByCreateDate = true hoặc null => đơn hàng mới nhắt nằm trên
        /// - OrderByCreateDate = false => đơn hàng cũ nhắt nằm trên
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("for-manager")]
        public async Task<IActionResult> GetListOrderOfStoreForManager([FromHeader] string authorization, [FromQuery] FilterOrderParameter param)
        {
            var result = await _service.GetListOrderOfStoreForManager(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Manager approve order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            var result = await _service.ApproveOrder(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Manager assign order to staff
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("assign-order")]
        public async Task<IActionResult> AssignOrder([FromBody] AssignOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.AssignOrder(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get list assign order for staff
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("assigned-order")]
        public async Task<IActionResult> GetListAssignedOrder([FromHeader] string authorization, [FromQuery] QueryStringParameters param)
        {
            var result = await _service.GetListAssignedOrder(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get list order of staff
        /// </summary>
        /// <remarks>
        /// - Order type = null => get mọi type
        /// - Order type = true => get đơn online
        /// - Order type = false => get đơn offlice
        /// ---------------------------------
        /// - OrderByCreateDate = true hoặc null => đơn hàng mới nhắt nằm trên
        /// - OrderByCreateDate = false => đơn hàng cũ nhắt nằm trên
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("for-staff")]
        public async Task<IActionResult> GetListOrderOfStoreForStaff([FromHeader] string authorization, [FromQuery] FilterOrderParameter param)
        {
            var result = await _service.GetListOrderOfStoreForStaff(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Staff update order status to packaged
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("packaged/{id}")]
        public async Task<IActionResult> PackagedOrder(int id)
        {
            var result = await _service.PackagedOrder(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Staff update order status to delivery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("delivery/{id}")]
        public async Task<IActionResult> DeliveryOrder(int id)
        {
            var result = await _service.DeliveryOrder(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Update status to received
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("received/{id}")]
        public async Task<IActionResult> ReceivedOrder(int id)
        {
            var result = await _service.ReceivedOrder(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Manager reject order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectOrder(int id)
        {
            var result = await _service.RejectOrder(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <remarks>
        /// customer chỉ hủy được những đơn hàng có trạng thái là "Chờ xác nhận", manager thì full quyền
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder([FromHeader] string authorization, int id)
        {
            var result = await _service.CancelOrder(authorization.Substring(7), id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("add-order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request, [FromHeader] string authorization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _service.AddOrder(authorization.Substring(7), request);

           
            return Ok(result);

        }

        /// <summary>
        /// Get revenue of own store (for manager)
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueOfOwnStore([FromHeader] string authorization, [FromQuery] GetRevenueParameter param)
        {
            var result = await _service.GetRevenueOfOwnStore(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get revenue by store Id (for owner)
        /// </summary>
        /// <remarks>
        /// - Id = 0 => lấy doanh thu của cả chuỗi
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("revenue/{id}")]
        public async Task<IActionResult> GetRevenueByStoreId(int id, [FromQuery] GetRevenueParameter param)
        {
            var result = await _service.GetRevenueByStoreId(id, param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
