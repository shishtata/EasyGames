using EasyGames.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Data
{
    // AppDbContext extends IdentityDbContext to include application-specific entities
    public class AppDbContext : IdentityDbContext<Users>
    {
        // Constructor accepting DbContextOptions
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>(); // Product categories
        public DbSet<StockItem> StockItems => Set<StockItem>(); // Items in stock
        public DbSet<Subscribers> Subscribers => Set<Subscribers>(); // Newsletter subscribers

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call the base method to ensure Identity configurations are applied
            base.OnModelCreating(builder);

            // Unique names for categories
            builder.Entity<Category>()
                   .HasIndex(c => c.Name)
                   .IsUnique();

            // Unique emails for subscribers
            builder.Entity<Subscribers>()
                   .HasIndex(s => s.Email)
                   .IsUnique();

            // Default created timestamp for stock items
            builder.Entity<StockItem>()
                   .Property(s => s.CreatedUtc)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Here, we did the seeding of initial categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Books" },
                new Category { Id = 2, Name = "Games" },
                new Category { Id = 3, Name = "Toys" }
            );
        }
    }
}
