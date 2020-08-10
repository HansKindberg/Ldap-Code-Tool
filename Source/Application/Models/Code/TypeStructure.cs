using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Code
{
	[SuppressMessage("Naming", "CA1720:Identifier contains type name")]
	public enum TypeStructure
	{
		Single,
		Multiple,
		MultipleWithBaseType
	}
}