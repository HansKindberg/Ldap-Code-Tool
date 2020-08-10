using System;
using System.Collections.Generic;

namespace Application.Models.ViewModels
{
	public class AttributesViewModel
	{
		#region Properties

		public virtual IAttributeInformation Attribute { get; set; }
		public virtual IDictionary<string, IAttributeInformation> Attributes { get; } = new SortedDictionary<string, IAttributeInformation>(StringComparer.OrdinalIgnoreCase);

		#endregion
	}
}