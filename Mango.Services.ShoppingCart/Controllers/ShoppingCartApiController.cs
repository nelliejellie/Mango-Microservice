using Azure;
using Mango.MessagePublisher.Services;
using Mango.Services.ShoppingCart.Models;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTOs;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartApiController : ControllerBase
    {
        private ResponseDto _responseDto;
        private readonly ILogger<ShoppingCartApiController> _logger;
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IRabbitPublisher _messageBus;
        private readonly IConfiguration _configuration;

        public ShoppingCartApiController(AppDbContext context, IProductService productService,
            ICouponService couponService, IRabbitPublisher messageBus)
        {
            _context = context;
            _productService = productService;
            _responseDto = new ResponseDto();
            _couponService = couponService;
            _messageBus = messageBus;
        }

        [HttpPost("CartUpsert")]
        public async Task<IActionResult> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    // Create a new CartHeader if it doesn't exist
                    var cartHeader = new CartHeader
                    {
                        UserId = cartDto.CartHeader.UserId,
                        CartTotal = 0
                    };
                    await _context.CartHeaders.AddAsync(cartHeader);
                    await _context.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _context.CartDetails.Add(new CartDetails
                    {
                        CartHeaderId = cartHeader.CartHeaderId,
                        ProductId = cartDto.CartDetails.First().ProductId,
                        Count = cartDto.CartDetails.First().Count
                    });
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //if card is not null
                    // update the existing CartHeader
                    var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb != null)
                    {
                        //create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        var cartDetails = new CartDetails
                        {
                            CartHeaderId = cartHeaderFromDb.CartHeaderId,
                            ProductId = cartDto.CartDetails.First().ProductId,
                            Count = cartDto.CartDetails.First().Count + cartDetailsFromDb.Count
                        };
                        _context.CartDetails.Add(cartDetails);
                    }
                    else
                    {
                        //update count in ncart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        var cartDetails = new CartDetails
                        {
                            CartHeaderId = cartHeaderFromDb.CartHeaderId,
                            ProductId = cartDto.CartDetails.First().ProductId,
                            Count = cartDto.CartDetails.First().Count
                        };
                        _context.CartDetails.Update(cartDetails);
                    }
                }

                _responseDto.Message = "Items collected successfully";
                _responseDto.StatusCode = 200;
                _responseDto.Result = cartDto;
                _responseDto.Success = true;
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _responseDto);
            }
            return Ok(_responseDto);
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _context.CartDetails
                   .First(u => u.CartDetailsId == cartDetailsId);

                int totalCountofCartItem = _context.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _context.CartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders
                       .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _context.SaveChangesAsync();

                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.Success = false;
            }
            return _responseDto;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                var CartHeaders = await _context.CartHeaders
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                var newCartHeaderDto = new CartHeaderDto
                {
                    CartHeaderId = CartHeaders.CartHeaderId,
                    UserId = CartHeaders.UserId,
                    CouponCode = CartHeaders.CouponCode,
                    Discount = 0, // Initialize discount to 0
                    CartTotal = 0 // Initialize cart total to 0
                };
                CartDto cart = new()
                {
                    CartHeader = newCartHeaderDto
                };

                cart.CartDetails = _context.CartDetails
                .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId)
                .Select(cd => new CartDetailsDto
                {
                    // Map properties manually
                    CartHeaderId = cd.CartHeaderId,
                    ProductId = cd.ProductId,
                    CartDetailsId = cd.CartDetailsId
                    // Add more properties as needed
                })
                .ToList();


                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _responseDto.Result = cart;
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.ToString();
            }
            return _responseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.ToString();
            }
            return _responseDto;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<IActionResult> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeader != null)
                {
                    cartDto.CartHeader.CartHeaderId = cartHeader.CartHeaderId;
                    await _messageBus.PublishMessageAsync(cartDto, _configuration.GetValue<string>("Queues:EmailShoppingCartQueue"));
                    _responseDto.Result = true;
                }
                else
                {
                    _responseDto.Success = false;
                    _responseDto.Message = "Cart not found for the user.";
                }
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.ToString();
            }
            return Ok(_responseDto);
        }

    }
}
