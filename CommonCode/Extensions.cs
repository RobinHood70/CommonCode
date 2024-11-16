namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using RobinHood70.CommonCode.Properties;

	/// <summary>Extension methods for a variety of types.</summary>
	public static class Extensions
	{
		#region Double Extensions

		/// <summary>Rounds the desired value to a given number of significant digits.</summary>
		/// <param name="value">The value to round.</param>
		/// <param name="digits">The number of significant digits to round to.</param>
		/// <returns>The provided value rounded to a given number of significant digits.</returns>
		public static double RoundSignificant(this double value, int digits)
		{
			if (value == 0)
			{
				return 0;
			}

			var logValue = Math.Floor(Math.Log10(Math.Abs(value)));
			var exp = Math.Pow(10, logValue + 1);
			return exp * Math.Round(value / exp, digits);
		}
		#endregion

		#region Enum Extensions

		/// <summary>Gets each single-bit value of a flags enumeration.</summary>
		/// <typeparam name="T">The enumeration type.</typeparam>
		/// <param name="value">The flags enumeration value to enumerate.</param>
		/// <returns>An enumeration of every single-bit value in the specified flags enumeration.</returns>
		public static IEnumerable<T> GetUniqueFlags<T>(this T value)
			where T : Enum
		{
			var valueLong = ((IConvertible)value).ToUInt64(CultureInfo.InvariantCulture);
			foreach (var enumValue in (T[])value.GetType().GetEnumValues())
			{
				var bitValue = ((IConvertible)enumValue).ToUInt64(CultureInfo.InvariantCulture);
				if ((valueLong & bitValue) != 0 && (bitValue & (bitValue - 1)) == 0)
				{
					yield return enumValue;
				}
			}
		}

		/// <summary>Determines whether or not an enum has any of the provided flags set.</summary>
		/// <typeparam name="T">The enumeration type.</typeparam>
		/// <param name="flagValue">The flags enumeration value to check.</param>
		/// <param name="values">The flag values to check.</param>
		/// <returns><see langword="true"/> if any of the flags in <paramref name="values"/> is set; otherwise, <see langword="false"/>.</returns>
		public static bool HasAnyFlag<T>(this T flagValue, T values)
			where T : Enum
		{
			var numericFlags = ((IConvertible)flagValue).ToUInt64(CultureInfo.InvariantCulture);
			var numericValues = ((IConvertible)values).ToUInt64(CultureInfo.InvariantCulture);
			return (numericFlags & numericValues) != 0;
		}

		/// <summary>Determines whether or not an enum represents a single-bit flag value.</summary>
		/// <typeparam name="T">The enumeration type.</typeparam>
		/// <param name="flagValue">The flags enumeration value to check.</param>
		/// <returns><see langword="true"/> if the flag value represents a single-bit value.</returns>
		public static bool IsUniqueFlag<T>(this T flagValue)
			where T : Enum
		{
			var numericFlags = ((IConvertible)flagValue).ToUInt64(CultureInfo.InvariantCulture);
			return (numericFlags & (numericFlags - 1)) == 0 && numericFlags != 0;
		}
		#endregion

		#region ICollection<T> Extensions

		/// <summary>Adds one collection of items to another.</summary>
		/// <typeparam name="T">The type of items.</typeparam>
		/// <param name="collection">The original collection.</param>
		/// <param name="values">The values to be added.</param>
		public static void AddRange<T>(this ICollection<T> collection, params T[] values) => AddRange(collection, values as IEnumerable<T>);

		/// <summary>Adds one collection of items to another.</summary>
		/// <typeparam name="T">The type of items.</typeparam>
		/// <param name="collection">The original collection.</param>
		/// <param name="values">The collection to be added.</param>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
		{
			ArgumentNullException.ThrowIfNull(collection);
			ArgumentNullException.ThrowIfNull(values);
			foreach (var value in values)
			{
				collection.Add(value);
			}
		}
		#endregion

		#region IEnumerable<T> Extensions

		/// <summary>Casts the enumerable to an IReadOnlyList if possible, or creates a new one if needed.</summary>
		/// <typeparam name="T">The type of the original enumerable.</typeparam>
		/// <param name="enumerable">The enumerable to convert.</param>
		/// <returns>The existing enumerable as an IReadOnlyList or a new list.</returns>
		public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T>? enumerable) => enumerable switch
		{
			null => [],
			IReadOnlyList<T> list => list,
			_ => new List<T>(enumerable).AsReadOnly(),
		};
		#endregion

		#region IFormattable Extensions

		// This was originally implemented as a generic. Why? I have a feeling it might've been something to do with unexpected name resolution at the time, but results seem consistent at this point.

		/// <summary>Convenience method to format any IFormattable value as an invariant value.</summary>
		/// <param name="value">The value to format.</param>
		/// <returns>The value as an invariant string.</returns>
		public static string ToStringInvariant(this IFormattable value)
		{
			ArgumentNullException.ThrowIfNull(value);
			return value.ToString(format: null, CultureInfo.InvariantCulture);
		}
		#endregion

		#region IList<T> Extensions

		/// <summary>Shuffles the specified list into a random order.</summary>
		/// <typeparam name="T">The list type.</typeparam>
		/// <param name="list">The list.</param>
		public static void Shuffle<T>(this IList<T> list)
		{
			ArgumentNullException.ThrowIfNull(list);
			Random random = new();
			var i = list.Count - 1;
			if (i <= 0)
			{
				return;
			}

			while (i >= 0)
			{
				var swapWith = random.Next(list.Count);
				(list[swapWith], list[i]) = (list[i], list[swapWith]);
				i--;
			}
		}
		#endregion

		#region LinkedList<T> Extensions

		/// <summary>Adds a collection of values to the list in the order provided.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="list">The list to add the value to.</param>
		/// <param name="node">The node to add the value after.</param>
		/// <param name="values">The values to add.</param>
		public static void AddAfter<T>(this LinkedList<T> list, LinkedListNode<T> node, IEnumerable<T> values)
		{
			ArgumentNullException.ThrowIfNull(list);
			ArgumentNullException.ThrowIfNull(node);
			ArgumentNullException.ThrowIfNull(values);
			foreach (var newNode in values)
			{
				node = list.AddAfter(node, newNode);
			}
		}

		/// <summary>Adds a collection of values to the list in the order provided.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="list">The list to add the value to.</param>
		/// <param name="node">The node to add the value Before.</param>
		/// <param name="values">The values to add.</param>
		public static void AddBefore<T>(this LinkedList<T> list, LinkedListNode<T> node, IEnumerable<T> values)
		{
			ArgumentNullException.ThrowIfNull(list);
			ArgumentNullException.ThrowIfNull(node);
			ArgumentNullException.ThrowIfNull(values);
			foreach (var newNode in values)
			{
				list.AddBefore(node, newNode);
			}
		}
		#endregion

		#region LinkedListNode<T> Extensions

		/// <summary>Adds a new value to the linked list.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="node">The node to add the values after.</param>
		/// <param name="value">The value to add.</param>
		/// <returns>The new <see cref="LinkedListNode{T}"/> containing value.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown when <paramref name="node"/> does not belong to a linked list.</exception>
		public static LinkedListNode<T> AddAfter<T>(this LinkedListNode<T> node, T value)
		{
			ArgumentNullException.ThrowIfNull(node);
			return node.List is LinkedList<T> list
				? list.AddAfter(node, value)
				: throw new InvalidOperationException(Resources.NoNodeList);
		}

		/// <summary>Adds a collection of values to the list in the order provided.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="node">The node to add the values after.</param>
		/// <param name="values">The values to add.</param>
		public static void AddAfter<T>(this LinkedListNode<T> node, IEnumerable<T> values)
		{
			ArgumentNullException.ThrowIfNull(node);
			ArgumentNullException.ThrowIfNull(values);
			if (node.List is LinkedList<T> list)
			{
				foreach (var newNode in values)
				{
					node = list.AddAfter(node, newNode);
				}
			}
		}

		/// <summary>Adds a new value to the linked list.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="node">The node to add the values before.</param>
		/// <param name="value">The value to add.</param>
		/// <returns>The new <see cref="LinkedListNode{T}"/> containing value.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown when <paramref name="node"/> does not belong to a linked list.</exception>
		public static LinkedListNode<T> AddBefore<T>(this LinkedListNode<T> node, T value)
		{
			ArgumentNullException.ThrowIfNull(node);
			return node.List is LinkedList<T> list
				? list.AddBefore(node, value)
				: throw new InvalidOperationException(Resources.NoNodeList);
		}

		/// <summary>Adds a collection of values to the list in the order provided.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="node">The node to add the values before.</param>
		/// <param name="values">The values to add.</param>
		public static void AddBefore<T>(this LinkedListNode<T> node, IEnumerable<T> values)
		{
			ArgumentNullException.ThrowIfNull(node);
			ArgumentNullException.ThrowIfNull(values);
			if (node.List is LinkedList<T> list)
			{
				foreach (var newNode in values)
				{
					list.AddBefore(node, newNode);
				}
			}
		}
		#endregion

		#region String Extensions

		/// <summary>Limits text to the specified maximum length.</summary>
		/// <param name="text">The text.</param>
		/// <param name="maxLength">The maximum length.</param>
		/// <returns>System.String.</returns>
		/// <remarks>This limits only the initial string length, not the total, so the return value can have a maximum length of maxLength + 3.</remarks>
		[return: NotNullIfNotNull(nameof(text))]
		public static string? Ellipsis(this string? text, int maxLength) =>
			text is string testString &&
			testString.Length > maxLength
				? text[..maxLength] + "..."
				: text;

		/// <summary>Converts the first character of a string to upper-case.</summary>
		/// <param name="text">The string to alter.</param>
		/// <returns>A copy of the original string, with the first charcter converted to upper-case.</returns>
		public static string LowerFirst(this string text) => LowerFirst(text, CultureInfo.InvariantCulture);

		/// <summary>Converts the first character of a string to upper-case.</summary>
		/// <param name="text">The string to alter.</param>
		/// <param name="culture">The culture to use for converting the first character to upper-case.</param>
		/// <returns>A copy of the original string, with the first charcter converted to upper-case.</returns>
		public static string LowerFirst(this string text, CultureInfo culture)
		{
			ArgumentNullException.ThrowIfNull(culture);
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			var retval = text[..1].ToLower(culture);
			return text.Length == 1 ? retval : retval + text[1..];
		}

		/// <summary>Extension shortcut to explicitly compare two strings ordinally.</summary>
		/// <param name="a">The string to compare.</param>
		/// <param name="b">The string to compare the first one with.</param>
		/// <returns><see langword="true"/>if the strings are identical; otherwise, <see langword="false"/>.</returns>
		public static bool OrdinalEquals(this string? a, string? b) => string.Equals(a, b, StringComparison.Ordinal);

		/// <summary>Shortcut to explicitly compare two strings ordinally, ignoring case.</summary>
		/// <param name="a">The string to compare.</param>
		/// <param name="b">The string to compare the first one with.</param>
		/// <returns><see langword="true"/>if the strings are identical; otherwise, <see langword="false"/>.</returns>
		public static bool OrdinalICEquals(this string? a, string? b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

		/// <summary>Takes a camel-case text and adds spaces before each block of one or more upper-case letters.</summary>
		/// <param name="text">The text to convert.</param>
		/// <returns>The original text with spaces inserted.</returns>
		/// <remarks>This method is not highly optimized and may be slow when called repeatedly or with large strings.</remarks>
		[return: NotNullIfNotNull(nameof(text))]
		public static string? UnCamelCase(this string text)
		{
			if (text == null)
			{
				return null;
			}

			text = text.UpperFirst(CultureInfo.CurrentUICulture);
			for (var i = text.Length - 2; i >= 1; i--)
			{
				if (char.IsUpper(text[i]) &&
						(char.IsLower(text[i - 1]) ||
						(char.IsLower(text[i + 1]) && char.IsLetter(text[i - 1]))))
				{
					text = text.Insert(i, " ");
				}
			}

			return text;
		}

		/// <summary>Converts the first character of a string to upper-case.</summary>
		/// <param name="text">The string to alter.</param>
		/// <returns>A copy of the original string, with the first charcter converted to upper-case.</returns>
		public static string UpperFirst(this string text) => UpperFirst(text, CultureInfo.InvariantCulture, findFirstLetter: false);

		/// <summary>Converts the first character of a string to upper-case.</summary>
		/// <param name="text">The string to alter.</param>
		/// <param name="culture">The culture to use for converting the first character to upper-case.</param>
		/// <returns>A copy of the original string, with the first charcter converted to upper-case.</returns>
		public static string UpperFirst(this string text, CultureInfo culture) => UpperFirst(text, culture, findFirstLetter: false);

		/// <summary>Converts the first character of a string to upper-case.</summary>
		/// <param name="text">The string to alter.</param>
		/// <param name="culture">The culture to use for converting the first character to upper-case.</param>
		/// <param name="findFirstLetter">If <see langword="true"/>, searches for the first actual letter in the string instead of trying to upper-case whatever's at position 0.</param>
		/// <returns>A copy of the original string, with the first charcter converted to upper-case.</returns>
		public static string UpperFirst(this string text, CultureInfo culture, bool findFirstLetter)
		{
			ArgumentNullException.ThrowIfNull(culture);
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			var charIndex = 0;
			if (findFirstLetter)
			{
				while (charIndex < text.Length)
				{
					var c = text[charIndex];
					if (char.IsLetter(c))
					{
						break;
					}

					charIndex++;
				}
			}
			else
			{
				charIndex = 0;
			}

			var start = charIndex == 0 ? string.Empty : text[0..charIndex];
			var upper = char.ToUpper(text[charIndex], culture);
			var end = charIndex >= text.Length ? string.Empty : text[(charIndex + 1)..];

			return string.Concat(start, upper, end);
		}
		#endregion

		#region Honeypot Methods
#if DEBUG
#pragma warning disable MA0016 // Prefer return collection abstraction instead of implementation

		// Calls to any of these methods should be replaced by native methods/properties.
		public static void AddRange<T>(this List<T> list, params T[] values)
		{
			ArgumentNullException.ThrowIfNull(list);
			ArgumentNullException.ThrowIfNull(values);
			list.AddRange(values);
		}

		public static IReadOnlyList<T> AsReadOnlyList<T>(this List<T>? list) => list?.AsReadOnly() ?? Array.Empty<T>() as IReadOnlyList<T>;

#pragma warning restore MA0016 // Prefer return collection abstraction instead of implementation
#endif
		#endregion
	}
}