using System.Collections.Generic;
using Application.Models.Code;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Models.Forms
{
	public class CodeOptionsForm : ICustomCodeOptions
	{
		#region Properties

		public virtual string BaseTypeName { get; set; }
		public virtual CodeType CodeType { get; set; }
		public virtual IList<SelectListItem> CodeTypeOptions { get; } = new List<SelectListItem>();
		public virtual bool MultiValueProperties { get; set; }
		public virtual IEnumerable<string> ObjectClasses { get; set; }
		public virtual IList<SelectListItem> ObjectClassOptions { get; } = new List<SelectListItem>();
		public virtual TypeStructure TypeStructure { get; set; }
		public virtual IList<SelectListItem> TypeStructureOptions { get; } = new List<SelectListItem>();
		public virtual IList<SelectListItem> UninterestingAttributeOptions { get; } = new List<SelectListItem>();
		public virtual IEnumerable<string> UninterestingAttributes { get; set; }

		#endregion
	}
}