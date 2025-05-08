using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Models.Utilities;

namespace Mango.Web.Services
{
    public class CouponService: ICouponService
    {
        private readonly IBaseService _baseService;
        private readonly string couponApiBase;

        public CouponService(IBaseService baseService, IConfiguration configuration)
        {
            _baseService = baseService;
            couponApiBase = configuration.GetValue<string>("ServiceUrls:CouponApi");
        }


        public async Task<ResponseDto?> GetAllCouponAsync()
        {
            var response = await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = couponApiBase + "/coupon",
            });
            return response;
        }
        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            var response = await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = couponApiBase + "/coupon/" + couponCode,
            });
            return response;
        }
        public async Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            var response = await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = couponApiBase + "/coupon/" + id,
            });
            return response;
        }
        public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
        {
            var response = await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = couponApiBase + "/coupon/",
                Data = couponDto
            });
            return response;
        }
        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
        {
            var response = await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Url = couponApiBase + "/coupon/",
                Data = couponDto
            });
            return response;
        }
        public async Task<ResponseDto?> DeleteCouponAsync(int id)
        {
            var response = await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = couponApiBase + "/coupon/" + id,
            });
            return response;
        }
    }
}