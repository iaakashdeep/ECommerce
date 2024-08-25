using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models.Models
{
	public class OrderHeader
	{
		
        public int Id { get; set; }
		public string ApplicationUserId { get; set; }
		[ForeignKey(nameof(ApplicationUserId))]
		[ValidateNever]
		public ApplicationUsers ApplicationUser { get; set; }

		public DateTime OrderDate { get; set; }
		public DateTime ShippingDate { get; set; }
		public double OrderTotal {  get; set; }

		public string? OrderStatus {  get; set; }
		public string? PaymentStatus { get; set; }
		public string? TrackingNumber { get; set; }
		public string? Carrier { get; set; }

		public DateTime PaymentDate { get; set; }
		public DateOnly PaymentdueDate { get; set; }		//Gives only Date part introduced in .NET 8

		public string? SessionId { get; set; }  //When stripe return a status for payment confirmation this will be checked by the session
		public string? PaymentIntentId { get; set; }		//As we will get an ID from payment processor provider that will be stored in DB

		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string StreetAddress { get; set; }
		[Required]
		public string City { get; set; }
		[Required]
		public string State { get; set; }
		[Required]
		public string PostalCode { get; set; }
		[Required]
		public string Name { get; set; }


	}
}
