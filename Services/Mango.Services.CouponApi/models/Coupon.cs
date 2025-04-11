public class Coupon
{
    [Key]
    public int CouponId { get; set; }
    [Required]
    public string CouponCode { get; set; }
    [Required]
    public double MinAmount { get; set; }
    [Required]
    public double DiscountAmount { get; set; }

}