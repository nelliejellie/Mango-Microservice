using Mango.Web.Models.Utilities;
using Mango.Web.Services.IService;

namespace Mango.Web.Services
{
    public class TokenProviderService : ITokenProviderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenProviderService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? GetToken()
        {
            string? token = null;
            bool? hasToken = _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(Dynamics.TokenCookie, out token);
            return hasToken is true ? token : null;
        }

        public void RemoveToken()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(Dynamics.TokenCookie);
        }

        public void SetToken(string token)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(Dynamics.TokenCookie, token);
        }
    }
}
