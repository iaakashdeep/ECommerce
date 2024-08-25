using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> shoppingCartList;

        public OrderHeader orderHeader { get; set; }  //Adding this property because on summary page it can be order details or cart information

        //public double OrderTotal;   removing this propert from here because OrderHeader already has OrderTotal
    }
}
