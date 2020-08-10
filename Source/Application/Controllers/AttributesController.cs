using System.Threading.Tasks;
using Application.Models;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
	public class AttributesController : SiteController
	{
		#region Constructors

		public AttributesController(IFacade facade) : base(facade) { }

		#endregion

		#region Methods

		public virtual async Task<IActionResult> Index(string id)
		{
			if(this.Facade.DirectoryInformation == null)
				return this.NotFound();

			var model = new AttributesViewModel();

			if(!string.IsNullOrEmpty(id))
			{
				if(!this.Facade.DirectoryInformation.Attributes.TryGetValue(id, out var attribute))
					return this.NotFound();

				model.Attribute = attribute;
			}
			else
			{
				foreach(var (key, attributeInformation) in this.Facade.DirectoryInformation.Attributes)
				{
					model.Attributes.Add(key, attributeInformation);
				}
			}

			return await Task.FromResult(this.View(model));
		}

		#endregion
	}
}