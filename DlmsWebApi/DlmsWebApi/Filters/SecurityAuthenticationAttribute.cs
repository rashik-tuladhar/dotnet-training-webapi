using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SecurityAuthenticationAttribute : TypeFilterAttribute
    {
        public SecurityAuthenticationAttribute(string realm = null) : base(typeof(SecurityAuthenticationFilter))
        {
            Arguments = new object[]
            {
                realm
            };
        }
    }
}
