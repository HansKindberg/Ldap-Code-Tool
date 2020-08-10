using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Application.Models.Extensions;
using RegionOrebroLan.DirectoryServices.Protocols;

namespace Application.Models
{
	public class EntryNode : IEntryNode
	{
		#region Fields

		private IDictionary<string, IEntryNode> _children;
		private Lazy<IEntryNode> _parent;
		private static readonly ConcurrentDictionary<string, string> _parentDistinguishedNameCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		#endregion

		#region Constructors

		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		public EntryNode(IDictionary<string, IEntry> entries, IEntry entry) : this(false, entries, entry) { }

		public EntryNode(bool clearParentDistinguishedNameCache, IDictionary<string, IEntry> entries, IEntry entry)
		{
			this.Entries = entries ?? throw new ArgumentNullException(nameof(entries));
			this.Entry = entry ?? throw new ArgumentNullException(nameof(entry));

			if(clearParentDistinguishedNameCache)
				_parentDistinguishedNameCache.Clear();
		}

		#endregion

		#region Properties

		public virtual IDictionary<string, IEntryNode> Children
		{
			get
			{
				// ReSharper disable All
				if(this._children == null)
				{
					var children = new Dictionary<string, IEntryNode>(StringComparer.OrdinalIgnoreCase);

					foreach(var (key, value) in this.Entries.Where(item => item.Key.EndsWith("," + this.Entry.DistinguishedName, StringComparison.OrdinalIgnoreCase)))
					{
						var parentDistinguishedName = this.GetParentDistinguishedName(key);

						if(this.Entry.DistinguishedName.Equals(parentDistinguishedName, StringComparison.OrdinalIgnoreCase))
							children.Add(key, new EntryNode(this.Entries, value));
					}

					this._children = children;
				}
				// ReSharper restore All

				return this._children;
			}
		}

		protected internal virtual IDictionary<string, IEntry> Entries { get; }
		public virtual IEntry Entry { get; }

		public virtual IEntryNode Parent
		{
			get
			{
				this._parent ??= new Lazy<IEntryNode>(() =>
				{
					var parent = this.GetParentInternal(this.Entry);

					return parent != null ? new EntryNode(this.Entries, parent) : null;
				});

				return this._parent.Value;
			}
		}

		protected internal virtual ConcurrentDictionary<string, string> ParentDistinguishedNameCache => _parentDistinguishedNameCache;

		#endregion

		#region Methods

		protected internal virtual string GetParentDistinguishedName(string distinguishedName)
		{
			return this.ParentDistinguishedNameCache.GetOrAdd(distinguishedName ?? string.Empty, (key) =>
			{
				if(string.IsNullOrWhiteSpace(key))
					return null;

				var index = key.IndexOfFirstUnescapedCharacter(',');

				return index != null ? key.Substring(index.Value + 1) : null;
			});
		}

		protected internal virtual IEntry GetParentInternal(IEntry entry)
		{
			if(entry == null)
				throw new ArgumentNullException(nameof(entry));

			var parentDistinguishedName = this.GetParentDistinguishedName(entry.DistinguishedName);

			return parentDistinguishedName != null && this.Entries.TryGetValue(parentDistinguishedName, out var parent) ? parent : null;
		}

		#endregion
	}
}