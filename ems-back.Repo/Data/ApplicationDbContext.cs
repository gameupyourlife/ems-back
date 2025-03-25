using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ems_Back.Repo.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}

	// Custom user class (optional, extend it as needed)
	public class ApplicationUser : IdentityUser
	{
		// Add custom user properties if needed, e.g., FirstName, LastName, etc.
	}
}
