
namespace Mango.Services.CouponApi.Models.DTO
{

    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public double MinAmount { get; set; }

        public double DiscountAmount { get; set; }

    }
    
}