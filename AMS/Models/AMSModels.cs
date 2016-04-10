using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AMS.Models
{
	public class AMSContext : IdentityDbContext<User>
	{
		public AMSContext()
			: base("AMSContext", throwIfV1Schema: false)
		{
		}

		static AMSContext()
		{
			Database.SetInitializer<AMSContext>(new ApplicationDbInitializer());
		}

		public static AMSContext Create()
		{
			return new AMSContext();
		}

		public virtual DbSet<Application> Applications { get; set; }
		public virtual DbSet<Client> Clients { get; set; }
		public virtual DbSet<UserApplication> UserApplications { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<IdentityUser>()
				.ToTable("Users");

			modelBuilder.Entity<User>()
				.ToTable("Users");

			modelBuilder.Entity<IdentityUser>()
				.HasMany(u => u.Roles)
				.WithOptional()
				.HasForeignKey(r => r.UserId);

			modelBuilder.Entity<IdentityUser>()
				.HasMany(u => u.Logins)
				.WithOptional()
				.HasForeignKey(l => l.UserId);

			modelBuilder.Entity<IdentityUser>()
				.HasMany(u => u.Claims)
				.WithOptional()
				.HasForeignKey(c => c.UserId);
		}
	}
}