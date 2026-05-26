using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiMeter.Data
{
    public class AppDbContext : IdentityDbContext<ApiMeter.Models.ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ApiMeter.Models.Meter> Meters { get; set; }
    }
}