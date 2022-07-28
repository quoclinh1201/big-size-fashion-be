using BigSizeFashion.Business.Dtos.Parameters;
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
    [Route("api/v1/delivery-notes")]
    [ApiController]
    public class DeliveryNotesController : ControllerBase
    {
        private readonly IDeliveryNoteService _service;

        public DeliveryNotesController(IDeliveryNoteService service)
        {
            _service = service;
        }

        /// <summary>
        /// Manager get list request import product
        /// </summary>
        /// <remarks>
        /// - Bên nào yêu cầu nhập hàng thì gọi api này
        /// - Status = 0 => cancel
        /// - Status = 1 => pending
        /// - status = 2 => approved
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("import-list")]
        public async Task<IActionResult> GetListRequestImportProduct([FromHeader] string authorization, [FromQuery] ImportProductParameter param)
        {
            var result = await _service.GetListRequestImportProduct(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Manager get list export
        /// </summary>
        /// <remarks>
        /// - Bên xuất hàng gọi api này
        /// - Status = 0 => cancel
        /// - Status = 1 => pending
        /// - status = 2 => approved
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("export-list")]
        public async Task<IActionResult> GetListExportProduct([FromHeader] string authorization, [FromQuery] ImportProductParameter param)
        {
            var result = await _service.GetListExportProduct(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get Delivery Note Detail by id
        /// </summary>
        /// <remarks>
        /// - ReceiveStaff là người nhận hàng (đứa request) 
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryNoteDetail(int id)
        {
            var result = await _service.GetDeliveryNoteDetail(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create request import product
        /// </summary>
        /// <remarks>
        /// - FromStoreId là id của store giao hàng
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRequestImportProduct([FromHeader] string authorization, [FromBody] ImportProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _service.CreateRequestImportProduct(authorization.Substring(7), request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Approve Request Import Product
        /// </summary>
        /// <remarks>
        /// - id là id của delivery note
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveRequestImportProduct(int id)
        {
            var result = await _service.ApproveRequestImportProduct(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Reject Request Import Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectRequestImportProduct(int id)
        {
            var result = await _service.RejectRequestImportProduct( id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get list export product for main warehouse (In admin page)
        /// </summary>
        /// <remarks>
        /// - Status = 0 => cancel
        /// - Status = 1 => pending
        /// - status = 2 => approved
        /// </remarks>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("export-list-for-main-warehouse")]
        public async Task<IActionResult> GetListExportProductForMainWarehouse([FromHeader] string authorization, [FromQuery] ImportProductParameter param)
        {
            var result = await _service.GetListExportProductForMainWarehouse(authorization.Substring(7), param);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
