using System;
using Application.Models.Forms;

namespace Application.Models.ViewModels
{
	public class CodeViewModel
	{
		#region Properties

		public virtual string Code { get; set; }
		public virtual Exception Exception { get; set; }
		public virtual CodeOptionsForm Form { get; } = new CodeOptionsForm();

		#endregion
	}
}