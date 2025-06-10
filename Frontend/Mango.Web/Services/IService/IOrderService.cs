using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
    public interface IOrderService
    {
        Task<ResponseDto> CreateOrder(CartDto cartDto);
    }
}
