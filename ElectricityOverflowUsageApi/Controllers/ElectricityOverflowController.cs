using ElectricityOverflowUsageApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/elecOverflow")]
    public class ElectricityOverflowController : ControllerBase {

        [HttpGet]
        public IActionResult GetElectricityOverflow() {
            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(ServiceDataChache.ElectricityOverflow));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
