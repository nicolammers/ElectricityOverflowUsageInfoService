using ElectricityOverflowUsageApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/elecUsage")]
    public class ElectricityUsageController : ControllerBase {

        [HttpGet]
        public IActionResult GetElectricityUsage() {
            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(ServiceDataChache.ElectricityUsage));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
