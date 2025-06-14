using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Models.Utilities;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult OrderIndex()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeaderDto> list;
            string userId = "";
            if (!User.IsInRole("Admin"))
            {
                userId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value;
            }
            ResponseDto response = _orderService.GetOrders(userId).Result;
            if (response != null && response.Success)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Result));
            }
            else
            {
                list = new List<OrderHeaderDto>();
            }

            return Json(new { data = list });
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeader = new OrderHeaderDto();
            string userId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value;

            var response = await _orderService.GetOrderById(orderId);
            if (response != null && response.Success)
            {
                orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            }
            if (!User.IsInRole("ADMIN") && orderHeader.UserId != userId)
            {
                return NotFound();
            }
            return View(orderHeader);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId.ToString(), StaticDetails.Status_ReadyForPickup);
            if (response != null && response.Success)
            {
                TempData["success"] = "Order marked as ready for pickup successfully.";
                return RedirectToAction("OrderDetail", new { orderId = orderId });
            }
            return View();

        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId.ToString(), StaticDetails.Status_Completed);
            if (response != null && response.Success)
            {
                TempData["success"] = "status updated successfully.";
                return RedirectToAction("OrderDetail", new { orderId = orderId });
            }
            return View();

        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId.ToString(), StaticDetails.Status_Cancelled);
            if (response != null && response.Success)
            {
                TempData["success"] = "status updated successfully.";
                return RedirectToAction("OrderDetail", new { orderId = orderId });
            }
            return View();

        }

    }
}
