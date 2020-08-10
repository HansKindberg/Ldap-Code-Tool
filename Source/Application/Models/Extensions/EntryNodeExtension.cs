using System;
using System.Collections.Generic;

namespace Application.Models.Extensions
{
	public static class EntryNodeExtension
	{
		#region Methods

		public static IEnumerable<IEntryNode> Ancestors(this IEntryNode entryNode)
		{
			if(entryNode == null)
				throw new ArgumentNullException(nameof(entryNode));

			var ancestors = new List<IEntryNode>();

			var parent = entryNode.Parent;

			while(parent != null)
			{
				ancestors.Add(parent);

				parent = parent.Parent;
			}

			return ancestors.ToArray();
		}

		public static string Name(this IEntryNode entryNode)
		{
			if(entryNode == null)
				throw new ArgumentNullException(nameof(entryNode));

			// ReSharper disable ConvertIfStatementToReturnStatement
			if(entryNode.Parent == null)
				return entryNode.Entry.DistinguishedName;
			// ReSharper restore ConvertIfStatementToReturnStatement

			return entryNode.Entry.DistinguishedName.Substring(0, entryNode.Entry.DistinguishedName.IndexOfFirstUnescapedCharacter(',') ?? 0);
		}

		#endregion
	}
}