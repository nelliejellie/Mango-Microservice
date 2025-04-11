using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponApi.AppDbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CouponDto> Coupons { get; set; }
    }
}