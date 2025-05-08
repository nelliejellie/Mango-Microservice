using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Models;
using System.Threading.Tasks;
using Mango.Services.AuthApi.Services.IService;

namespace Mango.Services.AuthApi.Services.IService
{
    public interface IJWTTokenGenerator
    {
        string GenerateToken(ApplicationUser user);
    }
}