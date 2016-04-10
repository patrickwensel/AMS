using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Models;
using AMS.ViewModels;
using AutoMapper;

namespace AMS
{
	public static class AutoMapperConfig
	{
		public static void RegisterMappings()
		{
			Mapper.CreateMap<Client, ClientViewModel>();
			Mapper.CreateMap<ClientViewModel, Client>();
			Mapper.CreateMap<Application, ApplicationViewModel>();
			Mapper.CreateMap<ApplicationViewModel, Application>();
		}
	}
}