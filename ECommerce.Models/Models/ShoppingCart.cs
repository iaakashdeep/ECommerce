using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1,1000,ErrorMessage ="Please enter value between 1-1000")]
        public int Count {  get; set; }
        public string ApplicationUserId {  get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUsers ApplicationUsers { get; set; }

        [NotMapped]             //This will tell that this property doesn't needs to be included in DB
        public double Price {  get; set; }   //Adding this property to show the total price after calculation but that doesn't need to be stored in DB
    }
}
