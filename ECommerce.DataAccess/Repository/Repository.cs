using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
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
            _context.Products.Include(x=>x.Category).ToList();
                //.Include(y=>(y as Product).CategoryId)

            //By using Include it will tell when Products data will upload it will also take Category data, Include function is provided by EF core, it can include multiple properties
            //This can only take navigational property so in Product model the navigational property for CategoryId is Category, we can pass other properties as well , seperated
        }
        public void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>>? filter=null,string? includeProperty = null)
        {
            IQueryable<T> queryData = _dbset;
            if(filter != null)
            {
                queryData = queryData.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperty))
            {
                foreach (var prop in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryData = queryData.Include(prop);
                }
            }
            return queryData.ToList();
        }

        public T GetFirstorDefault(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includeProperty=null, bool tracked=false)
        {
            IQueryable<T> queryData;
            if (tracked)
            {
                queryData = _dbset;
            }
            else
            {
                queryData = _dbset.AsNoTracking();      //AsNoTracking() used to tell the EF core to not track the changes automatically
            }
            
            queryData = queryData.Where(filter);
            if (!string.IsNullOrEmpty(includeProperty)) {
                foreach (var prop in includeProperty.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)) { 
                    queryData=queryData.Include(prop);
                }
            }
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
