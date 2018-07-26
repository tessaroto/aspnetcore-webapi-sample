using System;
using System.Linq;
using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleWebApiAspNetCore.Repositories;
using System.Collections.Generic;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Models;
using SampleWebApiAspNetCore.Helpers;

namespace SampleWebApiAspNetCore.v1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class HealthcheckController : ControllerBase
    {
        [HttpGet(Name = nameof(GetAllChecks))]
        public ActionResult GetAllChecks()
        {

            string version = System.IO.File.ReadAllText("build_version");
           
            var hc = new
            {
                result = true,
                version = version,
                checks = new []
                {
                    new {
                        name = "Sql Server",
                        status= true,
                        optional = false,
                        description = ""  
                    },
                    new {
                        name = "Elastic Search",
                        status= false,
                        optional = true,
                        description = ""  
                    }
                }
            };

            return Ok(hc);
        }

    }
}
