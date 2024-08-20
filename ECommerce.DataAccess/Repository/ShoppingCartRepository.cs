using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCart
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext applicationDbContext):base(applicationDbContext) 
        {
            _context = applicationDbContext;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _context.ShoppingCarts.Update(shoppingCart);
        }
    }
}
