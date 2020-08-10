using System;
using System.Threading.Tasks;
using Application.Models;
using Application.Models.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
	public class DirectoryInformationController : SiteController
	{
		#region Constructors

		public DirectoryInformationController(IFacade facade) : base(facade) { }

		#endregion

		#region Methods

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Clear(ReturnForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			this.Facade.ClearDirectoryInformation();

			return this.Redirect(await this.GetResolvedReturnUrlAsync(form.ReturnUrl));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Connect(ConnectionForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			this.Facade.SetDirectoryInformation(form);

			return this.Redirect(await this.GetResolvedReturnUrlAsync(form.ReturnUrl));
		}

		protected internal virtual async Task<string> GetResolvedReturnUrlAsync(string returnUrl)
		{
			if(!string.IsNullOrWhiteSpace(returnUrl) && Uri.TryCreate(returnUrl, UriKind.Relative, out var url) && !url.IsAbsoluteUri)
				return returnUrl;

			return await Task.FromResult("/");
		}

		#endregion
	}
}