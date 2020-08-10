using System;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
	public abstract class SiteController : Controller
	{
		#region Constructors

		protected SiteController(IFacade facade)
		{
			this.Facade = facade ?? throw new ArgumentNullException(nameof(facade));
		}

		#endregion

		#region Properties

		protected internal virtual IFacade Facade { get; }

		#endregion
	}
}