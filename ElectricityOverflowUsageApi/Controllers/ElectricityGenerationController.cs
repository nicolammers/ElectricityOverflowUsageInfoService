using ElectricityOverflowUsageApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/elecGeneration")]
    public class ElectricityGenerationController : ControllerBase {

        [HttpGet]
        public IActionResult GetElectricityGeneration() {
            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(ServiceDataChache.ElectricityGeneration));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
