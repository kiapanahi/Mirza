using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Mirza.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult Version()
        {
            var ver = Assembly.GetAssembly(typeof(InfoController))
                .GetName()?.Version?.ToString() ?? "unknown version";
            return Ok(ver);
        }
    }
}