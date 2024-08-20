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
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null,string? includeProperty=null);
        T GetFirstorDefault(Expression<Func<T, bool>> filter, string? includeProperty = null, bool tracked=false);

        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        //Adding filter in GetAll method and make it as nullable because this will serve both the purpose 1. Giving all details based on filter and 2. If filter is null giving all details

    }
}
