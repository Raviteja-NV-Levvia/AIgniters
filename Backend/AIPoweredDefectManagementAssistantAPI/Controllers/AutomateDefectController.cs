using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIPoweredDefectManagementAssistantAPI.Controllers
{
    [ApiController]
    public class AutomateDefectController : BaseController
    {
        // GET api/AutomateDefect
        [HttpGet("sample")]
        public ActionResult GetAll()
        {
            return Ok("Sample API Testing");
        }
    }
}
