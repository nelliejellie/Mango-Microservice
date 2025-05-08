using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Models;
using System.Threading.Tasks;
using Mango.Services.AuthApi.Services.IService;


namespace Mango.Services.AuthApi.Services.IService
{
    
    public interface IAuthService
    {
        Task<ResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<ResponseDto> Register(RegisterationRequestDto registerationRequestDto);
        Task<bool> AssignRole(string email, string rolename);
       
    }
}