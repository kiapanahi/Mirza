using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirza.Web.Services.User;

namespace Mirza.Web.Auth
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AccessKeyAuthenticationHandler : AuthenticationHandler<AccessKeyAuthenticationOptions>
    {
        private const string AuthorizationHeaderKey = "Authorization";
        private const string AuthenticationScheme = "AccessKey";
        private readonly IUserService _userService;

        public AccessKeyAuthenticationHandler(IOptionsMonitor<AccessKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, IUserService userService) : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderKey))
            {
                return AuthenticateResult.Fail("Authorization header not found");
            }

            var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers[AuthorizationHeaderKey]);
            if (authenticationHeaderValue.Scheme != AuthenticationScheme)
            {
                return AuthenticateResult.NoResult();
            }

            var accessKey = authenticationHeaderValue.Parameter;
            if (string.IsNullOrWhiteSpace(accessKey))
            {
                return AuthenticateResult.Fail("Empty access key was provided");
            }

            var user = await _userService.GetUserWithActiveAccessKey(accessKey).ConfigureAwait(false);

            if (user == null)
            {
                return AuthenticateResult.Fail($"Failed to retrieve user with active access key \"{accessKey}\"");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(MirzaClaimTypes.Team, user.Team.Id.ToString(CultureInfo.InvariantCulture)),

                // TODO: read tenant id form user object
                new Claim(MirzaClaimTypes.Tenant, "1")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}