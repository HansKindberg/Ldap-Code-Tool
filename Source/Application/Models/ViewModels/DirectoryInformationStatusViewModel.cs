using Application.Models.Forms;

namespace Application.Models.ViewModels
{
	public class DirectoryInformationStatusViewModel
	{
		#region Properties

		public virtual IDirectoryInformation DirectoryInformation { get; set; }
		public virtual ConnectionForm Form { get; set; } = new ConnectionForm();

		#endregion
	}
}