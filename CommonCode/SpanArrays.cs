namespace RobinHood70.CommonCode;

using System;
using System.Runtime.InteropServices;

/// <summary>A ref struct of spans for functions like Split() and Trim(). This avoids re-allocations of small array values—particularly important inside loops and frequently-called methods.</summary>
[StructLayout(LayoutKind.Auto)]
public readonly ref struct SpanArrays
{
	#region Fields

	/// <summary>Gets a character array containing only an at sign.</summary>
	public readonly ReadOnlySpan<char> At = "@";

	/// <summary>Gets a character array containing only a backslash.</summary>
	public readonly ReadOnlySpan<char> Backslash = "\\";

	/// <summary>Gets a character array containing a space and a hyphen.</summary>
	public readonly ReadOnlySpan<char> CategorySeparators = " -";

	/// <summary>Gets a character array containing only a colon.</summary>
	public readonly ReadOnlySpan<char> Colon = ":";

	/// <summary>Gets a character array containing only a comma.</summary>
	public readonly ReadOnlySpan<char> Comma = ",";

	/// <summary>Gets a character array containing open and close curly brackets.</summary>
	public readonly ReadOnlySpan<char> CurlyBrackets = "{}";

	/// <summary>Gets a character array containing only an equals sign.</summary>
	public readonly ReadOnlySpan<char> EqualsSign = "=";

	/// <summary>Gets a character array containing only a linefeeed.</summary>
	public readonly ReadOnlySpan<char> LineFeed = "\n";

	/// <summary>Gets a character array containing a linefeed and a carriage return.</summary>
	public readonly ReadOnlySpan<char> NewLineChars = "\n\r";

	/// <summary>Gets a character array containing only a null character.</summary>
	public readonly ReadOnlySpan<char> Null = "\0";

	/// <summary>Gets a character array containing only an octothorpe (aka: number sign, pound sign, hashtag).</summary>
	public readonly ReadOnlySpan<char> Octothorpe = "#";

	/// <summary>Gets a character array containing open and close parentheses.</summary>
	public readonly ReadOnlySpan<char> Parentheses = "()";

	/// <summary>Gets a character array containing only a period.</summary>
	public readonly ReadOnlySpan<char> Period = ".";

	/// <summary>Gets a character array containing only a pipe symbol.</summary>
	public readonly ReadOnlySpan<char> Pipe = "|";

	/// <summary>Gets a character array containing only a plus sign.</summary>
	public readonly ReadOnlySpan<char> Plus = "+";

	/// <summary>Gets a character array containing only a semicolon.</summary>
	public readonly ReadOnlySpan<char> Semicolon = ";";

	/// <summary>Gets a character array containing only a forward slash.</summary>
	public readonly ReadOnlySpan<char> Slash = "/";

	/// <summary>Gets a character array containing only a space.</summary>
	public readonly ReadOnlySpan<char> Space = " ";

	/// <summary>Gets a character array containing a space and a tab.</summary>
	public readonly ReadOnlySpan<char> SpaceAndTab = " \t";

	/// <summary>Gets a character array containing only a tab.</summary>
	public readonly ReadOnlySpan<char> Tab = "\t";
	#endregion

	#region Constructors

	/// <summary>Initializes a new instance of the <see cref="SpanArrays"/> struct.</summary>
	public SpanArrays()
	{
	}
	#endregion
}