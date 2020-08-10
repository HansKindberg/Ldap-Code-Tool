using System;
using System.Text.RegularExpressions;

namespace Application.Models.Extensions
{
	public static class StringExtension
	{
		#region Methods

		public static string FirstLetterToUpperInvariant(this string value)
		{
			if(value == null)
				throw new ArgumentNullException(nameof(value));

			if(value.Length > 0)
				return value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);

			return value;
		}

		public static int? IndexOfFirstUnescapedCharacter(this string value, char character)
		{
			if(value == null)
				throw new ArgumentNullException(nameof(value));

			const StringComparison comparison = StringComparison.Ordinal;
			var index = 0;

			while(value.Contains(character, comparison))
			{
				var i = value.IndexOf(character, comparison);
				index += i;

				if(i == 0 || value[i - 1] != '\\')
					return index;

				index++;
				value = value.Substring(i + 1);
			}

			return null;
		}

		public static bool Like(this string value, string pattern)
		{
			if(value == null)
				throw new ArgumentNullException(nameof(value));

			if(pattern == null)
				throw new ArgumentNullException(nameof(pattern));

			const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

			var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*", StringComparison.Ordinal) + "$";

			return Regex.IsMatch(value, regexPattern, regexOptions);
		}

		#endregion
	}
}