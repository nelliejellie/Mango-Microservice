
namespace Mango.Services.AuthApi.Models.DTO
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public UserDto User { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}