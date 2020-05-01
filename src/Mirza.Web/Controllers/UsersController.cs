using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mirza.Web.Dto;
using Mirza.Web.Models;
using Mirza.Web.Services.User;

namespace Mirza.Web.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "AccessKey,Identity.Application")]
    [Route("api/[controller]")]
    [Produces("application/json", "text/plain", "text/html")]
    [Consumes("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MirzaUser>>> List()
        {
            return await Task.FromResult(StatusCode(501, "Not Implemented")).ConfigureAwait(false);
        }


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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [HttpPost("access-key/generate")]
        public async Task<ActionResult<AccessKey>> GenerateAccessKey()
        {
            var userIdClaimValue = User.Claims
                                       .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "-1";

            if (int.TryParse(userIdClaimValue, out var userId))
            {
                try
                {
                    var newAccessKey = await _userService.AddAccessKey(userId).ConfigureAwait(false);
                    return Ok(new
                    {
                        newAccessKey.Expiration,
                        newAccessKey.Key,
                        newAccessKey.State,
                        newAccessKey.IsActive
                    });
                }
                catch (AccessKeyException e)
                {
                    return StatusCode(500, new
                    {
                        ErrorMessage = "Error while generating new access key",
                        ErrorDetail = e.Message
                    });
                }
                catch (Exception)
                {
                    return StatusCode(500, new
                    {
                        ErrorMessage = "Error while generating new access key",
                        ErrorDetail = $"Internal Error. LogId: {HttpContext.TraceIdentifier}"
                    });
                }
            }

            return BadRequest(new { ErrorMessage = "Could not acquire user id from request" });
        }

        [HttpDelete("access-key/{accessKey}")]
        public async Task<ActionResult<AccessKey>> DeactivateAccessKey(string accessKey)
        {
            var userIdClaimValue = User.Claims
                                       .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "-1";

            if (int.TryParse(userIdClaimValue, out var userId))
            {
                try
                {
                    var deactivatedAccessKey =
                        await _userService.DeactivateAccessKey(userId, accessKey).ConfigureAwait(false);
                    return Ok(new
                    {
                        deactivatedAccessKey.Expiration,
                        deactivatedAccessKey.Key,
                        deactivatedAccessKey.State,
                        deactivatedAccessKey.IsActive
                    });
                }
                catch (AccessKeyException e)
                {
                    return StatusCode(500, new
                    {
                        ErrorMessage = "Error while deactivating access key",
                        ErrorDetail = e.Message
                    });
                }
                catch (Exception)
                {
                    return StatusCode(500, new
                    {
                        ErrorMessage = "Error while generating new access key",
                        ErrorDetail = $"Internal Error. LogId: {HttpContext.TraceIdentifier}"
                    });
                }
            }

            return BadRequest(new { ErrorMessage = "Could not acquire user id from request" });
        }

        [HttpPost("worklog")]
        public async Task<ActionResult<WorkLog>> AddWorkLog(AddWorkLogInput input)
        {
            if (input == null)
            {
                return BadRequest(new { ErrorMessage = "Input was null" });
            }

            if (!TimeSpan.TryParse(input.Start, out var startTime))
            {
                return BadRequest(new { ErrorMessage = "Invalid format for 'start' field" });
            }

            if (!TimeSpan.TryParse(input.End, out var endTime))
            {
                return BadRequest(new { ErrorMessage = "Invalid format for 'end' field" });
            }

            var userIdClaimValue = User.Claims
                                       .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "-1";

            if (!int.TryParse(userIdClaimValue, out var userId))
            {
                return BadRequest(new { ErrorMessage = "Could not acquire user id from request" });
            }

            try
            {
                var model = new WorkLog
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    Description = input.Description ?? "-",
                    Details = input.Detail ?? "-",
                    EntryDate = DateTime.Today.Date,
                    Tags = input.Tags?.Select(t => new Tag(t)).ToArray() ?? Array.Empty<Tag>()
                };
                var log = await _userService.AddWorkLog(userId, model)
                                            .ConfigureAwait(false);
                return Ok(new { log.Id });
            }
            catch (ArgumentNullException)
            {
                return BadRequest(new { ErrorMessage = "Input was null" });
            }
            catch (WorkLogModelValidationException e)
            {
                return BadRequest(new
                {
                    ErrorMessage = "Input validation failed",
                    ErrorDetails = e.ValidationErrors
                });
            }
            catch (ArgumentException)
            {
                return BadRequest(new { ErrorMessage = "Invalid user" });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { ErrorMessage = "Overlapping times!" });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ErrorMessage = "Error while adding work log",
                    ErrorDetail = $"Internal Error. LogId: {HttpContext.TraceIdentifier}"
                });
            }
        }

        [HttpGet("worklog")]
        public async Task<ActionResult<WorkLogReportOutput>> GetWorkLogReport([FromQuery(Name = "date")] DateTime date)
        {
            var workLogDate = date == default ? DateTime.Today.Date : date;

            var userIdClaimValue = User.Claims
                                       .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "-1";

            if (!int.TryParse(userIdClaimValue, out var userId))
            {
                return BadRequest(new { ErrorMessage = "Could not acquire user id from request" });
            }

            try
            {
                var report = await _userService.GetWorkLogReport(userId, workLogDate)
                                               .ConfigureAwait(false);

                return Ok(report);
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    ErrorMessage = "Error while compiling work log report",
                    ErrorDetail = $"Internal Error. LogId: {HttpContext.TraceIdentifier}"
                });
            }
        }

        [HttpDelete("worklog/{id}")]
        public async Task<ActionResult<WorkLog>> DeleteWorkLog(int id)
        {
            var userIdClaimValue = User.Claims
                                       .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "-1";

            if (!int.TryParse(userIdClaimValue, out var userId))
            {
                return BadRequest(new { ErrorMessage = "Could not acquire user id from request" });
            }

            try
            {
                var log = await _userService.DeleteWorkLog(userId, id)
                                            .ConfigureAwait(false);
                return Ok(new { log.Id });
            }
            catch (ArgumentException e) when (e.Message.Contains("Invalid userId", StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest(new { ErrorMessage = "Invalid user id" });
            }
            catch (ArgumentException e) when (e.Message.Contains("Work log not found", StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest(new { ErrorMessage = $"Worklog Id {id} not found for user {userId}" });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    ErrorMessage = "Error while deleting work log",
                    ErrorDetail = $"Internal Error. LogId: {HttpContext.TraceIdentifier}"
                });
            }
        }
    }
}