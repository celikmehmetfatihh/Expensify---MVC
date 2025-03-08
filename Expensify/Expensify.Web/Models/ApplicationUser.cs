using Microsoft.AspNetCore.Identity;

namespace Expensify.Web.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; }  // Custom property

		public string? ManagerId { get; set; }  // Foreign key for Manager (self-referencing)
		public ApplicationUser? Manager { get; set; }  // Navigation property to the manager

		public ICollection<ApplicationUser>? Employees { get; set; }  // Collection of employees managed by this user
	}
}
