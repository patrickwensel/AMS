using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.ViewModels
{
	public class ApplicationViewModel
	{
		public int ID { get; set; }
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Application Name")]
		public string Name { get; set; }
	}
}