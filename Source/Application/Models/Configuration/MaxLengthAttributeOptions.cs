using System.Collections.Generic;

namespace Application.Models.Configuration
{
	public class MaxLengthAttributeOptions
	{
		#region Properties

		public virtual IList<MaxLengthAttributeBreakpointOptions> Breakpoints { get; } = new List<MaxLengthAttributeBreakpointOptions>();

		/// <summary>
		/// Add a MaxLength attribute automatically with an argument calculated by the longest attribute value.
		/// </summary>
		public virtual bool Enabled { get; set; }

		#endregion
	}
}