namespace RobinHood70.CommonCode
{
	using System.ComponentModel;
	using System.Text.RegularExpressions;

	/// <summary>A class that provides several common Regex patterns.</summary>
	public static partial class RegexLibrary
	{
		/// <summary>Gets a <see cref="Regex"/> that finds any newline character or combination of chacters except a carriage-return/linefeed on its own.</summary>
		public static Regex EolNotCrlf { get; } = EolNotCrlfInternal();

		/// <summary>Gets a <see cref="Regex"/> that finds any newline character or combination of chacters except a linefeed on its own.</summary>
		public static Regex EolNotLineFeed { get; } = EolNotLineFeedInternal();

		/// <summary>Gets a <see cref="Regex"/> that finds multiple spaces (and only spaces) used in sequence.</summary>
		public static Regex MultipleSpaces { get; } = MultipleSpacesInternal();

		/// <summary>Gets a <see cref="Regex"/> that finds multiple whitespace characters used in sequence.</summary>
		public static Regex MultipleWhitespaces { get; } = MultipleWhitespacesInternal();

		/// <summary>Gets a <see cref="Regex"/> that finds multiple spaces used in sequence and any occurrence of a newline character.</summary>
		public static Regex MultipleWhitespacesOrLine { get; } = MultipleWhitespacesOrLineInternal();

		#region Public Methods

		/// <summary>Converts all variants of newlines to a carriage-return/linefeed combination (\r\n).</summary>
		/// <param name="text">The text to convert.</param>
		/// <returns>A copy of the original text with all newline characters or combinations converted to carriage-return/linefeed characters.</returns>
		public static string NewLinesToCrlf(string text) => EolNotCrlfInternal().Replace(text, "\r\n");

		/// <summary>Converts all variants of newlines to linefeeds (\n).</summary>
		/// <param name="text">The text to convert.</param>
		/// <returns>A copy of the original text with all newline characters or combinations converted to linefeed characters.</returns>
		public static string NewLinesToLineFeed(string text) => EolNotLineFeedInternal().Replace(text, "\n");

		/// <summary>Prunes multiple space characters, turning them into a single space.</summary>
		/// <param name="text">The text to prune.</param>
		/// <returns>A copy of the original text with excess spaces removed.</returns>
		public static string PruneExcessSpaces([Localizable(false)] string text) => MultipleWhitespacesOrLineInternal().Replace(text, " ");

		/// <summary>Prunes multiple whitespace characters, turning them into a single space. Unlike <see cref="PruneExcessSpaces(string)"/>, this function prunes tabs, newlines, and anything else Regex considers to be whitespace.</summary>
		/// <param name="text">The text to prune.</param>
		/// <returns>A copy of the original text with excess whitespace removed.</returns>
		public static string PruneExcessWhitespace([Localizable(false)] string text) => MultipleWhitespacesOrLineInternal().Replace(text, " ");

		[GeneratedRegex(@"((?<!\n)\r\n?|(?<!\r)\n\r?)", RegexOptions.ExplicitCapture, Globals.DefaultGeneratedRegexTimeout)]
		private static partial Regex EolNotLineFeedInternal();

		[GeneratedRegex(@"((?<!\r)\n\r?|(?<!\n)\r(?!\n))", RegexOptions.ExplicitCapture, Globals.DefaultGeneratedRegexTimeout)]
		private static partial Regex EolNotCrlfInternal();

		[GeneratedRegex(" {2,}", RegexOptions.None, Globals.DefaultGeneratedRegexTimeout)]
		private static partial Regex MultipleSpacesInternal();

		[GeneratedRegex(@"\s{2,}", RegexOptions.None, Globals.DefaultGeneratedRegexTimeout)]
		private static partial Regex MultipleWhitespacesInternal();

		[GeneratedRegex(@"(\s{2,}|[\r\n]+)", RegexOptions.ExplicitCapture, Globals.DefaultGeneratedRegexTimeout)]
		private static partial Regex MultipleWhitespacesOrLineInternal();
		#endregion
	}
}