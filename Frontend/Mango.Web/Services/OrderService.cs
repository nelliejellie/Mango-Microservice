using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Models.Utilities;
using Mango.Web.Services.IService;
using Microsoft.Extensions.Configuration;

namespace Mango.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        private readonly string OrderApiBase;

        public OrderService(IBaseService baseService, IConfiguration configuration)
        {
            _baseService = baseService;
            OrderApiBase = configuration.GetValue<string>("ServiceUrls:OrderAPI");
        }
        public async Task<ResponseDto> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = OrderApiBase + "/api/order/CreateOrder",
                Data = cartDto
            });
        }

        public async Task<ResponseDto> CreatePaystackSession(PaystackRequestDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = OrderApiBase + "/api/order/createpaystacksession",
                Data = cartDto
            });
        }

        public async Task<ResponseDto> GetOrderById(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = OrderApiBase + "/api/order/GetOrder" + orderId
            });
        }

        public async Task<ResponseDto> GetOrders(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = OrderApiBase + "/api/order/GetOrder?userId=" + userId
            });
        }

        public async Task<ResponseDto> PaystackCallback(string referenceId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = OrderApiBase + "/api/order/payment/callback",
                Data = referenceId
            });
        }

        public async Task<ResponseDto> UpdateOrderStatus(string orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = newStatus,
                Url = OrderApiBase + "/api/order/UpdateOrderStatus/" + orderId
            });
        }
    }
}
