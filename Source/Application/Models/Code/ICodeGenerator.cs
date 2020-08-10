using System.Threading.Tasks;

namespace Application.Models.Code
{
	public interface ICodeGenerator
	{
		#region Methods

		Task<string> GenerateAsync(ICustomCodeOptions customOptions, IDirectoryInformation directoryInformation);

		#endregion
	}
}