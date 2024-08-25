using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository.IRepository
{
	public interface IOrderHeaderRepository:IRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeader);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus=null);  //Using if we only want to update paymentstatus or order status based on ID, payment status can be null sometimes because
																					//payment status once approved it will be approved all the time
		void UpdateStripePaymentId(int id,string sessionId,string paymentIntentId);  //Based on order header id we update session id and payment id
	}
}
