﻿using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.IServices.ZaloPay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigsizeFashion.API.Controllers
{
    [Route("api/v1/zalo-pay")]
    [ApiController]
    public class ZaloPayController : ControllerBase
    {
        private readonly IZaloPayService _service;

        public ZaloPayController(IZaloPayService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create order from zalo pay
        /// </summary>
        /// <remarks>
        /// - truyền order id vào
        /// - Dùng Order url để redirect sang app Zalo pay để thanh toán
        /// - Vào https://docs.zalopay.vn/v2/start/#A-I để tải app sandbox để test
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> CreateOrder(int id)
        {
            var result = await _service.CreateOrder(id);
            if(!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create order from zalo pay with money
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("money")]
        public async Task<IActionResult> CreateOrderWithMoney([FromBody] CreateOrderWithMoneyRequest request)
        {
            var result = await _service.CreateOrderWithMoney(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //[HttpGet("test/{id}")]
        //public async Task<IActionResult> CreateOrderString(int id)
        //{
        //    var result = await _service.CreateOrderString(id);
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}
    }
}
