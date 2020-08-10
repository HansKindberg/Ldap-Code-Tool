using System.Collections.Generic;

namespace Application.Models.Configuration
{
	public class PropertyAttributeOptions
	{
		#region Properties

		public virtual string Argument { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<string> Patterns { get; } = new List<string>();

		#endregion
	}
}