using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/elecUsage")]
    public class ElectricityUsageController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> GetElectricityUsageAsync() {

            var smardApiReader = new ElectricityOverflowUsageInfoService.SmardApi.SmardApiReader();
            var elecUsageServ = new ElectricityOverflowUsageInfoService.Services.ElectricityUsageService(smardApiReader);

            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(await elecUsageServ.GetTotalElectricityUsageAsync()));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
