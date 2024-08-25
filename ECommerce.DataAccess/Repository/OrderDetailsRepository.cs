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
	public class OrderDetailsRepository:Repository<OrderDetail>,IOrderDetailRepository
	{
        private readonly ApplicationDbContext _context;
        public OrderDetailsRepository(ApplicationDbContext dbContext):base(dbContext) 
        {
            _context = dbContext;
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }
    }
}
