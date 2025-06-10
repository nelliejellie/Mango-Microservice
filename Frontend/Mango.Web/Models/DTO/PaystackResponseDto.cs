namespace Mango.Web.Models.DTO
{
    public class PaystackResponseDto
    {
        public string? Email { get; set; }
        public bool Status { get; set; }
        public string? Message { get; set; }
        public PaystackDataDto PaystackDataDto { get; set; }
    }
}
