using ECommerceWeb.RazorPages.Data;
using ECommerceWeb.RazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceWeb.RazorPages.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly AppDBContext _AppDBContext;
        [BindProperty]
        public Category? category { get; set; }
        public DeleteModel(AppDBContext appDBContext)
        {
            _AppDBContext = appDBContext;
        }
        public void OnGet(int? categoryId)
        {
            if (categoryId != null && categoryId != 0) {
                category = _AppDBContext.Categories.Find(categoryId);
            }
        }

        public IActionResult OnPost() {
            if (category != null) { 
            _AppDBContext.Categories.Remove(category);
                _AppDBContext.SaveChanges();
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
