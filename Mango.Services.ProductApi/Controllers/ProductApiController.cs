using Azure;
using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Migrations;
using Mango.Services.ProductApi.Models;
using Mango.Services.ProductApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/product")]
    [ApiController]
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
                _responseDto.Result = dto;
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

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post(ProductDto ProductDto)
        {
            try
            {
                Product product = new Product
                {
                    Name = ProductDto.Name,
                    Price = ProductDto.Price,
                    Description = ProductDto.Description,
                    CategoryName = ProductDto.CategoryName,
                    ImageUrl = ProductDto.ImageUrl,
                    ImageLocalPath = ProductDto.ImageLocalPath
                };
                _context.Products.Add(product);
                _context.SaveChanges();

                if (ProductDto.Image != null)
                {

                    string fileName = product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;

                    //I have added the if condition to remove the any image with same name if that exist in the folder by any change
                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    FileInfo file = new FileInfo(directoryLocation);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        ProductDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }
                _context.Products.Update(product);
                _context.SaveChanges();
                _responseDto.Result = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryName = product.CategoryName,
                    ImageUrl = product.ImageUrl
                };
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("id:int")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product obj = _context.Products.First(x => x.ProductId == id);
                if (!string.IsNullOrEmpty(obj.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists) { 
                        file.Delete();
                    }
                }

                _context.Remove(obj);
            }
            catch (Exception ex)    
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put(ProductDto productDto)
        {
            try
            {
                Product obj = new Product
                {
                    ImageUrl = productDto.ImageUrl,
                    CategoryName = productDto.CategoryName,
                    Description = productDto.Description,
                    ImageLocalPath = productDto.ImageLocalPath,
                    Name = productDto.Name,
                    Price = productDto.Price,
                    ProductId = productDto.ProductId,
                    
                };
                if (productDto.ImageUrl != null) {
                    if (!string.IsNullOrEmpty(obj.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                }

                string filename = obj.ProductId + Path.GetExtension(productDto.Image.FileName);
                string filePath = @"wwwroot\ProductImages\" + filename;
                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                {
                    productDto.Image.CopyTo(fileStream);
                }
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                obj.ImageUrl = baseUrl + "/ProductImages/" + filename;
                obj.ImageLocalPath = filePath;

                _context.Products.Update(obj);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

    }

}
