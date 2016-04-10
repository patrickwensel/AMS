using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Models;

namespace AMS
{
	public class Common
	{
		private AMSContext db = new AMSContext();
		public List<SelectListItem> PopulateClients()
		{
			List<SelectListItem> clients = db.Clients.ToList().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.ClientID.ToString()
			}).ToList();

			clients.Insert(0, (new SelectListItem { Text = "-- Select Client --", Value = null }));

			return clients;

		}
	}
}