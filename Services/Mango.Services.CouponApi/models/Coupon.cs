namespace Mango.Services.CouponApi.Models
{

    public class Coupon
        {
            public int CouponId { get; set; }
            public string CouponCode { get; set; } = string.Empty;
            public double MinAmount { get; set; }

            public double DiscountAmount { get; set; }

        }
}
