namespace DlmsWebApi.Extensions.BasicAuthentication;

public interface IBasicAuthService
{
    Task<bool> AuthenticateAsync(string username, string password);
}