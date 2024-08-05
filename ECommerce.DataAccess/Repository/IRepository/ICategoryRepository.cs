using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository.IRepository
{
    //Category specific interface which is inheriting from base interface IRepository to inherit all the features in the base interface for Category class
    public interface ICategoryRepository:IRepository<Category>
    {
        void Update(Category category);
    }
}
