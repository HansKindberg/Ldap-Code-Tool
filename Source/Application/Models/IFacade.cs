using Application.Models.Code;
using Application.Models.Configuration;
using Application.Models.Forms;
using Microsoft.Extensions.Options;
using RegionOrebroLan.DirectoryServices.Protocols.Configuration;

namespace Application.Models
{
	public interface IFacade
	{
		#region Properties

		ICodeGenerator CodeGenerator { get; }
		IOptions<CodeOptions> CodeOptions { get; }
		IDirectoryInformation DirectoryInformation { get; }
		IOptions<DirectoryOptions> DirectoryOptions { get; }

		#endregion

		#region Methods

		void ClearDirectoryInformation();
		void SetDirectoryInformation(ConnectionForm form);

		#endregion
	}
}