namespace Mango.Services.OrderAPI.Models
{
    public class PaystackResponse
    {
        public string? Email { get; set; }
        public bool Status { get; set; }
        public string? Message { get; set; }
        public PaystackData PaystackData { get; set; }

    }
}
