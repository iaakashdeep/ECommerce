using ECommerce.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }  
        public DbSet<Product> Products { get; set; }

        public DbSet<ApplicationUsers> ApplicationUsers { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
            modelBuilder.SeedProduct();
            modelBuilder.SeedCompany();
        }

    }
}


