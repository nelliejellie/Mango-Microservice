using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ProductApi.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Range(1,1000)]
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? ImageLocalPath { get; set; }
    }
}
