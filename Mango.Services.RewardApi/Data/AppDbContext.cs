using Mango.Services.RewardApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Reward> Rewards { get; set; }
    }
}
