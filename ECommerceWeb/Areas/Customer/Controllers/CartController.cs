using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //To get the current userid which is doing the checkout from cart
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel spCartVM = new ShoppingCartViewModel()
            {
                //adding the Product property to Shopping Cart because this object will return shopping cart details along with  Product Id, but to view the product details we need to access the
                //Product class which reside in shopping cart model as Navigation Property. So, includeProperty is used to access Navigation Properties
                shoppingCartList = _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperty: "Product")
            };

            foreach (var spCart in spCartVM.shoppingCartList)
            {
                spCart.Price = GetOrderTotalPrice(spCart);
                spCartVM.OrderTotal += (spCart.Price * spCart.Count);
            }
            return View(spCartVM);
        }

        public IActionResult Plus(int? cartId)
        {
            var cartTotalfromDB = _unitOfWork.shoppingCart.GetFirstorDefault(x => x.Id == cartId);
            cartTotalfromDB.Count += 1;
            _unitOfWork.shoppingCart.Update(cartTotalfromDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int? cartId)
        {
            var cartTotalfromDB = _unitOfWork.shoppingCart.GetFirstorDefault(x => x.Id == cartId);
            if (cartTotalfromDB.Count <= 1)
            {
                _unitOfWork.shoppingCart.Remove(cartTotalfromDB);
            }
            else
            {

                cartTotalfromDB.Count -= 1;
                _unitOfWork.shoppingCart.Update(cartTotalfromDB);
            }


            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int? cartId)
        {
            var cartTotalfromDB = _unitOfWork.shoppingCart.GetFirstorDefault(x => x.Id == cartId);
            _unitOfWork.shoppingCart.Remove(cartTotalfromDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {

        return View(); 
        }

        private double GetOrderTotalPrice(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else
            {
                if (cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;
                }
            }
        }


    }
}
