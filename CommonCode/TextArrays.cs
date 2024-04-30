namespace RobinHood70.CommonCode
{
	using System;

	/// <summary>A static class of split values used throughout all related projects. This avoids frequent re-allocations of small array values—particularly important inside loops and frequently-called methods.</summary>
	public static class TextArrays
	{
		// These are all implemented as public fields due to the fact that arrays are strongly discouraged as public properties and the methods these are intended for, such as string.Split, do not allow spans currently.

		/// <summary>A character array containing only an at sign.</summary>
		public static readonly char[] At = ['@'];

		/// <summary>A character array containing only a backslash.</summary>
		public static readonly char[] Backslash = ['\\'];

		/// <summary>A character array containing a space and a hyphen.</summary>
		public static readonly char[] CategorySeparators = [' ', '-'];

		/// <summary>A character array containing only a colon.</summary>
		public static readonly char[] Colon = [':'];

		/// <summary>A character array containing only a comma.</summary>
		public static readonly char[] Comma = [','];

		/// <summary>A character array containing only an equals sign.</summary>
		public static readonly char[] EqualsSign = ['='];

		/// <summary>A character array containing only a linefeeed.</summary>
		public static readonly char[] LineFeed = ['\n'];

		/// <summary>A character array containing a linefeed and a carriage return.</summary>
		public static readonly char[] NewLineChars = ['\n', '\r'];

		/// <summary>A character array containing only a null character.</summary>
		public static readonly char[] Null = ['\0'];

		/// <summary>A character array containing only an octothorpe (aka: number sign, pound sign, hashtag).</summary>
		public static readonly char[] Octothorpe = ['#'];

		/// <summary>A character array containing open and close parentheses.</summary>
		public static readonly char[] Parentheses = ['(', ')'];

		/// <summary>A character array containing only a period.</summary>
		public static readonly char[] Period = ['.'];

		/// <summary>A character array containing only a pipe symbol.</summary>
		public static readonly char[] Pipe = ['|'];

		/// <summary>A character array containing only a plus sign.</summary>
		public static readonly char[] Plus = ['+'];

		/// <summary>A character array containing only a semicolon.</summary>
		public static readonly char[] Semicolon = [';'];

		/// <summary>A character array containing only a forward slash.</summary>
		public static readonly char[] Slash = ['/'];

		/// <summary>A character array containing only a space.</summary>
		public static readonly char[] Space = [' '];

		/// <summary>A character array containing a open and close brackets (aka: square brackets).</summary>
		public static readonly char[] SquareBrackets = ['[', ']'];

		/// <summary>A character array containing only a tab character.</summary>
		public static readonly char[] Tab = ['\t'];

		/// <summary>A string array containing a comma-space combination.</summary>
		public static readonly string[] CommaSpace = [", "];

		/// <summary>A string array containing only a NewLine character as defined by the current Environment.NewLine property.</summary>
		public static readonly string[] EnvironmentNewLine = [Environment.NewLine];

		/// <summary>A string array containing only a MediaWiki link terminator (]]).</summary>
		public static readonly string[] LinkTerminator = ["]]"];

		/// <summary>A string array containing only a MediaWiki link marker ([[).</summary>
		public static readonly string[] LinkMarker = ["[["];

		/// <summary>A string array containing only a MediaWiki template marker ({{).</summary>
		public static readonly string[] TemplateTerminator = ["}}"];

		/// <summary>A string array containing only a MediaWiki template terminator (}}).</summary>
		public static readonly string[] TemplateMarker = ["{{"];
	}
}