using System;
using System.Collections.Generic;
using System.Linq;
using RegionOrebroLan.DirectoryServices.Protocols;

namespace Application.Models
{
	public class DirectoryInformation : IDirectoryInformation
	{
		#region Fields

		private IDictionary<string, IAttributeInformation> _attributes;
		private IDictionary<string, IEnumerable<IAttributeInformation>> _objectClasses;
		private Lazy<IEntryNode> _tree;

		#endregion

		#region Constructors

		public DirectoryInformation(IConnectionInformation connection, IDictionary<string, IEntry> entries)
		{
			this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
			this.Entries = entries ?? throw new ArgumentNullException(nameof(entries));
		}

		#endregion

		#region Properties

		public virtual IDictionary<string, IAttributeInformation> Attributes
		{
			get
			{
				if(this._attributes == null)
				{
					var attributes = new SortedDictionary<string, AttributeInformation>(StringComparer.OrdinalIgnoreCase);
					const string objectClassAttributeName = "objectClass";

					foreach(var entry in this.Entries)
					{
						var objectClasses = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

						if(entry.Value.Attributes.ContainsKey(objectClassAttributeName))
						{
							foreach(var objectClass in entry.Value.Attributes[objectClassAttributeName].GetValues(typeof(string)).Cast<string>())
							{
								objectClasses.Add(objectClass);
							}
						}

						foreach(var attributeEntry in entry.Value.Attributes)
						{
							if(!attributes.TryGetValue(attributeEntry.Value.Name, out var attributeInformation))
							{
								attributeInformation = new AttributeInformation
								{
									Name = attributeEntry.Value.Name
								};

								attributes.Add(attributeEntry.Value.Name, attributeInformation);
							}

							foreach(var objectClass in objectClasses)
							{
								attributeInformation.ObjectClassesThatThisAttributeExistsAt.Add(objectClass);
							}

							var values = attributeEntry.Value.GetValues(typeof(string)).Cast<string>().ToArray();

							if(attributeInformation.HighestNumberOfValuesDistinguishedName == null || values.Length > attributeInformation.HighestNumberOfValues)
							{
								attributeInformation.HighestNumberOfValues = values.Length;
								attributeInformation.HighestNumberOfValuesDistinguishedName = entry.Value.DistinguishedName;
							}

							var totalValueLength = string.Join(string.Empty, values).Length;

							if(attributeInformation.LongestTotalValueLengthDistinguishedName == null || totalValueLength > attributeInformation.LongestTotalValueLength)
							{
								attributeInformation.LongestTotalValueLength = totalValueLength;
								attributeInformation.LongestTotalValueLengthDistinguishedName = entry.Value.DistinguishedName;
							}
						}
					}

					this._attributes = new SortedDictionary<string, IAttributeInformation>(attributes.ToDictionary(item => item.Key, item => (IAttributeInformation)item.Value), StringComparer.OrdinalIgnoreCase);
				}

				return this._attributes;
			}
		}

		public virtual IConnectionInformation Connection { get; }
		public virtual IDictionary<string, IEntry> Entries { get; }

		public virtual IDictionary<string, IEnumerable<IAttributeInformation>> ObjectClasses
		{
			get
			{
				// ReSharper disable InvertIf
				if(this._objectClasses == null)
				{
					var objectClasses = new SortedDictionary<string, IList<IAttributeInformation>>(StringComparer.OrdinalIgnoreCase);

					foreach(var item in this.Attributes)
					{
						foreach(var objectClass in item.Value.ObjectClassesThatThisAttributeExistsAt)
						{
							if(!objectClasses.TryGetValue(objectClass, out var attributes))
							{
								attributes = new List<IAttributeInformation>();
								objectClasses.Add(objectClass, attributes);
							}

							attributes.Add(item.Value);
						}
					}

					this._objectClasses = new SortedDictionary<string, IEnumerable<IAttributeInformation>>(objectClasses.ToDictionary(item => item.Key, item => (IEnumerable<IAttributeInformation>)item.Value));
				}
				// ReSharper restore InvertIf

				return this._objectClasses;
			}
		}

		public virtual IEntryNode Tree
		{
			get
			{
				this._tree ??= new Lazy<IEntryNode>(() => this.Entries.TryGetValue(this.Connection.RootDistinguishedName, out var entry) ? new EntryNode(true, this.Entries, entry) : null);

				return this._tree.Value;
			}
		}

		#endregion
	}
}