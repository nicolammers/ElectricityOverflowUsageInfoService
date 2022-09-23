using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ElectricityOverflowUsageController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> GetElectricityOverflowUsageAsync() {

            var smardApiReader = new ElectricityOverflowUsageInfoService.SmardApi.SmardApiReader();
            var elecGenServ = new ElectricityOverflowUsageInfoService.Services.ElectricityGenerationService(smardApiReader);
            var elecUsageServ = new ElectricityOverflowUsageInfoService.Services.ElectricityUsageService(smardApiReader);
            var elecOverflowServ = new ElectricityOverflowUsageInfoService.Services.ElectricityOverflowService(elecGenServ, elecUsageServ);

            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(await elecOverflowServ.GetElectricityOverflowAsync()));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
