using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Models;
using Mango.Services.ProductApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductApiController : ControllerBase
    {
        private readonly ILogger<ProductApiController> _logger;
        private ResponseDto _responseDto;
        private readonly AppDbContext _context;

        public ProductApiController(AppDbContext context)
        {
            _context = context;
            _responseDto = new ResponseDto();
        }

        [HttpGet]
        [AllowAnonymous]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> objList = _context.Products.ToList();
                foreach (var obj in objList)
                {
                    ProductDto dto = new ProductDto()
                    {
                        ProductId = obj.ProductId,
                        Name = obj.Name,
                        Price = obj.Price,
                        Description = obj.Description,
                        CategoryName = obj.CategoryName,
                        ImageUrl = obj.ImageUrl
                    };
                }
                _responseDto.Result = objList;
                _responseDto.Message = "Items collected successfully";
                _responseDto.StatusCode = 200;
                _responseDto.Success = true;
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.Message;
                throw;
            }

            return _responseDto;
        }

        [HttpGet("{id}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product obj = _context.Products.FirstOrDefault(u => u.ProductId == id);
                ProductDto dto = new ProductDto()
                {
                    ProductId = obj.ProductId,
                    Name = obj.Name,
                    Price = obj.Price,
                    Description = obj.Description,
                    CategoryName = obj.CategoryName,
                    ImageUrl = obj.ImageUrl
                };
                _responseDto.Result = obj;
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.Message;
                throw;
            }
            return _responseDto;
        }
    }

}
