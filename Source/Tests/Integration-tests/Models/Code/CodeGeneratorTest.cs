using System;
using System.Collections.Generic;
using System.Linq;
using Application.Models;
using Application.Models.Code;
using Application.Models.Configuration;
using Application.Models.Forms;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.DirectoryServices.Protocols.Configuration;
using RegionOrebroLan.DirectoryServices.Protocols.DependencyInjection;

namespace IntegrationTests.Models.Code
{
	[TestClass]
	public class CodeGeneratorTest
	{
		#region Fields

		private static IServiceProvider _serviceProvider;

		#endregion

		#region Properties

		protected internal virtual CodeGenerator CodeGenerator => (CodeGenerator)this.ServiceProvider.GetRequiredService<IFacade>().CodeGenerator;
		protected internal virtual CodeOptions CodeOptions => this.ServiceProvider.GetRequiredService<IOptions<CodeOptions>>().Value;
		protected internal virtual IDirectoryInformation DirectoryInformation => this.ServiceProvider.GetRequiredService<IFacade>().DirectoryInformation;
		protected internal virtual IServiceProvider ServiceProvider => _serviceProvider;

		#endregion

		#region Methods

		protected internal virtual ICustomCodeOptions CreateCustomOptions(CodeType codeType, TypeStructure typeStructure, IEnumerable<string> attributes = null, IEnumerable<string> objectClasses = null)
		{
			var form = new CodeOptionsForm();

			form.Initialize(this.CodeOptions, this.DirectoryInformation);

			if(attributes != null)
				form.Attributes = attributes;

			form.CodeType = codeType;

			if(objectClasses != null)
				form.ObjectClasses = objectClasses;

			form.TypeStructure = typeStructure;

			return form;
		}

		[TestMethod]
		public void GenerateAsync_IfMultipleObjectClasses_ShouldWorkProperly()
		{
			/*
				applicationProcess
				inetOrgPerson
				ivbbCA
				ivbbPerson
				ivbbRealEstate
				locality
				organization
				organizationalPerson
				organizationalUnit
				person
				pkiCA
				subtree
				top
			*/

			var expected = "public class Organization\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\t[MaxLength(10)]\r\n\tpublic virtual string O { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n}\r\npublic class OrganizationalPerson\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\t[MaxLength(Metadata.MyLength)]\r\n\tpublic virtual string Cn { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string GivenName { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Mail { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Sn { get; set; }\r\n}\r\npublic class OrganizationalUnit\r\n{\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string L { get; set; }\r\n\t\r\n\t[MaxLength(500)]\r\n\tpublic virtual string LabeledURI { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\t[MaxLength(200)]\r\n\tpublic virtual string Ou { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostalCode { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostOfficeBox { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string Street { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string TelephoneNumber { get; set; }\r\n}\r\n";
			var code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.Class, TypeStructure.Multiple), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public class Entry\r\n{\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n}\r\npublic class Organization : Entry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\t[MaxLength(10)]\r\n\tpublic virtual string O { get; set; }\r\n}\r\npublic class OrganizationalPerson : Entry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\t[MaxLength(Metadata.MyLength)]\r\n\tpublic virtual string Cn { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string GivenName { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Mail { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Sn { get; set; }\r\n}\r\npublic class OrganizationalUnit : Entry\r\n{\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string L { get; set; }\r\n\t\r\n\t[MaxLength(500)]\r\n\tpublic virtual string LabeledURI { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\t[MaxLength(200)]\r\n\tpublic virtual string Ou { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostalCode { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostOfficeBox { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string Street { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string TelephoneNumber { get; set; }\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.Class, TypeStructure.MultipleWithBaseType), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public class Entry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\t[MaxLength(Metadata.MyLength)]\r\n\tpublic virtual string Cn { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string GivenName { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string L { get; set; }\r\n\t\r\n\t[MaxLength(500)]\r\n\tpublic virtual string LabeledURI { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Mail { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\t[MaxLength(10)]\r\n\tpublic virtual string O { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\t[MaxLength(200)]\r\n\tpublic virtual string Ou { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostalCode { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostOfficeBox { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Sn { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string Street { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string TelephoneNumber { get; set; }\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.Class, TypeStructure.Single), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public interface IOrganization\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\tstring O\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\npublic interface IOrganizationalPerson\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\tstring Cn\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring GivenName\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Mail\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\tstring Sn\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\npublic interface IOrganizationalUnit\r\n{\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\tstring L\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring LabeledURI\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\tstring Ou\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring PostalCode\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring PostOfficeBox\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Street\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring TelephoneNumber\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\npublic class Organization : IOrganization\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\t[MaxLength(10)]\r\n\tpublic virtual string O { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n}\r\npublic class OrganizationalPerson : IOrganizationalPerson\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\t[MaxLength(Metadata.MyLength)]\r\n\tpublic virtual string Cn { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string GivenName { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Mail { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Sn { get; set; }\r\n}\r\npublic class OrganizationalUnit : IOrganizationalUnit\r\n{\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string L { get; set; }\r\n\t\r\n\t[MaxLength(500)]\r\n\tpublic virtual string LabeledURI { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\t[MaxLength(200)]\r\n\tpublic virtual string Ou { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostalCode { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostOfficeBox { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string Street { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string TelephoneNumber { get; set; }\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.ClassAndInterfaceGet, TypeStructure.Multiple), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public interface IEntry\r\n{\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\npublic interface IOrganization : IEntry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\tstring O\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\npublic interface IOrganizationalPerson : IEntry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\tstring Cn\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring GivenName\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Mail\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\tstring Sn\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\npublic interface IOrganizationalUnit : IEntry\r\n{\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\tstring L\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring LabeledURI\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\tstring Ou\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring PostalCode\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring PostOfficeBox\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Street\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring TelephoneNumber\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\npublic class Entry : IEntry\r\n{\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n}\r\npublic class Organization : Entry, IOrganization\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\t[MaxLength(10)]\r\n\tpublic virtual string O { get; set; }\r\n}\r\npublic class OrganizationalPerson : Entry, IOrganizationalPerson\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\t[MaxLength(Metadata.MyLength)]\r\n\tpublic virtual string Cn { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string GivenName { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Mail { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Sn { get; set; }\r\n}\r\npublic class OrganizationalUnit : Entry, IOrganizationalUnit\r\n{\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string L { get; set; }\r\n\t\r\n\t[MaxLength(500)]\r\n\tpublic virtual string LabeledURI { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\t[MaxLength(200)]\r\n\tpublic virtual string Ou { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostalCode { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostOfficeBox { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string Street { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string TelephoneNumber { get; set; }\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.ClassAndInterfaceGet, TypeStructure.MultipleWithBaseType), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public interface IEntry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\tstring Cn\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring GivenName\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\tstring L\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring LabeledURI\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Mail\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\tstring O\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\tstring Ou\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring PostalCode\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring PostOfficeBox\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\tstring Sn\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Street\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring TelephoneNumber\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\npublic class Entry : IEntry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\t[MaxLength(Metadata.MyLength)]\r\n\tpublic virtual string Cn { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string GivenName { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string L { get; set; }\r\n\t\r\n\t[MaxLength(500)]\r\n\tpublic virtual string LabeledURI { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Mail { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\t[MaxLength(10)]\r\n\tpublic virtual string O { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\t[MaxLength(200)]\r\n\tpublic virtual string Ou { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostalCode { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string PostOfficeBox { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Sn { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string Street { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string TelephoneNumber { get; set; }\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.ClassAndInterfaceGet, TypeStructure.Single), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public interface IOrganization\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\tstring O\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\npublic interface IOrganizationalPerson\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\tstring Cn\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring GivenName\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Mail\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\tstring Sn\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\npublic interface IOrganizationalUnit\r\n{\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\tstring L\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring LabeledURI\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\tstring Ou\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring PostalCode\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring PostOfficeBox\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Street\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring TelephoneNumber\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGetSet, TypeStructure.Multiple), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public interface IEntry\r\n{\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\npublic interface IOrganization : IEntry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\tstring O\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\npublic interface IOrganizationalPerson : IEntry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\tstring Cn\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring GivenName\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Mail\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\tstring Sn\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\npublic interface IOrganizationalUnit : IEntry\r\n{\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\tstring L\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring LabeledURI\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\tstring Ou\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring PostalCode\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring PostOfficeBox\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Street\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring TelephoneNumber\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGetSet, TypeStructure.MultipleWithBaseType), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public interface IEntry\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\tstring Cn\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring GivenName\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Locality name\r\n\t/// </summary>\r\n\tstring L\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring LabeledURI\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Mail\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Organization\r\n\t/// </summary>\r\n\tstring O\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Organizational unit\r\n\t/// </summary>\r\n\tstring Ou\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring PostalCode\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring PostOfficeBox\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\tstring Sn\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Street\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring TelephoneNumber\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGetSet, TypeStructure.Single), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);
		}

		[TestMethod]
		public void GenerateAsync_IfNoObjectClasses_ShouldReturnAnEmptyString()
		{
			var code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.Class, TypeStructure.Multiple, null, Enumerable.Empty<string>()), this.DirectoryInformation).Result;
			Assert.AreEqual(string.Empty, code);

			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.ClassAndInterfaceGet, TypeStructure.MultipleWithBaseType, null, Enumerable.Empty<string>()), this.DirectoryInformation).Result;
			Assert.AreEqual(string.Empty, code);

			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGetSet, TypeStructure.Single, null, Enumerable.Empty<string>()), this.DirectoryInformation).Result;
			Assert.AreEqual(string.Empty, code);
		}

		[TestMethod]
		public void GenerateAsync_IfSingleObjectClass_ShouldWorkProperly()
		{
			var objectClasses = new[] {"organizationalPerson"};

			var expected = "public class OrganizationalPerson\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\t[MaxLength(Metadata.MyLength)]\r\n\tpublic virtual string Cn { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Description { get; set; }\r\n\t\r\n\t[MaxLength(20)]\r\n\tpublic virtual string GivenName { get; set; }\r\n\t\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Mail { get; set; }\r\n\t\r\n\t[MaxLength(100)]\r\n\tpublic virtual string ObjectClass { get; set; }\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\t[MaxLength(50)]\r\n\tpublic virtual string Sn { get; set; }\r\n}\r\n";
			var code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.Class, TypeStructure.Multiple, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.Class, TypeStructure.MultipleWithBaseType, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.Class, TypeStructure.Single, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public interface IOrganizationalPerson\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\tstring Cn\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring GivenName\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring Mail\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\tstring Sn\r\n\t{\r\n\t\tget;\r\n\t}\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGet, TypeStructure.Multiple, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGet, TypeStructure.MultipleWithBaseType, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGet, TypeStructure.Single, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);

			expected = "public interface IOrganizationalPerson\r\n{\r\n\t\r\n\t/// <summary>\r\n\t/// Common name\r\n\t/// </summary>\r\n\tstring Cn\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Description\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring GivenName\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring Mail\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\tstring ObjectClass\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n\t\r\n\t/// <summary>\r\n\t/// Surname\r\n\t/// </summary>\r\n\tstring Sn\r\n\t{\r\n\t\tget;\r\n\t\tset;\r\n\t}\r\n}\r\n";
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGetSet, TypeStructure.Multiple, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGetSet, TypeStructure.MultipleWithBaseType, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);
			code = this.CodeGenerator.GenerateAsync(this.CreateCustomOptions(CodeType.InterfaceGetSet, TypeStructure.Single, null, objectClasses), this.DirectoryInformation).Result;
			Assert.AreEqual(expected, code);
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			if(testContext == null)
				throw new ArgumentNullException(nameof(testContext));

			var services = new ServiceCollection();

			services.Configure<CodeOptions>(Global.Configuration.GetSection("Code"));

			services.AddDirectory(Global.Configuration);
			services.AddScoped<IFacade, Facade>();
			services.AddSingleton<ICodeGenerator, CodeGenerator>();
			services.AddSingleton<IMemoryCache, MemoryCache>();
			services.AddSingleton<ISystemClock, SystemClock>();

			_serviceProvider = services.BuildServiceProvider();

			var directoryOptions = _serviceProvider.GetRequiredService<IOptions<DirectoryOptions>>().Value;
			var facade = _serviceProvider.GetRequiredService<IFacade>();

			try
			{
				facade.SetDirectoryInformation(new ConnectionForm
				{
					RootDistinguishedName = directoryOptions.RootDistinguishedName
				});
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Could not set directory-information. First make sure you are not behind a firewall blocking outgoing traffic to LDAP-ports.", exception);
			}
		}

		#endregion
	}
}