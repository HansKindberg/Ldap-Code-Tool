using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Models;
using Application.Models.Code;
using Application.Models.Forms;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

			await this.InitializeForm(model.Form);

			if(form != null)
			{
				model.Form.CodeType = form.CodeType;
				model.Form.ObjectClasses = form.ObjectClasses;
				model.Form.TypeStructure = form.TypeStructure;
				model.Form.UninterestingAttributes = form.UninterestingAttributes;
			}

			return model;
		}

		protected internal virtual SelectListItem CreateSelectListItem(string value)
		{
			var selectListItem = new SelectListItem();

			selectListItem.Text = selectListItem.Value = value;

			return selectListItem;
		}

		public virtual async Task<IActionResult> Index()
		{
			if(this.Facade.DirectoryInformation == null)
				return this.NotFound();

			var model = await this.CreateModel();

			return this.View(model);
		}

		protected internal virtual async Task InitializeForm(CodeOptionsForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			await Task.CompletedTask;

			var codeOptions = this.Facade.CodeOptions.Value;

			form.BaseTypeName = codeOptions.BaseTypeName;

			foreach(var codeType in Enum.GetNames(typeof(CodeType)))
			{
				form.CodeTypeOptions.Add(this.CreateSelectListItem(codeType));
			}

			foreach(var (objectClass, _) in this.Facade.DirectoryInformation.ObjectClasses)
			{
				form.ObjectClassOptions.Add(this.CreateSelectListItem(objectClass));
			}

			foreach(var typeStructure in Enum.GetNames(typeof(TypeStructure)))
			{
				form.TypeStructureOptions.Add(this.CreateSelectListItem(typeStructure));
			}

			foreach(var (attribute, _) in this.Facade.DirectoryInformation.Attributes)
			{
				var option = this.CreateSelectListItem(attribute);

				if(codeOptions.UninterestingAttributes.Contains(attribute))
					option.Selected = true;

				form.UninterestingAttributeOptions.Add(option);
			}
		}

		#endregion
	}
}