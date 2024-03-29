﻿using ElectricityOverflowUsageApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace ElectricityOverflowUsageApi.Controllers {
    [ApiController]
    [Route("api/elecPrices")]
    public class ElectricityPriceController : ControllerBase {

        [HttpGet]
        public IActionResult GetElectricityPrices() {
            try {
                return StatusCode((int) HttpStatusCode.OK, new JsonResult(ServiceDataChache.ElectricityPrices));
            } catch (Exception ex) {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
