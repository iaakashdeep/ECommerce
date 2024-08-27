using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models.ViewModels
{
	public class OrderViewModel
	{
		//public OrderHeader orderHeader;
		public OrderHeader orderHeader { get; set; }
		public IEnumerable<OrderDetail> orderDetail { get; set; }
	}

	//The difference between line 12 and line 13 is line 12 is Field declaration and line 13 is auto implemented property
	//You can include validation or any logic for propert but not for fields.
	//line 13 will automatically creates a private field with same structure in backend
}
