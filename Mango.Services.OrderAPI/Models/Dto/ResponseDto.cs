namespace Mango.Services.OrderAPI.Models.Dto
{
    public class ResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Result { get; set; }
        public bool Success { get; set; }
    }
}