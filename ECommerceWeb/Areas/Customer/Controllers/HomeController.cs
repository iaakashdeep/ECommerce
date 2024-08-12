using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            var prodList = _unitOfWork.Product.GetAll(includeProperty: "Category");
            return View(prodList);
        }

        public IActionResult Details(int productId)
        {
            var productDetails = _unitOfWork.Product.GetFirstorDefault(x => x.Id == productId, includeProperty: "Category");
            return View(productDetails);

            //@Model.ListPrice.ToString("c") to convert the price into currency
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
