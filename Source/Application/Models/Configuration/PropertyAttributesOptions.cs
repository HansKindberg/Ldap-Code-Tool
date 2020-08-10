using System.Collections.Generic;

namespace Application.Models.Configuration
{
	public class PropertyAttributesOptions
	{
		#region Properties

		public virtual IList<PropertyAttributeOptions> Items { get; } = new List<PropertyAttributeOptions>();
		public virtual MaxLengthAttributeOptions MaxLength { get; set; } = new MaxLengthAttributeOptions();

		#endregion
	}
}