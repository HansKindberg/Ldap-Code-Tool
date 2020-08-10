using System.Collections.Generic;

namespace Application.Models.ViewModels
{
	public class EntriesViewModel
	{
		#region Properties

		public virtual IList<IEntryNode> Ancestors { get; } = new List<IEntryNode>();
		public virtual IEntryNode EntryNode { get; set; }

		#endregion
	}
}