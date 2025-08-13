using BondTalesChat_Server.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BondTalesChat_Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
            public DbSet<Message> Messages { get; set; }

        

        // Add DbSets here if using EF Core for tables, otherwise omit
        // Example:
        // public DbSet<User> Users { get; set; }
        // public DbSet<Message> Messages { get; set; }
    }
}
