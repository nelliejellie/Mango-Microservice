using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
    public interface IOrderService
    {
        Task<ResponseDto> CreateOrder(CartDto cartDto);
        Task<ResponseDto> CreatePaystackSession(PaystackRequestDto cartDto);
        Task<ResponseDto> PaystackCallback(string referenceId);
        Task<ResponseDto> GetOrderById(int orderId);
        Task<ResponseDto> GetOrders(string userId);
        Task<ResponseDto> UpdateOrderStatus(string orderId, string newStatus);

    }
}
