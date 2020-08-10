using System;
using System.Collections.Generic;

namespace Application.Models
{
	public class AttributeInformation : IAttributeInformation
	{
		#region Properties

		public virtual int HighestNumberOfValues { get; set; }
		public virtual string HighestNumberOfValuesDistinguishedName { get; set; }
		public virtual int LongestTotalValueLength { get; set; }
		public virtual string LongestTotalValueLengthDistinguishedName { get; set; }
		public virtual string Name { get; set; }
		IEnumerable<string> IAttributeInformation.ObjectClassesThatThisAttributeExistsAt => this.ObjectClassesThatThisAttributeExistsAt;
		public virtual ISet<string> ObjectClassesThatThisAttributeExistsAt { get; } = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

		#endregion
	}
}