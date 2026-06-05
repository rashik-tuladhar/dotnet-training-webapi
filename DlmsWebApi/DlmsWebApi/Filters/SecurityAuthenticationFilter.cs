using System.Text;
using DlmsWebApi.Extensions.BasicAuthentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DlmsWebApi.Filters
{
    public class SecurityAuthenticationFilter : IAuthorizationFilter
    {
        private readonly IBasicAuthService _authService;
        private readonly string _realm;

        //private readonly IAuthorizationBusiness _authorizationBusiness;
        private readonly IConfiguration _configuration;
        public SecurityAuthenticationFilter(IConfiguration configuration, IBasicAuthService authService, string realm = null)
        {
            //_authorizationBusiness = authorizationBusiness;
            _configuration = configuration;
            _authService = authService;
            this._realm = realm;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                // Decode from Base64 to string
                if (encodedUsernamePassword != null)
                {
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    // Split username and password
                    var username = decodedUsernamePassword.Split(':', 2)[0];
                    var password = decodedUsernamePassword.Split(':', 2)[1];
                    // Check if login is correct
                    var result = IsAuthorized(username, password).GetAwaiter().GetResult();
                    if (result)
                    {
                        return;
                    }
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            // Return authentication type (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
            // Add realm if it is not null
            if (!string.IsNullOrWhiteSpace(_realm))
            {
                context.HttpContext.Response.Headers["WWW-Authenticate"] += $" realm=\"{_realm}\"";
            }
            // Return unauthorized
            context.Result = new UnauthorizedResult();
        }
        // Make your own implementation of this
        public async Task<bool> IsAuthorized(string username, string password)
        {
            var response =  await _authService.AuthenticateAsync(username, password);
            return response != null;
            var authenticationUsername = _configuration["BasicCredentials:Username"];
            var authenticationPassword = _configuration["BasicCredentials:Password"];
            return username == authenticationUsername && password == authenticationPassword;
        }
    }
}
