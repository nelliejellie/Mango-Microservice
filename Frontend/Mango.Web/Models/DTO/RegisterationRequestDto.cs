using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.DTO
{
    public class RegisterationRequestDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Password { get; set; }
        public string RoleName { get; set; } = string.Empty;

    }
}