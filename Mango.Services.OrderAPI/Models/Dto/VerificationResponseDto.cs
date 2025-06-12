namespace Mango.Services.OrderAPI.Models.Dto
{
    public class VerificationResponseDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public TransactionDataDto Data { get; set; }
    }
}
