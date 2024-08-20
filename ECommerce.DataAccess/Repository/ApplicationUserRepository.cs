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
    //If for some reason we may need to work on application users we need this repository
    public class ApplicationUserRepository:Repository<ApplicationUsers>,IApplicationUser
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext applicationDbContext):base(applicationDbContext)
        {
            _context=applicationDbContext;
        }
    }
}
