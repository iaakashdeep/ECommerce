using ECommerce.DataAccess.Data;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ECommerceWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
                _context = context;
        }
        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category) 
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order can not exactly match the Category name.");
            }
            //if (category.Name == "Test")
            //{
            //    ModelState.AddModelError("", "Category name can not be Test");
            //}
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? categoryId)
        {
            if (categoryId == null || categoryId == 0) {
                return NotFound();
            }
            //var detCategory = _context.Categories.Where(x => x.Id == categoryId).FirstOrDefault();   OR
            var detCategory = _context.Categories.FirstOrDefault(x => x.Id == categoryId); //We can use FirstOrdefault in any case not limited to primary key
            //OR
            //var detCategory = _context.Categories.Find(categoryId);   //Find will only work in case of primary key
            if (detCategory != null)
            {
                return View(detCategory);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {

            ///Note: if we will not pass <input asp-for="Id" hidden /> then this method will consider the Id as 0 and the Update method in EF core will consider as a new record and insert the new record instead of modfying the existing
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category Modified Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int? categoryId)
        {
            if (categoryId == null || categoryId == 0) {
                return NotFound();
            }
            var category = _context.Categories.FirstOrDefault(x=>x.Id == categoryId);
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? categoryId)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == categoryId);
            if (category != null) {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category Delete Successfully";        //Passing data for 1 request only
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
