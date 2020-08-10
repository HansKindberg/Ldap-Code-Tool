using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;

namespace Application.Models
{
	public class ConnectionInformation : IConnectionInformation
	{
		#region Properties

		public virtual IList<string> Attributes { get; } = new List<string>();
		public virtual AuthType? AuthenticationType { get; set; }
		public virtual TimeSpan Duration { get; set; }
		public virtual string Filter { get; set; }
		public virtual int? Port { get; set; }
		public virtual int ProtocolVersion { get; set; }
		public virtual string RootDistinguishedName { get; set; }
		public virtual IList<string> Servers { get; } = new List<string>();
		public virtual TimeSpan Timeout { get; set; }

		#endregion
	}
}