using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Application.Models.Configuration
{
	public class CodeOptions
	{
		#region Properties

		public virtual string BaseTypeName { get; set; } = "Entry";

		/// <summary>
		/// Patterns for default-selected attributes.
		/// </summary>
		public virtual ISet<string> DefaultAttributes { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Patterns for default-selected object-classes.
		/// </summary>
		public virtual ISet<string> DefaultObjectClasses { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public virtual CodeGeneratorOptions Generation { get; set; } = new CodeGeneratorOptions {BracingStyle = "C", IndentString = "\t"};
		public virtual PropertyAttributesOptions PropertyAttributes { get; set; } = new PropertyAttributesOptions();

		public virtual IDictionary<string, string> Replacements { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{"()]", "]"},
			{$"{Environment.NewLine}\t{{{Environment.NewLine}\t\tget{Environment.NewLine}\t\t{{{Environment.NewLine}\t\t}}{Environment.NewLine}\t}}", " { get; }"},
			{$"{Environment.NewLine}\t{{{Environment.NewLine}\t\tget{Environment.NewLine}\t\t{{{Environment.NewLine}\t\t}}{Environment.NewLine}\t\tset{Environment.NewLine}\t\t{{{Environment.NewLine}\t\t}}{Environment.NewLine}\t}}", " { get; set; }"}
		};

		public virtual IDictionary<string, string> Summaries { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		#endregion
	}
}