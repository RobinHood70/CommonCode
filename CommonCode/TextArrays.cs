namespace RobinHood70.CommonCode
{
	using System;

	/// <summary>A static class of split values used throughout all related projects. This avoids frequent re-allocations of small array values—particularly important inside loops and frequently-called methods.</summary>
	public static class TextArrays
	{
		// These are all implemented as public fields due to the fact that arrays are strongly discouraged as public properties and the methods these are intended for, such as string.Split, do not allow spans currently.
#pragma warning disable CS1591 // These should all be self-documenting.
		public static readonly char[] At = { '@' };
		public static readonly char[] CategorySeparators = { ' ', '-' };
		public static readonly char[] Colon = { ':' };
		public static readonly char[] Comma = { ',' };
		public static readonly char[] EqualsSign = { '=' };
		public static readonly char[] LineFeed = { '\n' };
		public static readonly char[] NewLineChars = { '\n', '\r' };
		public static readonly char[] Null = { '\0' };
		public static readonly char[] Octothorp = { '#' };
		public static readonly char[] Parentheses = { '(', ')' };
		public static readonly char[] Period = { '.' };
		public static readonly char[] Pipe = { '|' };
		public static readonly char[] Plus = { '+' };
		public static readonly char[] Semicolon = { ';' };
		public static readonly char[] Slash = { '/' };
		public static readonly char[] Space = { ' ' };
		public static readonly char[] SquareBrackets = { '[', ']' };
		public static readonly char[] Tab = { '\t' };

		public static readonly string[] CommaSpace = { ", " };
		public static readonly string[] EnvironmentNewLine = { Environment.NewLine };
		public static readonly string[] LinkTerminator = { "[[" };
		public static readonly string[] LinkMarker = { "[[" };
		public static readonly string[] TemplateTerminator = { "{{" };
		public static readonly string[] TemplateMarker = { "{{" };
#pragma warning restore CS1591
	}
}
