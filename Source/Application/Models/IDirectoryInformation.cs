using System.Collections.Generic;
using RegionOrebroLan.DirectoryServices.Protocols;

namespace Application.Models
{
	public interface IDirectoryInformation
	{
		#region Properties

		IDictionary<string, IAttributeInformation> Attributes { get; }
		IConnectionInformation Connection { get; }
		IDictionary<string, IEntry> Entries { get; }
		IDictionary<string, IEnumerable<IAttributeInformation>> ObjectClasses { get; }
		IEntryNode Tree { get; }

		#endregion
	}
}