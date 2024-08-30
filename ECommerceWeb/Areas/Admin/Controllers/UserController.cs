using ECommerce.DataAccess.Data;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager; 
        public UserController(ApplicationDbContext applicationDbContext,UserManager<IdentityUser> userManager)
        {
            _context = applicationDbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManager(string userId)
        {
            string roleId=_context.UserRoles.FirstOrDefault(x=>x.UserId == userId).RoleId;
            RoleManagerViewModel roleManagerVM = new RoleManagerViewModel()
            {
                User = _context.ApplicationUsers.Include(x => x.Company).FirstOrDefault(x => x.Id == userId),
                RoleList = _context.Roles.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                }),
                CompanyList = _context.Companies.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            roleManagerVM.User.Role = _context.Roles.FirstOrDefault(x => x.Id == roleId).Name;
            return View(roleManagerVM);
        }
        [HttpPost]
        public IActionResult RoleManager(RoleManagerViewModel RoleManagerViewModel)
        {
            string roleId = _context.UserRoles.FirstOrDefault(x => x.UserId == RoleManagerViewModel.User.Id).RoleId;
            string oldRole = _context.Roles.FirstOrDefault(x => x.Id == roleId).Name;

            if(!(RoleManagerViewModel.User.Role==oldRole))
            {
                //a role has been updated
                ApplicationUsers applicationUsers=_context.ApplicationUsers.FirstOrDefault(x=>x.Id==RoleManagerViewModel.User.Id);
                if (RoleManagerViewModel.User.Role == StaticDetails.Role_Company) { 
                    applicationUsers.CompanyId= RoleManagerViewModel.User.CompanyId;
                }
                if (oldRole == StaticDetails.Role_Company) {
                    applicationUsers.CompanyId = null;
                }
                _context.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUsers, oldRole).GetAwaiter().GetResult();
                _userManager.RemoveFromRoleAsync(applicationUsers, RoleManagerViewModel.User.Role).GetAwaiter().GetResult();
            }

          
            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetUsers()
        {
            var lstUsers = _context.ApplicationUsers.Include(x => x.Company).ToList();

            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();


            foreach (var user in lstUsers)
            {

                var roleId = userRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(y => y.Id == roleId).Name;

                if (user.Company == null)
                {

                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = lstUsers });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objfromDB = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (objfromDB == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objfromDB != null && objfromDB.LockoutEnd > DateTime.Now) { 

                //We have to unlock the user here
                objfromDB.LockoutEnd = DateTime.Now;
            }
            else
            {
                objfromDB.LockoutEnd= DateTime.Now.AddYears(100);
            }
            _context.SaveChanges();
            return Json(new { success = true, message = "Operation successful" });


        }
        #endregion
    }
}
