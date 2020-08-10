using System;
using System.Threading.Tasks;
using Application.Models.Forms;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Models.ViewComponents
{
	public class DirectoryInformationStatusViewComponent : ViewComponent
	{
		#region Constructors

		public DirectoryInformationStatusViewComponent(IFacade facade)
		{
			this.Facade = facade ?? throw new ArgumentNullException(nameof(facade));
		}

		#endregion

		#region Properties

		protected internal virtual IFacade Facade { get; }

		#endregion

		#region Methods

		protected internal virtual async Task<ConnectionForm> CreateForm()
		{
			var directoryOptions = this.Facade.DirectoryOptions.Value;

			return await Task.FromResult(new ConnectionForm
			{
				RootDistinguishedName = directoryOptions.RootDistinguishedName
			});
		}

		protected internal virtual async Task<DirectoryInformationStatusViewModel> CreateModel()
		{
			return new DirectoryInformationStatusViewModel
			{
				DirectoryInformation = this.Facade.DirectoryInformation,
				Form = await this.CreateForm()
			};
		}

		public virtual async Task<IViewComponentResult> InvokeAsync()
		{
			return this.View(await this.CreateModel());
		}

		#endregion
	}
}