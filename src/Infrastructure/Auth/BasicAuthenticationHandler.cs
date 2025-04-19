using System.Text;
using Infrastructure.Config;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;

namespace Infrastructure.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly BasicAuthSettings _settings;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptions<BasicAuthSettings> settings)
            : base(options, logger, encoder, clock)
        {
            _settings = settings.Value;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

            try
            {
                AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                byte[] credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? "");
                string[] credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                string username = credentials[0];
                string password = credentials[1];

                if (username != _settings.Username || password != _settings.Password)
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));

                Claim[] claims = {
                new Claim(ClaimTypes.Name, username)
            };
                ClaimsIdentity identity = new(claims, Scheme.Name);
                ClaimsPrincipal principal = new(identity);
                AuthenticationTicket ticket = new(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }
    }
}
