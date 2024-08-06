using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Repository
{
    //A generic Repository class where we can pass any class at runtime and all the methods defined here will be accessed by that class
    public class Repository<T> : IRepository<T> where T:class
    {
        private readonly ApplicationDbContext _context;

        internal DbSet<T> _dbset;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();     //In ApplicationDbContext class we have set Category to DBset and new table has been made but here this class is generic and whatever class we pass at runtime the DBSet will create a
                                            //tabel for that object, because of this we are setting dbset in the constructor
        }
        public void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> queryData = _dbset;
            return queryData.ToList();
        }

        public T GetFirstorDefault(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            //IEnumerable<T> values = _dbset.Where(filter);
            //return values.FirstOrDefault();

            //==================OR===========================

            IQueryable<T> queryData = _dbset;
            queryData = queryData.Where(filter);
            return queryData.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbset.RemoveRange(entities);       //This RemoveRange method will remove entities from database from certain range we can pass the range in an array
        }
    }
}
