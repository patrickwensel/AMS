using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.ViewModels
{
	public class ClientViewModel
	{
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		public string ClientCode { get; set; }
	}
}
