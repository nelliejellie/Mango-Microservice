﻿using Mango.Web.Models.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        [Range(1,100)]
        public int Count { get; set; } = 1;
        [MaxFileSize(1)]
        [AllowedExtension(new string[] {".jpg",".png"})]
        public IFormFile? Image { get; set; }

    }
}
