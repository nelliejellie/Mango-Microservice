using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface IBaseService
    {
        Task<ResponseDto> SendAsync(RequestDto request, bool withBearer = true);
    }
}