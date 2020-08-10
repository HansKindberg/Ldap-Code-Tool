namespace Application.Models.Forms
{
	public class ConnectionForm : ReturnForm
	{
		#region Properties

		public virtual string Attributes { get; set; }
		public virtual bool ClearBaseFilter { get; set; }
		public virtual string Filter { get; set; }
		public virtual string RootDistinguishedName { get; set; }
		public virtual int? Timeout { get; set; }

		#endregion
	}
}