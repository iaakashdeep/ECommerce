using ECommerceWeb.RazorPages.Data;
using ECommerceWeb.RazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceWeb.RazorPages.Pages.Categories
{
    [BindProperties]    //This will ensure all the properties inside this page will be binded
    public class CreateModel : PageModel
    {
        private readonly AppDBContext _AppDBContext;
        [BindProperty]          //Now this will inform the cshtml page to bind this property on Post, so that we don't need to define the category object as parameter in OnPost()
        public Category category { get; set; }  //In cshtml page the Model will access this property
        public CreateModel(AppDBContext appDBContext)
        {
            _AppDBContext = appDBContext;
        }
        public void OnGet()
        {

        }

        //public IActionResult OnPost(Category cat)
        //{
        //    //If we pass the object like in the parameter it will be null because in razor pages properties are not binded in the route
        //    _AppDBContext.Categories.Add(cat);
        //    _AppDBContext.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public IActionResult OnPost()
        {
            
            _AppDBContext.Categories.Add(category);
            _AppDBContext.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
