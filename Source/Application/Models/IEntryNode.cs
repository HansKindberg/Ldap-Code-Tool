using System.Collections.Generic;
using RegionOrebroLan.DirectoryServices.Protocols;

namespace Application.Models
{
	public interface IEntryNode
	{
		#region Properties

		IDictionary<string, IEntryNode> Children { get; }
		IEntry Entry { get; }
		IEntryNode Parent { get; }

		#endregion
	}
}