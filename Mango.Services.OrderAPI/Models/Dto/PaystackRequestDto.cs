namespace Mango.Services.OrderAPI.Models.Dto
{
    public class PaystackRequestDto
    {
        public string? PaystackSessionUrl { get; set; }
        public string? PaystackSessionId { get; set; }
        public string ApprovedUrl { get; set; }
        public string CancelUrl { get; set; }
        public OrderHeaderDto OrderHeader { get; set; }
    }
}
