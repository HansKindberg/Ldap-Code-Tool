using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Models;
using Application.Models.Forms;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
	public class CodeController : SiteController
	{
		#region Constructors

		public CodeController(IFacade facade) : base(facade) { }

		#endregion

		#region Methods

		[HttpPost]
		[ValidateAntiForgeryToken]
		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		public virtual async Task<IActionResult> Create(CodeOptionsForm form)
		{
			if(this.Facade.DirectoryInformation == null)
				return this.NotFound();

			var model = await this.CreateModel(form);

			try
			{
				model.Code = await this.Facade.CodeGenerator.GenerateAsync(form, this.Facade.DirectoryInformation);
			}
			catch(Exception exception)
			{
				model.Exception = new InvalidOperationException("Could not generate code.", exception);
			}

			return await Task.FromResult(this.View("Index", model));
		}

		protected internal virtual async Task<CodeViewModel> CreateModel(CodeOptionsForm form = null)
		{
			var model = new CodeViewModel();

			model.Form.Initialize(this.Facade.CodeOptions.Value, this.Facade.DirectoryInformation);

			// ReSharper disable InvertIf
			if(form != null)
			{
				model.Form.Attributes = form.Attributes;
				model.Form.CodeType = form.CodeType;
				model.Form.ObjectClasses = form.ObjectClasses;
				model.Form.TypeStructure = form.TypeStructure;
			}
			// ReSharper restore InvertIf

			return await Task.FromResult(model);
		}

		public virtual async Task<IActionResult> Index()
		{
			if(this.Facade.DirectoryInformation == null)
				return this.NotFound();

			var model = await this.CreateModel();

			return this.View(model);
		}

		#endregion
	}
}