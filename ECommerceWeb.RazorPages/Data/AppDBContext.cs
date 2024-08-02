using ECommerceWeb.RazorPages.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWeb.RazorPages.Data
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                 new Category { Id = 1, Name = "Romantic", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Scifi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Action", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Fiction", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Finance", DisplayOrder = 5 }
                );
        }
    }
}
