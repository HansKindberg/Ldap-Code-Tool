using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using Application.Models.Code;
using Application.Models.Configuration;
using Application.Models.Forms;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RegionOrebroLan.DirectoryServices.Protocols;
using RegionOrebroLan.DirectoryServices.Protocols.Configuration;

namespace Application.Models
{
	public class Facade : IFacade
	{
		#region Constructors

		public Facade(IMemoryCache cache, ICodeGenerator codeGenerator, IOptions<CodeOptions> codeOptions, IDirectory directory, IOptions<DirectoryOptions> directoryOptions, ISystemClock systemClock)
		{
			this.Cache = cache ?? throw new ArgumentNullException(nameof(cache));
			this.CodeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
			this.CodeOptions = codeOptions ?? throw new ArgumentNullException(nameof(codeOptions));
			this.Directory = directory ?? throw new ArgumentNullException(nameof(directory));
			this.DirectoryOptions = directoryOptions ?? throw new ArgumentNullException(nameof(directoryOptions));
			this.SystemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		}

		#endregion

		#region Properties

		protected internal virtual IMemoryCache Cache { get; }
		public virtual ICodeGenerator CodeGenerator { get; }
		public virtual IOptions<CodeOptions> CodeOptions { get; }
		protected internal virtual IDirectory Directory { get; }

		public virtual IDirectoryInformation DirectoryInformation
		{
			get => this.Cache.Get<IDirectoryInformation>(nameof(this.DirectoryInformation));
			protected internal set => this.Cache.Set(nameof(DirectoryInformation), value);
		}

		public virtual IOptions<DirectoryOptions> DirectoryOptions { get; }
		protected internal virtual ISystemClock SystemClock { get; }

		#endregion

		#region Methods

		public virtual void ClearDirectoryInformation()
		{
			this.Cache.Remove(nameof(DirectoryInformation));
		}

		protected internal virtual ConnectionInformation CreateConnectionInformation(IFindOptions options)
		{
			if(options == null)
				throw new ArgumentNullException(nameof(options));

			var ldapDirectoryIdentifier = options.Connection.Directory as LdapDirectoryIdentifier;

			var connectionInformation = new ConnectionInformation
			{
				AuthenticationType = options.Connection.AuthType,
				Filter = options.FilterBuilder.Build(),
				Port = ldapDirectoryIdentifier?.PortNumber,
				ProtocolVersion = options.Connection.SessionOptions.ProtocolVersion,
				RootDistinguishedName = options.RootDistinguishedName,
				Timeout = options.Connection.Timeout
			};

			foreach(var attribute in options.Attributes)
			{
				connectionInformation.Attributes.Add(attribute);
			}

			foreach(var server in ldapDirectoryIdentifier?.Servers ?? Enumerable.Empty<string>())
			{
				connectionInformation.Servers.Add(server);
			}

			return connectionInformation;
		}

		public virtual void SetDirectoryInformation(ConnectionForm form)
		{
			if(form == null)
				throw new ArgumentNullException(nameof(form));

			var attributes = (form.Attributes ?? string.Empty).Split(',').Select(part => part.Trim()).Where(part => !string.IsNullOrWhiteSpace(part)).ToArray();
			ConnectionInformation connectionInformation = null;

			var start = this.SystemClock.UtcNow;

			var searchResultEntries = this.Directory.Find((findOptions) =>
			{
				foreach(var attribute in attributes)
				{
					findOptions.Attributes.Add(attribute);
				}

				if(form.ClearBaseFilter)
					findOptions.FilterBuilder.Filters.Clear();

				if(!string.IsNullOrWhiteSpace(form.Filter))
					findOptions.FilterBuilder.Filters.Add(form.Filter);

				findOptions.RootDistinguishedName = form.RootDistinguishedName;

				if(form.Timeout != null)
					findOptions.Connection.Timeout = TimeSpan.FromMinutes(form.Timeout.Value);

				connectionInformation = this.CreateConnectionInformation(findOptions);
			});

			connectionInformation.Duration = this.SystemClock.UtcNow - start;

			var entries = new Dictionary<string, IEntry>(StringComparer.OrdinalIgnoreCase);

			foreach(var entry in searchResultEntries)
			{
				entries.Add(entry.DistinguishedName, entry);
			}

			this.DirectoryInformation = new DirectoryInformation(connectionInformation, entries);
		}

		#endregion
	}
}