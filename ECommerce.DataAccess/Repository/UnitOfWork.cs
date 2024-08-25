using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category {  get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCart shoppingCart { get; private set; }
        public IApplicationUser applicationUser { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        private ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Product=new ProductRepository(_context);
            Company=new CompanyRepository(_context);
            shoppingCart = new ShoppingCartRepository(_context);
            applicationUser = new ApplicationUserRepository(_context);
            OrderDetail=new OrderDetailsRepository(_context);
            OrderHeader=new OrderHeaderRepository(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
