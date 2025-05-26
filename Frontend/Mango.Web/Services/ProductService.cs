using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Models.Utilities;
using Mango.Web.Services.IService;
using Microsoft.Extensions.Configuration;

namespace Mango.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        private readonly string couponApiBase;
        public ProductService(IBaseService baseService, IConfiguration configuration)
        {
            _baseService = baseService;
            couponApiBase = configuration.GetValue<string>("ServiceUrls:ProductApi");
        }

        public async Task<ResponseDto?> CreateProductsAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = productDto,
                Url = couponApiBase + "/api/product",
                //ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> DeleteProductsAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.DELETE,
                Url = couponApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = couponApiBase + "/api/product"
            },false);
        }



        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = couponApiBase + "/api/product" + id
            });
        }

        public async Task<ResponseDto?> UpdateProductsAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.PUT,
                Data = productDto,
                Url = couponApiBase + "/api/product",
               // ContentType = SD.ContentType.MultipartFormData
            });
        }
    }
}
