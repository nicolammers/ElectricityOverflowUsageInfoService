using ElectricityOverflowUsageApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/windPhotovoltaikGenerationPercent")]
    public class WindPhotovoltaikController : ControllerBase {
        [HttpGet]
        public IActionResult GetGreenEnergy() {
            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(ServiceDataChache.WindPhotovoltaikGenerationPercent));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
