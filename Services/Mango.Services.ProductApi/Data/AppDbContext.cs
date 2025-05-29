using Mango.Services.ProductApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Product 1",
                Price = 100,
                Description = "Description for Product 1",
                CategoryName = "Category 1",
                ImageUrl = "https://placehold.co/603*403"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Product 2",
                Price = 200,
                Description = "Description for Product 2",
                CategoryName = "Category 2",
                ImageUrl = "https://placehold.co/602*402"
            });
        }
    }
}
