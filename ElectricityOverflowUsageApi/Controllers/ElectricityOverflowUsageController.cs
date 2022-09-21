using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ElectricityOverflowUsageController : ControllerBase {

        [HttpGet]
        public IActionResult GetElectricityOverflowUsage() {
            //ToDo: Methode aus dem ServiceProjekt aufrufen, umwandeln in JSON
            var todo = "test";
            return Ok(new JsonResult(todo));
        }
    }
}
