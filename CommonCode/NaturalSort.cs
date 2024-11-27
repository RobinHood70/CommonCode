namespace RobinHood70.CommonCode;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

// Adapted from http://stackoverflow.com/questions/248603/natural-sort-order-in-c-sharp/11624488#11624488
// For some reason, the default Regex produces a 3-element split with strings like "a100" (a, 100, ""), but this does not affect sorting.
// A + could also be added to handle explicitly positive numbers, but this can lead to odd sorting like "a100", "a+100", "a100". The fix would be to add "x.Length.CompareTo(y.Length)" if splitX and splitY are equal, but this was unnecessary for my purposes, so left out.

/// <summary>An IComparer that provides natural sorting for mixed text and numeric strings.</summary>
public sealed partial class NaturalSort : IComparer<string>, IComparer
{
	#region Constructors

	private NaturalSort()
	{
	}
	#endregion

	#region Public Static Properties

	/// <summary>Gets a singleton instance of the NaturalSort class.</summary>
	/// <value>The instance.</value>
	public static NaturalSort Instance { get; } = new NaturalSort();
	#endregion

	#region Public Static Methods

	/// <summary>Compares two strings and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
	/// <param name="x">The first string to compare.</param>
	/// <param name="y">The second string to compare.</param>
	/// <param name="culture">The culture to use for both numeric and string comparisons.</param>
	/// <param name="options">The <see cref="CompareOptions"/> to use for string comparisons.</param>
	/// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
	/// Less than zero: <paramref name="x" /> is less than <paramref name="y" />.
	/// Zero: <paramref name="x" /> equals <paramref name="y" />.
	/// Greater than zero: <paramref name="x" /> is greater than <paramref name="y" />.</returns>
	[SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "False hit, culture is specified.")]
	[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Ironically suppressing a false unnecessary suppression message.")]
	public static int Compare(string? x, string? y, CultureInfo culture, CompareOptions options)
	{
		// This is not the fastest possible algorithm, since it re-parses strings every time Compare is called, but it has the advantage of being fairly straight-forward.
		ArgumentNullException.ThrowIfNull(culture);
		if (x == null)
		{
			return y == null ? 0 : -1;
		}

		if (y == null)
		{
			return 1;
		}

		var splitX = NumberRegex().Split(x);
		var splitY = NumberRegex().Split(y);
		var len = Math.Min(splitX.Length, splitY.Length);
		for (var i = 0; i < len; i++)
		{
			var result = (
				Parse(splitX[i], culture, out var numX) &&
				Parse(splitY[i], culture, out var numY))
					? numX.CompareTo(numY)
					: string.Compare(splitX[i], splitY[i], culture, options);
			if (result != 0)
			{
				return result;
			}
		}

		return splitX.Length.CompareTo(splitY.Length);

		static bool Parse(string value, CultureInfo culture, out double numX) => double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, culture.NumberFormat, out numX);
	}
	#endregion

	#region Public Methods

	/// <summary>Compares two strings and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
	/// <param name="x">The first string to compare.</param>
	/// <param name="y">The second string to compare.</param>
	/// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
	/// Less than zero: <paramref name="x" /> is less than <paramref name="y" />.
	/// Zero: <paramref name="x" /> equals <paramref name="y" />.
	/// Greater than zero: <paramref name="x" /> is greater than <paramref name="y" />.</returns>
	public int Compare(string? x, string? y) => Compare(x, y, CultureInfo.CurrentCulture, CompareOptions.None);

	/// <summary>Compares two objects as strings and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
	/// <param name="x">The first string to compare.</param>
	/// <param name="y">The second string to compare.</param>
	/// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
	/// Less than zero: <paramref name="x" /> is less than <paramref name="y" />.
	/// Zero: <paramref name="x" /> equals <paramref name="y" />.
	/// Greater than zero: <paramref name="x" /> is greater than <paramref name="y" />.</returns>
	public int Compare(object? x, object? y) => Compare(x as string, y as string, CultureInfo.CurrentCulture, CompareOptions.None);
	#endregion

	#region Partial Methods
	[GeneratedRegex(@"(\d+([\.,]\d+)?)", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
	private static partial Regex NumberRegex();
	#endregion
}