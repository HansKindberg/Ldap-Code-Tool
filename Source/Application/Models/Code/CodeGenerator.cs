using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Models.Configuration;
using Application.Models.Extensions;
using Microsoft.CSharp;
using Microsoft.Extensions.Options;

namespace Application.Models.Code
{
	public class CodeGenerator : ICodeGenerator
	{
		#region Constructors

		public CodeGenerator(IOptions<CodeOptions> options)
		{
			this.Options = options ?? throw new ArgumentNullException(nameof(options));
		}

		#endregion

		#region Properties

		protected internal virtual IOptions<CodeOptions> Options { get; }

		#endregion

		#region Methods

		protected internal virtual CodeAttributeDeclaration CreateCodeAttributeDeclaration(string argument, string name)
		{
			if(argument == null)
				throw new ArgumentNullException(nameof(argument));

			if(name == null)
				throw new ArgumentNullException(nameof(name));

			return new CodeAttributeDeclaration(name, new CodeAttributeArgument(new CodeVariableReferenceExpression(argument)));
		}

		protected internal virtual CodeDomProvider CreateCodeDomProvider()
		{
			return new CSharpCodeProvider();
		}

		protected internal virtual CodeTypeDeclaration CreateCodeTypeDeclaration(IEnumerable<IAttributeInformation> attributes, IEnumerable<string> baseTypes, ICustomCodeOptions customOptions, TypeKind kind, string name)
		{
			if(customOptions == null)
				throw new ArgumentNullException(nameof(customOptions));

			var codeTypeDeclaration = new CodeTypeDeclaration(name);

			foreach(var baseType in baseTypes ?? Enumerable.Empty<string>())
			{
				codeTypeDeclaration.BaseTypes.Add(baseType);
			}

			var hasSet = true;

			if(kind == TypeKind.Interface)
			{
				codeTypeDeclaration.IsInterface = true;
				hasSet = customOptions.CodeType == CodeType.ClassAndInterfaceGetSet || customOptions.CodeType == CodeType.InterfaceGetSet;
			}
			else
			{
				codeTypeDeclaration.IsClass = true;
			}

			var options = this.Options.Value;

			foreach(var attribute in attributes ?? Enumerable.Empty<IAttributeInformation>())
			{
				var codeMemberProperty = new CodeMemberProperty
				{
					HasGet = true,
					HasSet = hasSet,
					Name = this.EnsureAlphanumericAndUnderscoreCharactersOnly(attribute.Name.FirstLetterToUpperInvariant()),
					Type = customOptions.MultiValueProperties ? new CodeTypeReference("IEnumerable<string>") : new CodeTypeReference(typeof(string))
				};

				if(options.Summaries.TryGetValue(attribute.Name, out var summary))
				{
					// https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-create-an-xml-documentation-file-using-codedom?redirectedfrom=MSDN
					codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
					codeMemberProperty.Comments.Add(new CodeCommentStatement(summary, true));
					codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
				}

				if(kind == TypeKind.Class)
				{
					codeMemberProperty.Attributes = MemberAttributes.Public; // This will make it public and virtual. If we don't want it virtual we should set it to MemberAttributes.Public | MemberAttributes.Final. See https://docs.microsoft.com/en-us/dotnet/api/system.codedom.memberattributes?view=dotnet-plat-ext-3.1
					this.PopulatePropertyAttributes(attribute, codeMemberProperty);
				}

				codeTypeDeclaration.Members.Add(codeMemberProperty);
			}

			return codeTypeDeclaration;
		}

		protected internal virtual string EnsureAlphanumericAndUnderscoreCharactersOnly(string value)
		{
			if(value == null)
				throw new ArgumentNullException(nameof(value));

			var regex = new Regex("[^a-zA-Z0-9_]");

			return regex.Replace(value, string.Empty);
		}

		public virtual async Task<string> GenerateAsync(ICustomCodeOptions customOptions, IDirectoryInformation directoryInformation)
		{
			if(customOptions == null)
				throw new ArgumentNullException(nameof(customOptions));

			if(directoryInformation == null)
				throw new ArgumentNullException(nameof(directoryInformation));

			var codeTypeDeclarations = this.GetCodeTypeDeclarations(customOptions, directoryInformation);

			// ReSharper disable ConvertToUsingDeclaration
			await using(var stringWriter = new StringWriter())
			{
				using(var codeDomProvider = this.CreateCodeDomProvider())
				{
					var options = this.Options.Value;

					foreach(var codeTypeDeclaration in codeTypeDeclarations)
					{
						codeDomProvider.GenerateCodeFromType(codeTypeDeclaration, stringWriter, options.Generation);
					}

					var code = stringWriter.ToString();

					// ReSharper disable LoopCanBeConvertedToQuery
					foreach(var (key, value) in this.Options.Value.Replacements)
					{
						code = code.Replace(key, value, StringComparison.Ordinal);
					}
					// ReSharper restore LoopCanBeConvertedToQuery

					return await Task.FromResult(code).ConfigureAwait(false);
				}
			}
			// ReSharper restore ConvertToUsingDeclaration
		}

		protected internal virtual IEnumerable<IAttributeInformation> GetAllAttributes(ICustomCodeOptions customOptions, IDirectoryInformation directoryInformation, IEnumerable<string> objectClasses)
		{
			return this.GetInterestingAttributes((directoryInformation?.Attributes ?? new Dictionary<string, IAttributeInformation>()).Values.Where(attribute => (objectClasses ?? Enumerable.Empty<string>()).Intersect(attribute.ObjectClassesThatThisAttributeExistsAt).Any()), customOptions);
		}

		[SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity")]
		protected internal virtual IEnumerable<CodeTypeDeclaration> GetCodeTypeDeclarations(ICustomCodeOptions customOptions, IDirectoryInformation directoryInformation)
		{
			if(customOptions == null)
				throw new ArgumentNullException(nameof(customOptions));

			if(directoryInformation == null)
				throw new ArgumentNullException(nameof(directoryInformation));

			var codeTypeDeclarations = new List<CodeTypeDeclaration>();
			var objectClasses = (customOptions.ObjectClasses ?? Enumerable.Empty<string>()).ToArray();

			// ReSharper disable InvertIf
			if(objectClasses.Any())
			{
				var baseTypes = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
				var classAttributes = new Dictionary<string, IEnumerable<IAttributeInformation>>(StringComparer.OrdinalIgnoreCase);
				var codeType = customOptions.CodeType;
				var includeClasses = codeType == CodeType.Class || codeType == CodeType.ClassAndInterfaceGet || codeType == CodeType.ClassAndInterfaceGetSet;
				var includeInterfaces = codeType == CodeType.ClassAndInterfaceGet || codeType == CodeType.ClassAndInterfaceGetSet || codeType == CodeType.InterfaceGet || codeType == CodeType.InterfaceGetSet;
				var interfaceAttributes = new Dictionary<string, IEnumerable<IAttributeInformation>>(StringComparer.OrdinalIgnoreCase);

				if(customOptions.TypeStructure == TypeStructure.MultipleWithBaseType && objectClasses.Length > 1)
				{
					var commonAttributes = this.GetCommonAttributes(customOptions, directoryInformation, objectClasses).ToArray();

					if(commonAttributes.Any())
					{
						var baseClassName = this.EnsureAlphanumericAndUnderscoreCharactersOnly(customOptions.BaseTypeName.FirstLetterToUpperInvariant());

						if(includeClasses)
							classAttributes.Add(baseClassName, commonAttributes);

						if(includeInterfaces)
						{
							var baseInterfaceName = this.GetInterfaceName(baseClassName);
							interfaceAttributes.Add(baseInterfaceName, commonAttributes);
							baseTypes.Add(baseClassName, new[] {baseInterfaceName});
						}
					}
				}

				if(customOptions.TypeStructure == TypeStructure.Single)
				{
					var attributes = this.GetAllAttributes(customOptions, directoryInformation, objectClasses).ToArray();

					if(attributes.Any())
					{
						var className = this.EnsureAlphanumericAndUnderscoreCharactersOnly((objectClasses.Length > 1 ? customOptions.BaseTypeName : objectClasses.First()).FirstLetterToUpperInvariant());

						if(includeClasses)
							classAttributes.Add(className, attributes);

						if(includeInterfaces)
						{
							var interfaceName = this.GetInterfaceName(className);
							interfaceAttributes.Add(interfaceName, attributes);
							baseTypes.Add(className, new[] {interfaceName});
						}
					}
				}
				else
				{
					var baseClassName = classAttributes.FirstOrDefault().Key;
					var baseInterfaceName = interfaceAttributes.FirstOrDefault().Key;
					var commonAttributes = classAttributes.FirstOrDefault().Value ?? interfaceAttributes.FirstOrDefault().Value ?? Enumerable.Empty<IAttributeInformation>();

					foreach(var objectClass in objectClasses)
					{
						if(directoryInformation.ObjectClasses.TryGetValue(objectClass, out var attributes))
						{
							attributes = this.GetInterestingAttributes(attributes, customOptions).Where(attribute => !commonAttributes.Any(commonAttribute => commonAttribute.Name.Equals(attribute.Name, StringComparison.OrdinalIgnoreCase))).ToArray();

							var className = this.EnsureAlphanumericAndUnderscoreCharactersOnly(objectClass.FirstLetterToUpperInvariant());
							var classBaseTypes = new List<string>();

							if(includeClasses)
							{
								classAttributes.Add(className, attributes);

								if(baseClassName != null)
									classBaseTypes.Add(baseClassName);
							}

							if(includeInterfaces)
							{
								var interfaceName = this.GetInterfaceName(className);
								interfaceAttributes.Add(interfaceName, attributes);

								if(baseInterfaceName != null)
									baseTypes.Add(interfaceName, new[] {baseInterfaceName});

								classBaseTypes.Add(interfaceName);
							}

							if(classBaseTypes.Any())
								baseTypes.Add(className, classBaseTypes);
						}
					}
				}

				foreach(var (interfaceName, attributes) in interfaceAttributes)
				{
					if(!baseTypes.TryGetValue(interfaceName, out var interfaceBaseTypes))
						interfaceBaseTypes = Enumerable.Empty<string>();

					codeTypeDeclarations.Add(this.CreateCodeTypeDeclaration(attributes, interfaceBaseTypes, customOptions, TypeKind.Interface, interfaceName));
				}

				foreach(var (className, attributes) in classAttributes)
				{
					if(!baseTypes.TryGetValue(className, out var classBaseTypes))
						classBaseTypes = Enumerable.Empty<string>();

					codeTypeDeclarations.Add(this.CreateCodeTypeDeclaration(attributes, classBaseTypes, customOptions, TypeKind.Class, className));
				}
			}
			// ReSharper restore InvertIf

			return codeTypeDeclarations.ToArray();
		}

		protected internal virtual IEnumerable<IAttributeInformation> GetCommonAttributes(ICustomCodeOptions customOptions, IDirectoryInformation directoryInformation, IEnumerable<string> objectClasses)
		{
			return this.GetInterestingAttributes((directoryInformation?.Attributes ?? new Dictionary<string, IAttributeInformation>()).Values.Where(attribute => !(objectClasses ?? Enumerable.Empty<string>()).Except(attribute.ObjectClassesThatThisAttributeExistsAt).Any()), customOptions);
		}

		protected internal virtual IEnumerable<IAttributeInformation> GetInterestingAttributes(IEnumerable<IAttributeInformation> attributes, ICustomCodeOptions customOptions)
		{
			if(attributes == null)
				throw new ArgumentNullException(nameof(attributes));

			if(customOptions == null)
				throw new ArgumentNullException(nameof(customOptions));

			return attributes.Where(attribute => !(customOptions.UninterestingAttributes ?? Enumerable.Empty<string>()).Contains(attribute.Name, StringComparer.OrdinalIgnoreCase));
		}

		protected internal virtual string GetInterfaceName(string name)
		{
			return $"I{name}";
		}

		protected internal virtual void PopulatePropertyAttributes(IAttributeInformation attribute, CodeMemberProperty codeMemberProperty)
		{
			if(attribute == null)
				throw new ArgumentNullException(nameof(attribute));

			if(codeMemberProperty == null)
				throw new ArgumentNullException(nameof(codeMemberProperty));

			var propertyAttributes = new List<CodeAttributeDeclaration>();
			var maxLengthAttributeAdded = false;
			const string maxLengthAttributeName = "MaxLength";
			var options = this.Options.Value.PropertyAttributes;

			foreach(var item in options.Items)
			{
				if(!item.Patterns.Any(pattern => attribute.Name.Like(pattern)))
					continue;

				propertyAttributes.Add(this.CreateCodeAttributeDeclaration(item.Argument, item.Name));

				if(item.Name.Equals(maxLengthAttributeName, StringComparison.OrdinalIgnoreCase))
					maxLengthAttributeAdded = true;
			}

			if(options.MaxLength.Enabled && !maxLengthAttributeAdded)
			{
				int? maxLengthArgument = null;

				foreach(var breakpoint in options.MaxLength.Breakpoints.OrderByDescending(breakpoint => breakpoint.Key))
				{
					// ReSharper disable InvertIf
					if(attribute.LongestTotalValueLength > breakpoint.Key)
					{
						maxLengthArgument = breakpoint.Value;
						break;
					}
					// ReSharper restore InvertIf
				}

				if(maxLengthArgument != null)
					propertyAttributes.Add(this.CreateCodeAttributeDeclaration(maxLengthArgument.Value.ToString(CultureInfo.InvariantCulture), maxLengthAttributeName));
			}

			foreach(var propertyAttribute in propertyAttributes.OrderBy(propertyAttribute => propertyAttribute.Name))
			{
				codeMemberProperty.CustomAttributes.Add(propertyAttribute);
			}
		}

		#endregion
	}
}