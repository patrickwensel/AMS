using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AMS.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool IsClient { get; set; }

		public int? ClientID { get; set; }
		public bool Active { get; set; }

		public IEnumerable<SelectListItem> RolesList { get; set; }

		public IEnumerable<Application> UserApplications { get; set; }

        public IEnumerable<Application> Applications { get; set; }

        public IEnumerable<int> SelectedApplications { get; set; }

    }
}