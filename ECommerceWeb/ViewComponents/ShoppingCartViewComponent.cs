using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWeb.ViewComponents
{
    public class ShoppingCartViewComponent:ViewComponent
    {
        private IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var claim = claimidentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(StaticDetails.SessionDetails)==null)
                {
                    HttpContext.Session.SetInt32(ECommerce.Utility.StaticDetails.SessionDetails, _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(StaticDetails.SessionDetails));
                
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
