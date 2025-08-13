using Microsoft.EntityFrameworkCore;

namespace BondTalesChat_Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Add DbSets here if using EF Core for tables, otherwise omit
        // Example:
        // public DbSet<User> Users { get; set; }
        // public DbSet<Message> Messages { get; set; }
    }
}
