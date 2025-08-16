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
            public DbSet<MessageModel> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageModel>(entity =>
            {
                entity.Property(e => e.MessageType).HasDefaultValue((byte)0);
                entity.Property(e => e.SentAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.Edited).HasDefaultValue(false);
                entity.Property(e => e.Deleted).HasDefaultValue(false);
            });
        }


        // Add DbSets here if using EF Core for tables, otherwise omit
        // Example:
        // public DbSet<User> Users { get; set; }
        // public DbSet<Message> Messages { get; set; }
    }
}
