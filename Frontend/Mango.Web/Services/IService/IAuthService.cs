using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<ResponseDto?> RegisterAsync(RegisterationRequestDto registerRequest);
        Task<ResponseDto?> AssignRole(RoleAssignDto roleAssignDto);
    }
}