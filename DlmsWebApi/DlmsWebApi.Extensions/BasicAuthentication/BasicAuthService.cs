namespace DlmsWebApi.Extensions.BasicAuthentication;

public class BasicAuthService : IBasicAuthService
{
    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        if(username == "admin" && password == "password")
        {
            return true;
        }
        return false;
    }
}
