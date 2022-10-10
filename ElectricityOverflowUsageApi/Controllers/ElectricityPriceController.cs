using ElectricityOverflowUsageApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/elecPrices")]
    public class ElectricityPriceController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> GetElectricityUsageAsync() {

            var smardApiReader = new ElectricityOverflowUsageInfoService.SmardApi.SmardApiReader();
            var elecPriceServ = new ElectricityPriceService(smardApiReader);
            
            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(await elecPriceServ.GetElectricityPricesAsync()));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
