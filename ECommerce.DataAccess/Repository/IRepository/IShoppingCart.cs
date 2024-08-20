﻿using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository.IRepository
{
    public interface IShoppingCart:IRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
    }
}