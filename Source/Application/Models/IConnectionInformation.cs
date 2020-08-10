using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;

namespace Application.Models
{
	public interface IConnectionInformation
	{
		#region Properties

		IList<string> Attributes { get; }
		AuthType? AuthenticationType { get; }
		TimeSpan Duration { get; }
		string Filter { get; }
		int? Port { get; }
		int ProtocolVersion { get; }
		string RootDistinguishedName { get; }
		IList<string> Servers { get; }
		TimeSpan Timeout { get; }

		#endregion
	}
}