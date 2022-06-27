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
    }
}
