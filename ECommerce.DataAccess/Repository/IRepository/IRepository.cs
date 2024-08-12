using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository.IRepository
{
    //A generic interface where T is class means we can pass any class at runtime
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperty=null);
        T GetFirstorDefault(Expression<Func<T, bool>> filter, string? includeProperty = null);

        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        //Expression<Func<T, bool>> filter: Here expression will take a lambda expression, we have passed Func delegate because we have to pass a condition, in this case condition to match the id from the page to the ID from DB
        //and this will result in bool (true or false) either found or not found
        //this will acts as:  x => x.Id == categoryId

    }
}
