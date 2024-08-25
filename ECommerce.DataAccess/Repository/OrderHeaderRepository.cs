using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
	public class OrderHeaderRepository:Repository<OrderHeader>,IOrderHeaderRepository
	{
        private readonly ApplicationDbContext _dbContext;
        public OrderHeaderRepository(ApplicationDbContext dbContext):base(dbContext) 
        {
            _dbContext = dbContext;
        }
        public void Update(OrderHeader orderHeader)
        {
            _dbContext.OrderHeaders.Update(orderHeader);
        }

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderHeaderdetails=_dbContext.OrderHeaders.FirstOrDefault(x=>x.Id == id);
			if (orderHeaderdetails != null)
			{
				orderHeaderdetails.OrderStatus=orderStatus;
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					orderHeaderdetails.PaymentStatus=paymentStatus;
				}
			}
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderHeaderdetails = _dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if (orderHeaderdetails != null && !string.IsNullOrEmpty(sessionId)) { 
				orderHeaderdetails.SessionId=sessionId;
			}
			if (!string.IsNullOrEmpty(paymentIntentId)) { 
				orderHeaderdetails.PaymentIntentId=paymentIntentId;
				orderHeaderdetails.PaymentDate=DateTime.Now;
			}

		}
	}
}
