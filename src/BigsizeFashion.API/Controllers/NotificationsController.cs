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
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get list notifications (order by create date)
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllNotifications([FromHeader] string authorization, QueryStringParameters param)
        {
            var result = await _service.GetAllNotifications(authorization.Substring(7), param);
            return Ok(result);
        }

        /// <summary>
        /// Get notification by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}", Name = "GetNotificationByID")]
        public async Task<IActionResult> GetNotificationByID(int id)
        {
            var result = await _service.GetNotificationByID(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var result = await _service.DeleteNotification(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
