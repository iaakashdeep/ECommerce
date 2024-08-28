using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ECommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //CHecking the session for cart total on  login and logout
            var claimidentity = (ClaimsIdentity)User.Identity;
            var claim = claimidentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                HttpContext.Session.SetInt32(ECommerce.Utility.StaticDetails.SessionDetails, _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Count());
            }

            var prodList = _unitOfWork.Product.GetAll(includeProperty: "Category");
            return View(prodList);
        }
        [HttpGet]
        public IActionResult Details(int productId)
        {
            ShoppingCart shopping = new ShoppingCart
            {
                Product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == productId, includeProperty: "Category"),
                Count = 1,
                ProductId = productId
            };
            
            return View(shopping);

        }
        [HttpPost]
        [Authorize]             //This alone Authorize keyword indicates that any user must be login to post the shopping cart
        public IActionResult Details(ShoppingCart cart)
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var userId = claimidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            var cartDetails = _unitOfWork.shoppingCart.GetFirstorDefault(x => x.ApplicationUserId == userId && x.ProductId == cart.ProductId);

            //If we don't check the below condition every time if we increse the count of cart for the same user it will create new entry instead of increasing the count
            if (cartDetails != null) {
                cartDetails.Count += cart.Count;
                _unitOfWork.shoppingCart.Update(cartDetails);
                _unitOfWork.Save();
                //Even though if we don't write _unitOfWork.shoppingCart.Update(cartDetails); this will update the DB because EF core is smart enough to see the count property is updating so it will update the table 
                //as it is checking the table constantly, for that not to be happen we need to make the EF core monitor or tracked property to false and we have done in Repository class and interface
            }
            else
            {
                _unitOfWork.shoppingCart.Add(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(ECommerce.Utility.StaticDetails.SessionDetails, _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());
            }

            
            
            TempData["Success"] = "Cart updated successfully!!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
