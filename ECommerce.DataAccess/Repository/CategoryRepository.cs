using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class CategoryRepository : Repository.Repository<Category>, IRepository.ICategoryRepository
    {
        private ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context):base(context) {
            _context = context;
            
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }
    }

    #region Notes Important
    //This class is the client class the user will need this class object for category related operations, this is why it been referenced by Repository class and ICategoryRepository interface to inherit all the features
    //in repository class and ICategoryRepository interface as well as IRepository interface because this interface is base interface for ICategoryRepository, so all the features of IRepository will be automatically inherited by
    //ICategoryRepository interface.

    //Now this class has been inherited by Repository class and ICategoryRepository interface, because if we only inherit by IRepository.ICategoryRepository interface we have to implement all the methods again in this class,
    //but we don't need to do that because those has been already implemented in Repository.Repository<Category> class, we need to make sure of the order, Repository.Repository<Category> will be followed by IRepository.ICategoryRepository
    //while inheriting
    #endregion
}
