using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
	public class ObjectClassesController : SiteController
	{
		#region Constructors

		public ObjectClassesController(IFacade facade) : base(facade) { }

		#endregion

		#region Methods

		public virtual async Task<IActionResult> Index(string id)
		{
			if(this.Facade.DirectoryInformation == null)
				return this.NotFound();

			var model = new ObjectClassesViewModel();

			if(!string.IsNullOrEmpty(id))
			{
				foreach(var (key, attributeInformations) in this.Facade.DirectoryInformation.ObjectClasses)
				{
					// ReSharper disable InvertIf
					if(key.Equals(id, StringComparison.OrdinalIgnoreCase))
					{
						model.ObjectClass = new KeyValuePair<string, IEnumerable<IAttributeInformation>>(key, attributeInformations);
						break;
					}
					// ReSharper restore InvertIf
				}

				if(model.ObjectClass == null)
					return this.NotFound();
			}
			else
			{
				foreach(var (key, attributeInformations) in this.Facade.DirectoryInformation.ObjectClasses)
				{
					model.ObjectClasses.Add(key, attributeInformations);
				}
			}

			return await Task.FromResult(this.View(model));
		}

		#endregion
	}
}