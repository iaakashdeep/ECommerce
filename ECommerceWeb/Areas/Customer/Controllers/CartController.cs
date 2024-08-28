using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using System.CodeDom.Compiler;
using System;
using System.Security.Claims;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace ECommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]                              //To get the shopping cart details on POST
        public ShoppingCartViewModel _cartViewModel { get; set; }
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
                shoppingCartList = _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperty: "Product"),
                orderHeader=new()
			};

            spCartVM.orderHeader.ApplicationUser = _unitOfWork.applicationUser.GetFirstorDefault(x=>x.Id==userId);
            spCartVM.orderHeader.City = spCartVM.orderHeader.ApplicationUser.City;
            spCartVM.orderHeader.Name=spCartVM.orderHeader.ApplicationUser.Name;
            spCartVM.orderHeader.PostalCode = spCartVM.orderHeader.ApplicationUser.Name;
            spCartVM.orderHeader.StreetAddress = spCartVM.orderHeader.ApplicationUser.StreetAddress;
            spCartVM.orderHeader.PhoneNumber = spCartVM.orderHeader.ApplicationUser.PhoneNumber;


			foreach (var spCart in spCartVM.shoppingCartList)
            {
                spCart.Price = GetOrderTotalPrice(spCart);
                spCartVM.orderHeader.OrderTotal += (spCart.Price * spCart.Count);
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
            var cartTotalfromDB = _unitOfWork.shoppingCart.GetFirstorDefault(x => x.Id == cartId,tracked:true);
            if (cartTotalfromDB.Count <= 1)
            {
                HttpContext.Session.SetInt32(ECommerce.Utility.StaticDetails.SessionDetails, _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == cartTotalfromDB.ApplicationUserId).Count() - 1);
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
            //System.InvalidOperationException
            //Message = The instance of entity type 'ShoppingCart' cannot be tracked because another instance with the same key value for { 'Id'} is already being tracked.When attaching existing entities,
            //ensure that only one entity instance with a given key value is attached.Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values.
  

            var cartTotalfromDB = _unitOfWork.shoppingCart.GetFirstorDefault(x => x.Id == cartId,tracked:true);
            HttpContext.Session.SetInt32(ECommerce.Utility.StaticDetails.SessionDetails, _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == cartTotalfromDB.ApplicationUserId).Count()-1);
            _unitOfWork.shoppingCart.Remove(cartTotalfromDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {

			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartViewModel spCartVM = new ShoppingCartViewModel()
			{
				shoppingCartList = _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperty: "Product"),
				orderHeader = new()
			};

			spCartVM.orderHeader.ApplicationUser = _unitOfWork.applicationUser.GetFirstorDefault(x => x.Id == userId);
			spCartVM.orderHeader.City = spCartVM.orderHeader.ApplicationUser.City;
			spCartVM.orderHeader.Name = spCartVM.orderHeader.ApplicationUser.Name;
			spCartVM.orderHeader.PostalCode = spCartVM.orderHeader.ApplicationUser.PostalCode;
			spCartVM.orderHeader.StreetAddress = spCartVM.orderHeader.ApplicationUser.StreetAddress;
			spCartVM.orderHeader.PhoneNumber = spCartVM.orderHeader.ApplicationUser.PhoneNumber;
            spCartVM.orderHeader.State= spCartVM.orderHeader.ApplicationUser.State;


			foreach (var spCart in spCartVM.shoppingCartList)
			{
				spCart.Price = GetOrderTotalPrice(spCart);
				spCartVM.orderHeader.OrderTotal += (spCart.Price * spCart.Count);
			}
			return View(spCartVM);
		}
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            _cartViewModel.shoppingCartList = _unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperty: "Product");
            _cartViewModel.orderHeader.OrderDate = DateTime.Now;
            _cartViewModel.orderHeader.ApplicationUserId = userId;

			//The error will come if we save the user in _cartViewModel.orderHeader.ApplicationUser because this is already been inserted in the DB
			//Error: Violation of PRIMARY KEY constraint 'PK_AspNetUsers'. Cannot insert duplicate key in object 'dbo.AspNetUsers'. The duplicate key value is (bea01c59-ed76-4b26-9524-0fd273172355).

			//_cartViewModel.orderHeader.ApplicationUser = _unitOfWork.applicationUser.GetFirstorDefault(x => x.Id == userId);

            ApplicationUsers users = _unitOfWork.applicationUser.GetFirstorDefault(x => x.Id == userId);



			foreach (var spCart in _cartViewModel.shoppingCartList)
			{
				spCart.Price = GetOrderTotalPrice(spCart);
				_cartViewModel.orderHeader.OrderTotal += (spCart.Price * spCart.Count);
			}

            //Checking this condition because company user can have the leverage of making payment within 30 days after placing the order but not the normal user
            if(users.CompanyId.GetValueOrDefault()==0)
            {
                //Normal user and can be redirected to payment gateway
                _cartViewModel.orderHeader.PaymentStatus=StaticDetails.PaymentStatusPending;
                _cartViewModel.orderHeader.OrderStatus = StaticDetails.StatusPending;
            }
            else
            {
				//Company user
				_cartViewModel.orderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
				_cartViewModel.orderHeader.OrderStatus = StaticDetails.StatusApproved;
			}

            //Order Header Creation
            _unitOfWork.OrderHeader.Add(_cartViewModel.orderHeader);
            _unitOfWork.Save();

            //Order Details creation for each shopping cart list items
            foreach(var items in _cartViewModel.shoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId= items.ProductId,
                    OrderHeaderId=_cartViewModel.orderHeader.Id,
                    Price= items.Price,
                    Count= items.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

			//Payment gateway logic stripe

			if (users.CompanyId.GetValueOrDefault() == 0)
			{
                //Stripe logic from googl

                var domain = Request.Scheme + "://" + Request.Host.Value;       //We can't hardcode the URL because when we host this app and send Back to Application button from Stripe it will throw error
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={_cartViewModel.orderHeader.Id}",    //Redirecting to this URL doesn't give the details from stripe so we have to check the session id again in Order Confirmation Action
                    CancelUrl = domain + "customer/cart/index",

                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach(var item in _cartViewModel.shoppingCartList)
                {
                    var sessionLineItems = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),       //If-> $20.50 =>2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItems);
                }

                var service=new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session sessionService=service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentId(_cartViewModel.orderHeader.Id, sessionService.Id, sessionService.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location",sessionService.Url);
                return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation),new {id=_cartViewModel.orderHeader.Id});
		}

        //Order Confirmation Page

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeaderDetails=_unitOfWork.OrderHeader.GetFirstorDefault(x=>x.Id == id,includeProperty:"ApplicationUser");
            if (orderHeaderDetails.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
            {
                //means this order is from customer

                var service = new Stripe.Checkout.SessionService();
				Stripe.Checkout.Session session = service.Get(orderHeaderDetails.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id,session.Id,session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id,StaticDetails.StatusApproved,StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();            //Added this line because after payment from Stripe the cart count hasn't been cleared
            }
            List<ShoppingCart> shoppingCart=_unitOfWork.shoppingCart.GetAll(x=>x.ApplicationUserId==orderHeaderDetails.ApplicationUserId).ToList();
            _unitOfWork.shoppingCart.RemoveRange(shoppingCart);
            _unitOfWork.Save();
            return View(orderHeaderDetails);
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
