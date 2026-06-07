using System.Text;
using DlmsWebApi.Extensions.BasicAuthentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DlmsWebApi.Filters;

public class BasicAuthFilter : IAsyncAuthorizationFilter
{
    private readonly IBasicAuthService _authService;

    public BasicAuthFilter(IBasicAuthService authService)
    {
        _authService = authService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authHeader) ||
            !authHeader.StartsWith("Basic "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var encodedCredentials = authHeader["Basic ".Length..];
        var credentials = Encoding.UTF8.GetString(
            Convert.FromBase64String(encodedCredentials));

        var parts = credentials.Split(':', 2);

        if (parts.Length != 2)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var username = parts[0];
        var password = parts[1];

        var user = await _authService.AuthenticateAsync(username, password);

        if (user == null)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}