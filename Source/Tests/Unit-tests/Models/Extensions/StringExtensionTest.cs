using System;
using System.Collections.Generic;
using Application.Models.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Models.Extensions
{
	[TestClass]
	public class StringExtensionTest
	{
		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IndexOfFirstUnescapedCharacter_IfTheValueIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				((string)null).IndexOfFirstUnescapedCharacter(',');
			}
			catch(ArgumentNullException argumentNullException)
			{
				// ReSharper disable PossibleNullReferenceException
				if(argumentNullException.ParamName.Equals("value", StringComparison.Ordinal))
					throw;
				// ReSharper restore PossibleNullReferenceException
			}
		}

		[TestMethod]
		public void IndexOfFirstUnescapedCharacter_Test()
		{
			var dictionary = new Dictionary<string, int?>
			{
				{string.Empty, null},
				{" ", null},
				{",", 0},
				{"\\,", null},
				{"abcd\\,efgh,ijkl", 10},
				{"abcd\\,efgh\\,ijkl,mnop", 16},
				{"abcd\\,efgh\\,ijkl\\,mnop\\,qrst,", 28},
				{"_\\,_\\,_\\,_\\,_\\,_\\,_\\,_\\,_\\,_\\,_\\,_\\,_\\,_,", 40},
				{"\\,\\,\\,\\,\\,\\,\\,\\,\\,\\,\\,\\,\\,,\\,\\,\\,\\,\\,", 26}
			};

			const char character = ',';
			foreach(var (key, value) in dictionary)
			{
				Assert.AreEqual(value, key.IndexOfFirstUnescapedCharacter(character));
			}
		}

		#endregion
	}
}