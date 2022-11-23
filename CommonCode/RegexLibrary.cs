namespace RobinHood70.CommonCode
{
	using System.ComponentModel;
	using System.Text.RegularExpressions;

	/// <summary>A class that provides several common Regex patterns.</summary>
	public static class RegexLibrary
	{
		/// <summary>Gets a <see cref="Regex"/> that finds any newline character or combination of chacters except a carriage-return/linefeed on its own.</summary>
		public static Regex EolNotCrlf { get; } = new Regex(@"((?<!\r)\n\r?|(?<!\n)\r(?!\n))", RegexOptions.ExplicitCapture, Globals.DefaultRegexTimeout);

		/// <summary>Gets a <see cref="Regex"/> that finds any newline character or combination of chacters except a linefeed on its own.</summary>
		public static Regex EolNotLineFeed { get; } = new Regex(@"((?<!\n)\r\n?|(?<!\r)\n\r?)", RegexOptions.ExplicitCapture, Globals.DefaultRegexTimeout);

		/// <summary>Gets a <see cref="Regex"/> that finds multiple spaces (and only spaces) used in sequence.</summary>
		public static Regex MultipleSpaces { get; } = new Regex(" {2,}", RegexOptions.None, Globals.DefaultRegexTimeout);

		/// <summary>Gets a <see cref="Regex"/> that finds multiple whitespace characters used in sequence.</summary>
		public static Regex MultipleWhitespaces { get; } = new Regex(@"\s{2,}", RegexOptions.None, Globals.DefaultRegexTimeout);

		/// <summary>Gets a <see cref="Regex"/> that finds multiple spaces used in sequence and any occurrence of a newline character.</summary>
		public static Regex MultipleWhitespacesOrLine { get; } = new Regex(@"(\s{2,}|[\r\n]+)", RegexOptions.ExplicitCapture, Globals.DefaultRegexTimeout);

		#region Public Methods

		/// <summary>Converts all variants of newlines to a carriage-return/linefeed combination (\r\n).</summary>
		/// <param name="text">The text to convert.</param>
		/// <returns>A copy of the original text with all newline characters or combinations converted to carriage-return/linefeed characters.</returns>
		public static string NewLinesToCrlf(string text) => EolNotCrlf.Replace(text, "\r\n");

		/// <summary>Converts all variants of newlines to linefeeds (\n).</summary>
		/// <param name="text">The text to convert.</param>
		/// <returns>A copy of the original text with all newline characters or combinations converted to linefeed characters.</returns>
		public static string NewLinesToLineFeed(string text) => EolNotLineFeed.Replace(text, "\n");

		/// <summary>Converts multiple whitespace characters into a single space.</summary>
		/// <param name="text">The text to prune.</param>
		/// <returns>A copy of the original text with multiple whitespace characters, including blank lines, converted to a single space.</returns>
		public static string WhitespaceToSpace([Localizable(false)] string text) => MultipleWhitespacesOrLine.Replace(text, " ");
		#endregion
	}
}
