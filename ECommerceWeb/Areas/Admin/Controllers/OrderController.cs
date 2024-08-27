using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Diagnostics;
using System.Security.Claims;

namespace ECommerceWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
	public class OrderController : Controller
	{
		private IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderViewModel orderViewModel { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		public IActionResult Index()
		{
			return View();
		}

        #region API Calls
        /// <summary>
        /// This will act as API when you call by: admin/order/getorders from Datatable script this will give the result because MVC has in built support for api
        /// </summary>
        /// <returns></returns>
        public IActionResult GetOrders(string status)
		{
            IEnumerable<OrderHeader> lstOrders;
            //Filter orders based on roles
            if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                lstOrders = _unitOfWork.OrderHeader.GetAll(includeProperty: "ApplicationUser").ToList();
            }
			else
            {
                //For customer and company users based on their userid
                var claimIdentity=(ClaimsIdentity)User.Identity;
                var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                lstOrders = _unitOfWork.OrderHeader.GetAll(x=>x.ApplicationUser.Id==userId,includeProperty: "ApplicationUser").ToList();
            }

            switch (status)
            {
                case "pending":
					lstOrders = lstOrders.Where(x => x.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
					lstOrders = lstOrders.Where(x => x.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "approved":
					lstOrders = lstOrders.Where(x => x.OrderStatus == StaticDetails.StatusApproved);
                    break;
                case "completed":
					lstOrders = lstOrders.Where(x => x.PaymentStatus == StaticDetails.StatusShipped);
                    break;
                default:
                    break;
            }
            return Json(new { data = lstOrders });
		}

        #endregion

        public IActionResult Details(int orderId)
		{
            orderViewModel = new OrderViewModel()
            {
                orderDetail=_unitOfWork.OrderDetail.GetAll(x=>x.OrderHeaderId==orderId,includeProperty:"Product"),
                orderHeader=_unitOfWork.OrderHeader.GetFirstorDefault(x=>x.Id==orderId,includeProperty:"ApplicationUser"),

            };
            return View(orderViewModel);
		}
        [HttpPost]
        [Authorize(Roles=StaticDetails.Role_Admin+","+ StaticDetails.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var orderDetailsfromDB = _unitOfWork.OrderHeader.GetFirstorDefault(x => x.Id == orderViewModel.orderHeader.Id);
            if (orderDetailsfromDB != null) { 
                orderDetailsfromDB.Name= orderViewModel.orderHeader.Name;
                orderDetailsfromDB.PhoneNumber= orderViewModel.orderHeader.PhoneNumber;
                orderDetailsfromDB.City= orderViewModel.orderHeader.City;
                orderDetailsfromDB.StreetAddress= orderViewModel.orderHeader.StreetAddress;
                orderDetailsfromDB.PostalCode= orderViewModel.orderHeader.PostalCode;
                orderDetailsfromDB.State= orderViewModel.orderHeader.State;
                if(!string.IsNullOrEmpty(orderViewModel.orderHeader.Carrier))
                {
                    orderDetailsfromDB.Carrier= orderViewModel.orderHeader.Carrier;
                }
                if (!string.IsNullOrEmpty(orderViewModel.orderHeader.TrackingNumber)) { 
                    orderDetailsfromDB.TrackingNumber= orderViewModel.orderHeader.TrackingNumber;
                }
                _unitOfWork.OrderHeader.Update(orderDetailsfromDB);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Order Details Updated Successfully!";
            }
            return RedirectToAction(nameof(Details), new { orderId = orderDetailsfromDB.Id });
        }
        [HttpPost]
        [Authorize(Roles =StaticDetails.Role_Admin+","+StaticDetails.Role_Employee)]
        public IActionResult StartProcessing()
        {
            //Done only by Admin or Emloyee and changing the status of the order to Processing after clicking Start Processing
            _unitOfWork.OrderHeader.UpdateStatus(orderViewModel.orderHeader.Id, StaticDetails.StatusInProcess);
            _unitOfWork.Save();
            TempData["SuccessMessage"] = "Order Details Updated Successfully!";
            return RedirectToAction(nameof(Details), new { orderId = orderViewModel.orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult ShipOrder()
        {
            //Done only by Admin or Emloyee and changing the status of the order to Processing after clicking Start Processing
            var orderHeaderfromDB= _unitOfWork.OrderHeader.GetFirstorDefault(x => x.Id == orderViewModel.orderHeader.Id);
            orderHeaderfromDB.TrackingNumber = orderViewModel.orderHeader.TrackingNumber;
            orderHeaderfromDB.Carrier = orderViewModel.orderHeader.Carrier;
            orderHeaderfromDB.OrderStatus = StaticDetails.StatusShipped;
            orderHeaderfromDB.ShippingDate = DateTime.Now;
            if(orderHeaderfromDB.PaymentStatus== StaticDetails.PaymentStatusDelayedPayment)
            {
                orderHeaderfromDB.PaymentdueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeaderfromDB);
            _unitOfWork.Save();
            TempData["SuccessMessage"] = "Order Shipped Successfully!";
            return RedirectToAction(nameof(Details), new { orderId = orderViewModel.orderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeaderfromDB = _unitOfWork.OrderHeader.GetFirstorDefault(x => x.Id == orderViewModel.orderHeader.Id);
            if (orderHeaderfromDB.PaymentStatus == StaticDetails.PaymentStatusApproved) {
                //Money refund to the customer using Stripe API's
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderfromDB.PaymentIntentId,

                };
                var service=new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderfromDB.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderfromDB.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["SuccessMessage"] = "Order Cancelled Successfully!";
            return RedirectToAction(nameof(Details), new { orderId = orderViewModel.orderHeader.Id });
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult Details_Pay_Now()
        {
            orderViewModel.orderHeader=_unitOfWork.OrderHeader.GetFirstorDefault(x=>x.Id==orderViewModel.orderHeader.Id,includeProperty:"ApplicationUser");
            orderViewModel.orderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderViewModel.orderHeader.Id, includeProperty: "Product");
            //Stripe logic from googl

            var domain = "https://localhost:7154/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderViewModel.orderHeader.Id}",    //Redirecting to this URL doesn't give the details from stripe so we have to check the session id again in Order Confirmation Action
                CancelUrl = domain + $"admin/order/details?orderId={orderViewModel.orderHeader.Id}",

                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in orderViewModel.orderDetail)
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

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session sessionService = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(orderViewModel.orderHeader.Id, sessionService.Id,sessionService.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", sessionService.Url);
            return new StatusCodeResult(303);
            
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            var orderHeaderDetails = _unitOfWork.OrderHeader.GetFirstorDefault(x => x.Id == orderHeaderId);
            if (orderHeaderDetails.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                //means this order is from company

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeaderDetails.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeaderDetails.OrderStatus, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }

            }
            
           
            return View(orderHeaderDetails);
        }
    }
}
