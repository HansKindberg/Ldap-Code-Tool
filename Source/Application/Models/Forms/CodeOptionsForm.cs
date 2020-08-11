using System;
using System.Collections.Generic;
using System.Linq;
using Application.Models.Code;
using Application.Models.Configuration;
using Application.Models.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Models.Forms
{
	public class CodeOptionsForm : ICustomCodeOptions
	{
		#region Properties

		public virtual IList<SelectListItem> AttributeOptions { get; } = new List<SelectListItem>();
		public virtual IEnumerable<string> Attributes { get; set; }
		public virtual string BaseTypeName { get; set; }
		public virtual CodeType CodeType { get; set; }
		public virtual IList<SelectListItem> CodeTypeOptions { get; } = new List<SelectListItem>();
		public virtual bool MultiValueProperties { get; set; }
		public virtual IEnumerable<string> ObjectClasses { get; set; }
		public virtual IList<SelectListItem> ObjectClassOptions { get; } = new List<SelectListItem>();
		public virtual TypeStructure TypeStructure { get; set; }
		public virtual IList<SelectListItem> TypeStructureOptions { get; } = new List<SelectListItem>();

		#endregion

		#region Methods

		protected internal virtual SelectListItem CreateSelectListItem(string value)
		{
			var selectListItem = new SelectListItem();

			selectListItem.Text = selectListItem.Value = value;

			return selectListItem;
		}

		public virtual void Initialize(CodeOptions codeOptions, IDirectoryInformation directoryInformation)
		{
			if(codeOptions == null)
				throw new ArgumentNullException(nameof(codeOptions));

			if(directoryInformation == null)
				throw new ArgumentNullException(nameof(directoryInformation));

			foreach(var (attribute, _) in directoryInformation.Attributes)
			{
				var option = this.CreateSelectListItem(attribute);

				foreach(var pattern in codeOptions.DefaultAttributes)
				{
					// ReSharper disable InvertIf
					if(attribute.Like(pattern))
					{
						option.Selected = true;
						break;
					}
					// ReSharper restore InvertIf
				}

				this.AttributeOptions.Add(option);
			}

			this.Attributes = this.AttributeOptions.Where(option => option.Selected).Select(option => option.Value).ToArray();

			this.BaseTypeName = codeOptions.BaseTypeName;

			foreach(var codeType in Enum.GetNames(typeof(CodeType)))
			{
				this.CodeTypeOptions.Add(this.CreateSelectListItem(codeType));
			}

			foreach(var (objectClass, _) in directoryInformation.ObjectClasses)
			{
				var option = this.CreateSelectListItem(objectClass);

				foreach(var pattern in codeOptions.DefaultObjectClasses)
				{
					// ReSharper disable InvertIf
					if(objectClass.Like(pattern))
					{
						option.Selected = true;
						break;
					}
					// ReSharper restore InvertIf
				}

				this.ObjectClassOptions.Add(option);
			}

			this.ObjectClasses = this.ObjectClassOptions.Where(option => option.Selected).Select(option => option.Value).ToArray();

			foreach(var typeStructure in Enum.GetNames(typeof(TypeStructure)))
			{
				this.TypeStructureOptions.Add(this.CreateSelectListItem(typeStructure));
			}
		}

		#endregion
	}
}