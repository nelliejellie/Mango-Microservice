using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService,IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View(orderId);
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("CheckOut")]    
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.Email = cartDto.CartHeader.Email;

            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            if (response != null && response.Success)
            {
                PaystackRequestDto paystackRequestDto = new PaystackRequestDto();
                paystackRequestDto.OrderHeader.OrderTotal = orderHeaderDto.OrderTotal * 100; // Paystack expects amount in kobo
                paystackRequestDto.OrderHeader.UserId = orderHeaderDto.UserId;
                paystackRequestDto.OrderHeader.Email = orderHeaderDto.Email;

                ResponseDto? paystackResponse = await _orderService.CreatePaystackSession(paystackRequestDto);
                PaystackResponseDto Dto = JsonConvert.DeserializeObject<PaystackResponseDto>(Convert.ToString(response.Result));
                if (paystackResponse != null && paystackResponse.Success)
                {
                    return Redirect(Convert.ToString(Dto.PaystackDataDto.Authorization_Url));
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null & response.Success)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            if(response != null && response.Success)
            {
                //implement paystack integration here
            }
            return View();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null & response.Success)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {

            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null & response.Success)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            var response = await _cartService.EmailCart(cartDto);
            if(response != null && response.Success)
            {
                TempData["success"] = "Cart emailed successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDto? response = await _cartService.GetCartByUserIdAsnyc(userId);
            if (response != null & response.Success)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }
    }
}
