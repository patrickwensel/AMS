﻿using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;

namespace AMS.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(IUserStore<User> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<User>(context.Get<AMSContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<User>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<User>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole,string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<AMSContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<AMSContext> 
    {
        protected override void Seed(AMSContext context) {
            InitializeIdentityForEF(context);
            base.Seed(context);

	        var applications = new List<Application>
	        {
				new Application {ApplicationID = 1, Name = "Application 1"},
				new Application {ApplicationID = 2, Name = "Application 2"}
			};

			applications.ForEach(a=> context.Applications.Add(a));
	        context.SaveChanges();

	        var clients = new List<Client>
	        {
		        new Client {Name = "Client Number One", ClientCode = "Client1"},
		        new Client {Name = "Client Number Two", ClientCode = "Client2"},
		        new Client {Name = "Client Number Three", ClientCode = "Client3"},
	        };

			clients.ForEach(c=>context.Clients.Add(c));
	        context.SaveChanges();
        }

		//Create User=dwadek@hotmail.com with password=Admin@123456 in the Admin role        
		public static void InitializeIdentityForEF(AMSContext db) {
			var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
			const string adminUserName = "dwalter";
			const string adminEmail = "dwadek@hotmail.com";
			string password = "Admin@123456";
			const string roleName = "Admin";
			string standardRoleName = "Standard User";

			//Create Role Admin if it does not exist
			var role = roleManager.FindByName(roleName);
			if (role == null)
			{
				role = new IdentityRole(roleName);
				var roleresult = roleManager.Create(role);
			}

			//Create Standard User Admin if it does not exist
			var standardUserRole = roleManager.FindByName(standardRoleName);
			if (standardUserRole == null)
			{
				standardUserRole = new IdentityRole(standardRoleName);
				var roleresult = roleManager.Create(standardUserRole);
			}

			//Create Admin User
			var user = userManager.FindByName(adminEmail);
			if (user == null)
			{
				user = new User
				{
					UserName = adminUserName,
					Email = adminEmail,
					FirstName = "David",
					LastName = "Walter"
				};
				var result = userManager.Create(user, password);
				result = userManager.SetLockoutEnabled(user.Id, false);
			}

			// Add user admin to Role Admin if not already added
			var rolesForUser = userManager.GetRoles(user.Id);
			if (!rolesForUser.Contains(role.Name))
			{
				var result = userManager.AddToRole(user.Id, role.Name);
			}

			//Create Sample users
			for (int i = 0; i < 10; i++)
			{
				var standardUserName = "suser" + i;
				var standardUserEmail = "StandardUser" + i + "@example.com";
				password = "Standard@123456";

				user = userManager.FindByName(standardUserName);
				if (user == null)
				{
					user = new User
					{
						UserName = standardUserName,
						Email = standardUserEmail,
						FirstName = "Standard",
						LastName = "User" + i
					};
					var result = userManager.Create(user, password);
					result = userManager.SetLockoutEnabled(user.Id, false);

				}

				// Add user admin to Role Admin if not already added
				rolesForUser = userManager.GetRoles(user.Id);
				if (!rolesForUser.Contains(standardUserRole.Name))
				{
					var result = userManager.AddToRole(user.Id, standardUserRole.Name);
				}
			}
		}
    }

    public class ApplicationSignInManager : SignInManager<User, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}