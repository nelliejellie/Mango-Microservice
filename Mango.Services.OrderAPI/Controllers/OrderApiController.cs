using AutoMapper;
using Mango.MessagePublisher.Services;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Services.IServices;
using Mango.Services.OrderAPI.Utilites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        protected ResponseDto _response;
        private readonly AppDbContext _db;
        private readonly string _sk;
        private readonly string _PaystackBaseUrl;
        private readonly IRabbitPublisher _rabbitPublisher; // 
        private readonly string _PaystackCallbackUrl; // Adjust as needed
        private IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly string _FrontendBaseUrl; // Adjust as needed
        private readonly IHttpClientFactory _httpClientFactory;
        public OrderApiController(AppDbContext db,
            IProductService productService, IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IRabbitPublisher rabbitPublisher
            )
        {
            _db = db;
            _response = new ResponseDto();
            _rabbitPublisher = rabbitPublisher;
            _productService = productService;
            _configuration = configuration;
            _sk = configuration.GetValue<string>("Paystack:SecretKey");
            _PaystackBaseUrl = configuration.GetValue<string>("Paystack:InitialiseUrl");
            _PaystackCallbackUrl = configuration.GetValue<string>("Paystack:CallbackUrl");
            _FrontendBaseUrl = configuration.GetValue<string>("ServiceUrls:frontendUrl");
            _httpClientFactory = httpClientFactory;
        }

       


        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseDto> GetOrders(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                if(User.IsInRole(StaticDetails.RoleAdmin))
                {
                    objList = await _db.OrderHeaders.Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product).ToListAsync();
                }
                else
                {
                    objList = await _db.OrderHeaders.Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                        .Where(u => u.UserId == userId).ToListAsync();
                }

                OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
                List<OrderHeaderDto> orderHeaderDtoList = new List<OrderHeaderDto>();
                foreach (var obj in objList)
                {
                    orderHeaderDto = new OrderHeaderDto
                    {
                        OrderHeaderId = obj.OrderHeaderId,
                        UserId = obj.UserId,
                        Name = obj.Name,
                        Phone = obj.Phone,
                        Email = obj.Email,
                        CouponCode = obj.CouponCode,
                        Discount = obj.Discount,
                        OrderTotal = obj.OrderTotal,
                        Status = obj.Status,
                        OrderTime = obj.OrderTime,
                        PaymentIntentId = obj.PaymentIntentId,
                        PaystackSessionId = obj.PaystackSessionId,
                        OrderDetails = obj.OrderDetails.Select(od => new OrderDetailsDto
                        {
                            OrderDetailsId = od.OrderDetailsId,
                            ProductId = od.ProductId,
                            Count = od.Count,
                            ProductName = od.ProductName,
                            Price = od.Price
                            // Add other necessary fields as needed
                        }).ToList()
                    };
                    orderHeaderDtoList.Add(orderHeaderDto);
                }
                _response.Result = orderHeaderDtoList;

            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string status)
        {
            try
            {
                var orderHeader = await _db.OrderHeaders.FindAsync(orderId);
                if (orderHeader == null)
                {
                    _response.Success = false;
                    _response.Message = "Order not found";
                    return _response;
                } 
                
                orderHeader.Status = status;
                await _db.SaveChangesAsync();
                _response.Result = new OrderHeaderDto
                {
                    OrderHeaderId = orderHeader.OrderHeaderId,
                    Status = orderHeader.Status
                };
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpPost]
        public async Task<ResponseDto> CreatePaystackSession(PaystackRequestDto paystackRequest)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sk);

                var requestData = new
                {
                    email = paystackRequest.OrderHeader.Email,
                    amount = paystackRequest.OrderHeader.OrderTotal, // in kobo (i.e. ₦5000.00)
                    callback_url = _PaystackCallbackUrl
                };

                var json = System.Text.Json.JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_PaystackBaseUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Response from Paystack:");
                Console.WriteLine(responseString);
            }
            catch (Exception ex)
            {

                _response.Success = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("/payment/callback")]
        public async Task<IActionResult> PaystackCallback([FromQuery] string reference)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reference))
                {
                    _response.Success = false;
                    return Redirect($"{_FrontendBaseUrl}/payment-failed");
                }

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _configuration["Paystack:SecretKey"]);

                var response = await client.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");
                var responseContent = await response.Content.ReadAsStringAsync();
                VerificationResponseDto Dto = JsonConvert.DeserializeObject<VerificationResponseDto>(Convert.ToString(response));

                if (!response.IsSuccessStatusCode)
                {
                    _response.Success = false;
                    return Redirect($"{_FrontendBaseUrl}/payment-failed");
                }
                // Optionally: parse the response JSON and update order status in your database
                // Example:
                // var transactionData = JsonSerializer.Deserialize<PaystackVerifyResponse>(responseContent);


                RewardsDto rewards = new RewardsDto();
                rewards.UserID = User.FindFirst("sub")?.Value; // Assuming you have user ID in JWT token
                rewards.RewardsActivity = Convert.ToInt32(Dto.Data.Amount);

                await _rabbitPublisher.PublishFanOutMessageAsync(rewards, _configuration.GetValue<string>("Queues:OrderCreated"));


                _response.Success = true;
                _response.Message = "Payment verification successful";
                _response.Result = responseContent; // You can return the raw content or parse it into a DTO
                return Redirect($"{_FrontendBaseUrl}/payment-success?reference={reference}");

            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
                return Redirect($"{_FrontendBaseUrl}/payment-failed");
            }

        }

        public async Task<ResponseDto> CreateOrder([FromBody]CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = new OrderHeaderDto
                {
                    UserId = cartDto.CartHeader.UserId,
                    Name = cartDto.CartHeader.Name,
                    Phone = cartDto.CartHeader.Phone,
                    Email = cartDto.CartHeader.Email,
                    CouponCode = cartDto.CartHeader.CouponCode,
                    OrderDetails = new List<OrderDetailsDto>()
                };

                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = StaticDetails.Status_Pending;
                var orderDetailsList = new List<OrderDetailsDto>();

                if (cartDto.CartDetails != null)
                {
                    foreach (var cartDetail in cartDto.CartDetails)
                    {
                        var orderDetail = new OrderDetailsDto
                        {
                            OrderHeaderId = orderHeaderDto.OrderHeaderId, // assuming this is already set
                            ProductId = cartDetail.ProductId,
                            Count = cartDetail.Count,
                            ProductName = cartDetail.Product?.Name ?? string.Empty, // null-safe
                            Product = cartDetail.Product != null ? new ProductDto
                            {
                                // map only necessary fields
                                Price = cartDetail.Product.Price,
                                Name = cartDetail.Product.Name,
                                Description = cartDetail.Product.Description,
                                ImageUrl = cartDetail.Product.ImageUrl,
                                CategoryName = cartDetail.Product.CategoryName,
                                ProductId = cartDetail.Product.ProductId
                                // add others as needed
                            } : null
                        };

                        orderDetailsList.Add(orderDetail);
                    }
                }

                orderHeaderDto.OrderDetails = orderDetailsList;

                OrderHeader createdOrder = new OrderHeader
                {
                    OrderTime = orderHeaderDto.OrderTime,
                    UserId = orderHeaderDto.UserId,
                    Name = orderHeaderDto.Name,
                    Phone = orderHeaderDto.Phone,
                    Email = orderHeaderDto.Email,
                    CouponCode = orderHeaderDto.CouponCode,
                    Discount = orderHeaderDto.Discount,
                    OrderTotal = orderHeaderDto.OrderTotal,
                    Status = orderHeaderDto.Status,
                    PaymentIntentId = orderHeaderDto.PaymentIntentId,
                    PaystackSessionId = orderHeaderDto.PaystackSessionId,
                    OrderDetails = orderHeaderDto.OrderDetails.Select(od => new OrderDetails
                    {
                        ProductId = od.ProductId,
                        Count = od.Count,
                        ProductName = od.ProductName,
                        Price = od.Product?.Price ?? 0 // null-safe
                    }).ToList()
                };
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = createdOrder.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
                
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include(o => o.OrderDetails)
                    .First(u => u.OrderHeaderId == id);

                OrderHeaderDto orderHeaderDto = new OrderHeaderDto
                {
                    OrderHeaderId = orderHeader.OrderHeaderId,
                    UserId = orderHeader.UserId,
                    Name = orderHeader.Name,
                    Phone = orderHeader.Phone,
                    Email = orderHeader.Email,
                    CouponCode = orderHeader.CouponCode,
                    Discount = orderHeader.Discount,
                    OrderTotal = orderHeader.OrderTotal,
                    Status = orderHeader.Status,
                    OrderTime = orderHeader.OrderTime,
                    PaymentIntentId = orderHeader.PaymentIntentId,
                    PaystackSessionId = orderHeader.PaystackSessionId,
                    OrderDetails = orderHeader.OrderDetails.Select(od => new OrderDetailsDto
                    {
                        OrderDetailsId = od.OrderDetailsId,
                        ProductId = od.ProductId,
                        Count = od.Count,
                        ProductName = od.ProductName,
                        Price = od.Price
                        // Add other necessary fields as needed
                    }).ToList()
                };
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {   
                _response.Success = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
