using AMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
		private AMSContext db = new AMSContext();

		public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
			Common common = new Common();
			ViewData["clients"] = common.PopulateClients();

			if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            EditUserViewModel model = new EditUserViewModel()
            {
                Id = user.Id,
				UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsClient = user.IsClient,
                ClientID = user.ClientID.GetValueOrDefault(),
				Active = user.Active
            };

            model.RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            {
                Selected = userRoles.Contains(x.Name),
                Text = x.Name,
                Value = x.Name
            }).ToList();

            model.Applications = db.Applications.ToList();

            var userApplications = db.UserApplications
                                     .Where(ua => ua.UserID == user.Id)
                                     .ToList();

            model.UserApplications = userApplications.Select(ua => ua.ApplicationID)
                                                     .Distinct()
                                                     .Select(aid => new Application { ApplicationID = aid, Name = id.ToString() })
                                                     .ToList();
            return View(model);
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
	            var user = await UserManager.FindByIdAsync(editUser.Id);
	            if (user == null) return HttpNotFound();

	            user.Id = editUser.Id;
	            //user.UserName = editUser.UserName; // if email is NOT exact, this will cause new user.
				user.Email = editUser.Email;
				user.FirstName = editUser.FirstName;
				user.LastName = editUser.LastName;
				user.IsClient = editUser.IsClient;
	            user.ClientID = editUser.ClientID;
	            user.Active = editUser.Active;
				// Update User Information
	            await UserManager.UpdateAsync(user);

				// Client / Roles
	            var client = db.Clients.FirstOrDefaultAsync(x => x.ClientID == editUser.ClientID);
	            if (client != null)
	            {
					selectedRole = selectedRole ?? new string[] { };
					var existRoles = await UserManager.GetRolesAsync(user.Id);
                    
                    // add new roles
			        var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(existRoles).ToArray());

                    // remove deleted roles
                    if (result.Succeeded)
                        result = await UserManager.RemoveFromRolesAsync(user.Id, existRoles.Where(ur => !selectedRole.Contains(ur)).ToArray());

                    if (!result.Succeeded)
					{
						ModelState.AddModelError("", result.Errors.First());
						return View();
					}
				}

                
                // Applications
                {
                    editUser.SelectedApplications = editUser.SelectedApplications ?? new int[] { };

                    List<UserApplication> existUserApplications = db.UserApplications.Where(ua => ua.UserID == editUser.Id).ToList();
                    var userApplicationsToDelete = existUserApplications.Where(ua => !editUser.SelectedApplications.Contains(ua.ApplicationID));
                    var userApplicationIdToAdd = editUser.SelectedApplications.Except(existUserApplications.Select(ua => ua.ApplicationID));

                    db.UserApplications.RemoveRange(userApplicationsToDelete);
                    db.UserApplications.AddRange(userApplicationIdToAdd.Select(uaid => new UserApplication
                    {
                        ApplicationID = uaid,
                        UserID = editUser.Id
                    }));
                }

                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
			ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
	}
}
