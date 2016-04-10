using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AMS.Models
{
	public class User : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool IsClient { get; set; }
		public int? ClientID { get; set; }
		public virtual Client Client { get; set; }
		public bool Active { get; set; }
		public virtual List<UserApplication> UserApplications { get; set; } 
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
		{
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

			return userIdentity;
		}
	}

}