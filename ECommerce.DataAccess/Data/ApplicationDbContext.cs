using ECommerce.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }  //<Category> here we define the entity whatever we want to create a table for in DB
        public DbSet<Product> Products { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
            modelBuilder.SeedProduct(); //Extention methods created for modelBuilder
        }

    }
} 
