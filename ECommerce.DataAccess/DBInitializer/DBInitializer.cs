using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _dbCOntext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public DBInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;

            _roleManager = roleManager;
            _dbCOntext = dbContext;
        }
        public DBInitializer()
        {
            
        }
        public void Initialize()
        {

            //migrations if not applied

            try
            {
                if(_dbCOntext.Database.GetPendingMigrations().Count()>0)
                {
                    _dbCOntext.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            //create roles if not already created
            if (!_roleManager.RoleExistsAsync(StaticDetails.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Company)).GetAwaiter().GetResult();

                //if roles are not created we have to create admin user as well

                _userManager.CreateAsync(new ApplicationUsers
                {
                    UserName = "admin_new@gmail.com",
                    Email = "admin_new@gmail.com",
                    Name = "Aakash Admin",
                    PhoneNumber = "54800545",
                    StreetAddress = "test in bangalore",
                    PostalCode = "54850054545874",
                    City = "Bengaluru",
                    State = "Karnataka"
                }, "1616@Aakash").GetAwaiter().GetResult();

                ApplicationUsers user = _dbCOntext.ApplicationUsers.FirstOrDefault(x => x.Email == "admin_new@gmail.com");
                _userManager.AddToRoleAsync(user,StaticDetails.Role_Admin).GetAwaiter().GetResult();        //To make the newly created user as admin
            }
            return;
            
        }
    }
}
