using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features
{
    public class HomeController : ApiController
    {
        [Authorize]
        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Works");
        }
    }
}
