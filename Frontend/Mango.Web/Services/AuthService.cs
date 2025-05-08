using Mango.Web.Services.IService;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Models.Utilities;

namespace Mango.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        private readonly string _authApiBase;

        public AuthService(IBaseService baseService, IConfiguration configuration)
        {
            _baseService = baseService;
            _authApiBase = configuration.GetValue<string>("ServiceUrls:AuthApi");
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var request = new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{_authApiBase}/api/AuthAPI/login",
                Data = loginRequest
            };
            return await _baseService.SendAsync(request);
        }

        public async Task<ResponseDto?> RegisterAsync(RegisterationRequestDto registerRequest)
        {
            var request = new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{_authApiBase}/api/AuthAPI/register",
                Data = registerRequest
            };
            return await _baseService.SendAsync(request);
        }

        public async Task<ResponseDto?> AssignRole(RoleAssignDto roleAssignDto)
        {
            var request = new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{_authApiBase}/api/AuthAPI/assignrole",
                Data = roleAssignDto
            };
            return await _baseService.SendAsync(request);
        }
    }
}