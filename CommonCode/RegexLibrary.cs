namespace RobinHood70.CommonCode
{
	using System.ComponentModel;
	using System.Text.RegularExpressions;
	using static RobinHood70.CommonCode.Globals;

	public static class RegexLibrary
	{
		public static Regex EolNotCrlf { get; } = new Regex(@"((?<!\r)\n\r?|(?<!\n)\r(?!\n))", RegexOptions.ExplicitCapture, DefaultRegexTimeout);

		public static Regex EolNotLineFeed { get; } = new Regex(@"((?<!\n)\r\n?|(?<!\r)\n\r?)", RegexOptions.ExplicitCapture, DefaultRegexTimeout);

		public static Regex MultipleSpaces { get; } = new Regex(" {2,}", RegexOptions.None, DefaultRegexTimeout);

		public static Regex MultipleWhitespaces { get; } = new Regex(@"\s{2,}", RegexOptions.None, DefaultRegexTimeout);

		public static Regex MultipleWhitespacesOrLine { get; } = new Regex(@"(\s{2,}|[\r\n]+)", RegexOptions.ExplicitCapture, DefaultRegexTimeout);

		#region Public Methods
		public static string NewLinesToCrlf(string text) => EolNotCrlf.Replace(text, "\r\n");

		public static string NewLinesToLineFeed(string text) => EolNotLineFeed.Replace(text, "\n");

		public static string WhitespaceToSpace([Localizable(false)] string text) => MultipleWhitespacesOrLine.Replace(text, " ");
		#endregion
	}
}
