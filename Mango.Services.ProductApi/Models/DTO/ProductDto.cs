namespace Mango.Services.ProductApi.Models.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }
    }
}
