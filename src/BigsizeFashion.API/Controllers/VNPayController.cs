using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.IServices.VNPay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigsizeFashion.API.Controllers
{
    [Route("api/v1/vnpay")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IVNPayService _service;

        public VNPayController(IVNPayService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get payment url (for customer)
        /// </summary>
        /// <remarks>
        /// - id là id của order cần thanh toán
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLink(int id)
        {
            var clientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _service.GetLink(id, clientIPAddr);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// asdasdas
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("IPN")]
        public async Task<IActionResult> GetIPNResponse([FromQuery] VNPayParameter param)
        {
            var result = await _service.GetIPNResponse(param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
