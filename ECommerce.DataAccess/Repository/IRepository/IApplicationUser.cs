using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository.IRepository
{
    //If for some reason we may need to work on application users we need this repository
    public interface IApplicationUser:IRepository<ApplicationUsers>
    {
    }
}
