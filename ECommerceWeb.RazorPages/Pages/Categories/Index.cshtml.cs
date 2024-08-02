using ECommerceWeb.RazorPages.Data;
using ECommerceWeb.RazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceWeb.RazorPages.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDBContext _AppDBContext;
        public List<Category> lstCat { get; set; }  //In cshtml page the Model will access this property
        public IndexModel(AppDBContext appDBContext)
        {
            _AppDBContext = appDBContext;
        }
        public void OnGet()
        {
          lstCat= _AppDBContext.Categories.ToList();
        }
    }
}
