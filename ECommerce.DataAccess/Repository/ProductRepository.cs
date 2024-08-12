using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class ProductRepository : Repository.Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext db):base(db) 
        {
            _context = db;
        }
        public void Update(Product product)
        {
            var prodDetails=_context.Products.FirstOrDefault(x=>x.Id==product.Id);
            if (prodDetails != null) { 
                prodDetails.Title = product.Title;
                prodDetails.Description = product.Description;  
                prodDetails.CategoryId = product.CategoryId;
                prodDetails.Price = product.Price;
                prodDetails.ListPrice = product.ListPrice;
                prodDetails.Price100 = product.Price100;
                prodDetails.Price50 = product.Price50;
                prodDetails.ISBN = product.ISBN;
                prodDetails.Author = product.Author;
                if (product.ImageUrl != null) { 
                    prodDetails.ImageUrl= product.ImageUrl;
                }
            }

            //==========================OR We can also write============================

            //_context.Products.Update(product);  //but sometimes this method will not work as it is provided by EF core and if we try to update using third party tools like data tables this will show error while updating because of Id
        }


    }
}
