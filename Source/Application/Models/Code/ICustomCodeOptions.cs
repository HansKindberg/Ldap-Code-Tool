using System.Collections.Generic;

namespace Application.Models.Code
{
	public interface ICustomCodeOptions
	{
		#region Properties

		string BaseTypeName { get; set; }
		CodeType CodeType { get; set; }
		bool MultiValueProperties { get; set; }
		IEnumerable<string> ObjectClasses { get; set; }
		TypeStructure TypeStructure { get; set; }
		IEnumerable<string> UninterestingAttributes { get; set; }

		#endregion
	}
}