using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.RazorPages.Models
{
    public class Category
    {
        public int Id { get; set; }     //When adding migration it will consdier Id automatically as PK for any table, if we give any other name apart from Id we have to define
        [MaxLength(20, ErrorMessage = "Category Name must be 20 characters long")]                                //[Key] attribute above that
        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }
        [Range(1, 100, ErrorMessage = "Order must be between 1-100")]
        [Required(ErrorMessage = "Display Order is required")]
        public int DisplayOrder { get; set; }
    }
}
