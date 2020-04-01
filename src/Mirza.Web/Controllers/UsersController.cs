using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mirza.Web.Models;

namespace Mirza.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MirzaUser>>> List()
        {
            return await Task.FromResult(StatusCode(501, "Not Implemented")).ConfigureAwait(false);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MirzaUser>> Detail(int id)
        {
            return await Task.FromResult(StatusCode(501, "Not Implemented")).ConfigureAwait(false);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, MirzaUser user)
        {
            return await Task.FromResult(StatusCode(501, "Not Implemented")).ConfigureAwait(false);
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MirzaUser>> PostUser(MirzaUser user)
        {
            return await Task.FromResult(StatusCode(501, "Not Implemented")).ConfigureAwait(false);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MirzaUser>> DeleteUser(int id)
        {
            return await Task.FromResult(StatusCode(501, "Not Implemented")).ConfigureAwait(false);
        }
    }
}
