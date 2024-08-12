using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        [ValidateNever]         //because at runtime Model State will also validate dropdown which is not needed
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}

//this namespace will come when we add <FrameworkReference Include="Microsoft.AspNetCore.App"></FrameworkReference> to the project file of Model otherwise it will ask to 
//install the depreciated nuget package for SelectListItem class
