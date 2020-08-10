using System;
using System.Collections.Generic;

namespace Application.Models.ViewModels
{
	public class ObjectClassesViewModel
	{
		#region Properties

		public virtual KeyValuePair<string, IEnumerable<IAttributeInformation>>? ObjectClass { get; set; }
		public virtual IDictionary<string, IEnumerable<IAttributeInformation>> ObjectClasses { get; } = new SortedDictionary<string, IEnumerable<IAttributeInformation>>(StringComparer.OrdinalIgnoreCase);

		#endregion
	}
}