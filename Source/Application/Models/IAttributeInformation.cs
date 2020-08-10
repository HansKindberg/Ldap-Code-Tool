using System.Collections.Generic;

namespace Application.Models
{
	public interface IAttributeInformation
	{
		#region Properties

		int HighestNumberOfValues { get; }
		string HighestNumberOfValuesDistinguishedName { get; }
		int LongestTotalValueLength { get; }
		string LongestTotalValueLengthDistinguishedName { get; }
		string Name { get; }
		IEnumerable<string> ObjectClassesThatThisAttributeExistsAt { get; }

		#endregion
	}
}