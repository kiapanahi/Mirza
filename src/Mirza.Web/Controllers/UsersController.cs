using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mirza.Web.Models;
using Mirza.Web.Services.User;

namespace Mirza.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json", "text/plain", "text/html")]
    [Consumes("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MirzaUser>>> List() => await Task.FromResult(StatusCode(501, "Not Implemented")).ConfigureAwait(false);


        [HttpGet("{id}")]
        public async Task<ActionResult<MirzaUser>> Detail(int id)
        {
            try
            {
                var user = await _userService.GetUser(id).ConfigureAwait(false);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.IsActive
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(500, $"Internal Error. LogId: {HttpContext.TraceIdentifier}");
            }
        }

        [HttpGet("detail/{accessKey}")]
        public async Task<ActionResult<MirzaUser>> GetUserByAccessKey(string accessKey)
        {
            try
            {
                var user = await _userService.GetUserWithActiveAccessKey(accessKey).ConfigureAwait(false);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    user.FirstName,
                    user.LastName,
                    user.Email
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(500, $"Internal Error. LogId: {HttpContext.TraceIdentifier}");
            }
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MirzaUser>> PostUser(MirzaUser user)
        {
            try
            {
                var registeredUser = await _userService.Register(user).ConfigureAwait(false);
                return CreatedAtAction(nameof(Detail), new { id = registeredUser.Id }, null);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Input was null");
            }
            catch (UserModelValidationException e)
            {
                return BadRequest(e.ValidationErrors);
            }
            catch (DuplicateEmailException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(500, $"Internal Error. LogId: {HttpContext.TraceIdentifier}");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MirzaUser>> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUser(id).ConfigureAwait(false);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(500, $"Internal Error. LogId: {HttpContext.TraceIdentifier}");
            }
        }
    }
}
