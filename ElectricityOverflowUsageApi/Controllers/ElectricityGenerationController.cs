using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/elecGeneration")]
    public class ElectricityGenerationController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> GetElectricityUsageAsync() {

            var smardApiReader = new ElectricityOverflowUsageInfoService.SmardApi.SmardApiReader();
            var elecGenServ = new ElectricityOverflowUsageInfoService.Services.ElectricityGenerationService(smardApiReader);

            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(await elecGenServ.GetTotalElectricityGenerationAsync()));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
