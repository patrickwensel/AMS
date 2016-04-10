using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Models;

namespace AMS.Models
{
	public class UserApplication
	{
		public int UserApplicationID { get; set; }
		public int ApplicationID { get; set; }
		public string UserID { get; set; }

		public virtual Application Application { get; set; }
	}
}