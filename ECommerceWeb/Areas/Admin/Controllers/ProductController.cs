using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.FileProviders;

namespace ECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;     //built in service we directly need to inject use for accessing the contents from the web, in this case accessing the file we are uploading
        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var lstProducts = _unitOfWork.Product.GetAll(includeProperty:"Category");
            return View(lstProducts);
        }
        [HttpGet]
        public IActionResult Upsert(int? productId)
        {
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            //SelectListItem generally used for rendering dropdown elements
            //This is also an example of EF Core projections where we are filtering data dynamically from the table.
            //We can't pass categorylist to the view because the view is already binded with Product model.

            //ViewBag.Category = categoryList;

            //============OR===================

            //ViewData["Category"]=categoryList;
            ProductViewModel pvm = new()
            {
                Product = new Product(),
                CategoryList = categoryList
            };

            if (productId == null || productId == 0)
            {

                return View(pvm);
            }
            else {
                pvm.Product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == productId);
                if (pvm != null) { return View(pvm); }
                return View();
            }
            

        }
        [HttpPost]
        public IActionResult Upsert(ProductViewModel productVM, IFormFile? file)        //IFormfile will check any file is uploaded in the form, we have to define enctype="multipart/form-data" in the form tage of View
        {
                if (ModelState.IsValid)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;       //this will basically returns the www root folder path
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);     //Will append the filename coming from the form to the Guid
                        string productPath = Path.Combine(wwwRootPath, @"images\product");        //wwwRootPath will give the path for www location appending images\product will give full product path where we want to upload the file

                        if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);        //Copying the file to fileStream which has full path of where to store the file
                        }

                        productVM.Product.ImageUrl = @"\images\product\" + fileName;
                    }
                    if (productVM.Product.Id != 0)
                    {
                        _unitOfWork.Product.Update(productVM.Product);
                    }
                    else
                    {
                        _unitOfWork.Product.Add(productVM.Product);
                    }

                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                else
                {
                    productVM.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });

                    //This else condition data is for dropdown binding because the model will throw error if ValidateNever attribute is not set, because the dropdown will not bind when error comes or when page reloads
                }
                return View(productVM);
        }
    
        //[HttpGet]
        //public IActionResult Delete(int productId)
        //{
        //    if (productId == 0) {  return NotFound(); }

        //    var productDetails = _unitOfWork.Product.GetFirstorDefault(x => x.Id == productId);
        //    if (productDetails != null) { return View(productDetails); }
        //    return View();
            
        //}
        //[HttpPost,ActionName("Delete")]
        //public IActionResult DeletePost(int productId)
        //{
        //    var productDetails= _unitOfWork.Product.GetFirstorDefault(y => y.Id == productId);
        //    if (productDetails != null) {
        //        _unitOfWork.Product.Remove(productDetails);
        //        _unitOfWork.Save();
        //        return RedirectToAction("Index");
        //    }
        //    return NotFound();
        //}

        #region API Calls
        /// <summary>
        /// This will act as API when you call by: admin/product/getproducts this will give the result because MVC has in built support for api
        /// </summary>
        /// <returns></returns>
        public IActionResult GetProducts()
        {
            var lstProducts = _unitOfWork.Product.GetAll(includeProperty: "Category").ToList();
            return Json(new {data=lstProducts});
        }
        //[HttpGet]
        //public IActionResult Delete(int productId)
        //{
        //    if (productId == 0) { return NotFound(); }

        //    var productDetails = _unitOfWork.Product.GetFirstorDefault(x => x.Id == productId);
        //    if (productDetails == null) { return Json(new { success = false, message = "Error while deleting" }); }
        //    return Json(new { success = true, message = "Deletion" });
        //}

        [HttpDelete]
        public IActionResult Delete(int? productId)
        {
            var productDetails = _unitOfWork.Product.GetFirstorDefault(x => x.Id == productId);
            if (productDetails != null) {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productDetails.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                _unitOfWork.Product.Remove(productDetails);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Product deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
        }

        #endregion
    }
}
