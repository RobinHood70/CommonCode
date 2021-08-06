namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using RobinHood70.CommonCode.Properties;

	/// <summary>Extension methods for a variety of types.</summary>
	public static class Extensions
	{
		#region BinaryReader Extensions

		/// <summary>Reads a length-prefixed string. Both length and characters should be bytes.</summary>
		/// <param name="reader">The <see cref="BinaryReader"/> to read from.</param>
		/// <returns>The string.</returns>
		public static string BString8(this BinaryReader reader)
		{
			var size = reader.NotNull(nameof(reader)).ReadByte();
			return new string(reader.ReadChars(size));
		}

		/// <summary>Reads a null-terminated string. Characters should be bytes.</summary>
		/// <param name="reader">The <see cref="BinaryReader"/> to read from.</param>
		/// <returns>The string.</returns>
		/// <remarks>For fixed-length strings, it's normally better to read the entire thing and then trim everything after the null. This function is intended for those cases where the length is unknown.</remarks>
		public static string ZString(this BinaryReader reader)
		{
			StringBuilder retval = new();
			var c = reader.NotNull(nameof(reader)).ReadChar();
			while (c != '\0')
			{
				retval.Append(c);
				c = reader.ReadChar();
			}

			return retval.ToString();
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
			if (values != null)
			{
				if (collection.NotNull(nameof(collection)) is List<T> list)
				{
					list.AddRange(values);
				}
				else
				{
					foreach (var value in values)
					{
						collection.Add(value);
					}
				}
			}
		}
		#endregion

		#region IDictionary<TKey, TValue> Extensions

		/// <summary>Adds the items from one dictionary (or other set of key-value pairs) to another.</summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dictionary">The dictionary to add to.</param>
		/// <param name="items">The items to add.</param>
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
			where TKey : notnull
		{
			dictionary.ThrowNull(nameof(dictionary));
			foreach (var item in items.NotNull(nameof(items)))
			{
				dictionary.Add(item.Key, item.Value);
			}
		}

		/// <summary>Convenience method to convert a dictionary to read-only.</summary>
		/// <typeparam name="TKey">The key-type of the <paramref name="dictionary" /> (inferred).</typeparam>
		/// <typeparam name="TValue">The value-type of the <paramref name="dictionary" /> (inferred).</typeparam>
		/// <param name="dictionary">The dictionary to convert.</param>
		/// <returns>A read-only dictionary based on the provided dictionary. If the input was null, an empty read-only dictionary is returned.</returns>
		public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary)
			where TKey : notnull => new(dictionary ?? new Dictionary<TKey, TValue>());

		/// <summary>Gets the value of the "first" item in a dictionary.</summary>
		/// <typeparam name="TKey">The key type of the dictionary.</typeparam>
		/// <typeparam name="TValue">The value type of the dictionary.</typeparam>
		/// <param name="dictionary">The dictionary from which to retrieve the first value.</param>
		/// <returns>The first value in the enumerable, or throws an error.</returns>
		/// <exception cref="KeyNotFoundException">The list was empty.</exception>
		/// <remarks>Although the current implementation of dictionaries appears to maintain insertion order, this is not guaranteed. This function should be used only to get the value from a single-entry dictionary, or to get a unspecified value from a multi-entry dictionary.</remarks>
		/// <exception cref="InvalidOperationException">Thrown when <paramref name="dictionary"/> is empty.</exception>
		public static TValue First<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
			where TKey : notnull
		{
			using var enumerator = dictionary.NotNull(nameof(dictionary)).GetEnumerator();
			return enumerator.MoveNext() ? enumerator.Current.Value : throw new InvalidOperationException();
		}
		#endregion

		#region IEnumerable<T> Extensions

		/// <summary>Casts the enumerable to an IReadOnlyList if possible, or creates a new one if needed.</summary>
		/// <typeparam name="T">The type of the original enumerable.</typeparam>
		/// <param name="enumerable">The enumerable to convert.</param>
		/// <returns>The existing enumerable as an IReadOnlyList or a new list.</returns>
		public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T>? enumerable) => enumerable switch
		{
			null => Array.Empty<T>(),
			IReadOnlyList<T> list => list,
			var other when other.IsEmpty() => Array.Empty<T>(),
			_ => new List<T>(enumerable),
		};

		/// <summary>Determines whether an IEnumerable<typeparamref name="T"/> contains the specified value.</summary>
		/// <typeparam name="T">The type of the original enumerable.</typeparam>
		/// <param name="enumerable">The enumerable to convert.</param>
		/// <param name="value">The value to find.</param>
		/// <returns><see langword="true"/> if the collection contains the specified value; otherwise, <see langword="false"/>.</returns>
		public static bool Contains<T>(this IEnumerable<T> enumerable, T value) => (enumerable as ICollection<T>)?.Contains(value) ?? Contains(enumerable, value, null);

		/// <summary>Determines whether an IEnumerable<typeparamref name="T"/> contains the specified value.</summary>
		/// <typeparam name="T">The type of the original enumerable.</typeparam>
		/// <param name="enumerable">The enumerable to convert.</param>
		/// <param name="value">The value to find.</param>
		/// <param name="comparer">The equality comparer to use to make the comparison.</param>
		/// <returns><see langword="true"/> if the collection contains the specified value; otherwise, <see langword="false"/>.</returns>
		public static bool Contains<T>(this IEnumerable<T> enumerable, T value, IEqualityComparer<T>? comparer)
		{
			// It's understandable why this wasn't in IEnumerable<T>, since enumerating can potentially be a slow-running operation, but it's beyond me why this wasn't put into IReadOnlyCollection<T>. MS covered it with Linq, using a virtually identical implementation to this, but that's still only a workaround (as is this).
			comparer ??= EqualityComparer<T>.Default;
			foreach (var item in enumerable.NotNull(nameof(enumerable)))
			{
				if (comparer.Equals(item, value))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>Gets the first item of the collection, or the specified default value.</summary>
		/// <typeparam name="T">The collection type.</typeparam>
		/// <param name="enumerable">The collection to enumerate.</param>
		/// <returns>The first item in the collection or the specified default value.</returns>
		[return: MaybeNull]
		public static T First<T>(this IEnumerable<T>? enumerable) => First(enumerable, default!);

		/// <summary>Gets the first item of the collection, or the specified default value.</summary>
		/// <typeparam name="T">The collection type.</typeparam>
		/// <param name="enumerable">The collection to enumerate.</param>
		/// <param name="defaultValue">The default value to use if the collection is empty.</param>
		/// <returns>The first item in the collection or the specified default value.</returns>
		[return: MaybeNull]
		public static T First<T>(this IEnumerable<T>? enumerable, [MaybeNull] T defaultValue)
		{
			if (enumerable != null)
			{
				using var enumerator = enumerable.GetEnumerator();
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}

			return defaultValue;
		}
		#endregion

		#region IEnumerable Extensions

		/// <summary>Determines whether an IEnumerable has items.</summary>
		/// <param name="enumerable">The enumerable to check.</param>
		/// <returns><see langword="true"/> if the list is non-null and has at least one item; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="KeyNotFoundException">The list was empty.</exception>
		public static bool IsEmpty(this IEnumerable? enumerable) => enumerable?.GetEnumerator().MoveNext() != true;
		#endregion

		#region IFormattable Extensions

		// This was originally implemented as a generic. Why? I have a feeling it might've been something to do with unexpected name resolution at the time, but results seem consistent at this point.

		/// <summary>Convenience method to format any IFormattable value as an invariant value.</summary>
		/// <param name="value">The value to format.</param>
		/// <returns>The value as an invariant string.</returns>
		public static string ToStringInvariant(this IFormattable value) => value
			.NotNull(nameof(value))
			.ToString(null, CultureInfo.InvariantCulture);
		#endregion

		#region IList<T> Extensions

		/// <summary>Shuffles the specified list into a random order.</summary>
		/// <typeparam name="T">The list type.</typeparam>
		/// <param name="list">The list.</param>
		public static void Shuffle<T>(this IList<T>? list)
		{
			Random random = new();
			var i = list.NotNull(nameof(list)).Count - 1;
			if (i <= 0)
			{
				return;
			}

			while (i >= 0)
			{
				var swapWith = random.Next(list.Count);
				var value = list[i];
				list[i] = list[swapWith];
				list[swapWith] = value;
				i--;
			}
		}
		#endregion

		#region IReadOnlyDictionary<TKey, TValue> Extentions

		/// <summary>Tries to find a substitute value in a dictionary for the key provided.</summary>
		/// <typeparam name="T">The type for both the key and the value.</typeparam>
		/// <param name="dictionary">The dictionary to search.</param>
		/// <param name="key">The key to look for.</param>
		/// <returns>The substitute value if one is found; otherwise, the original key.</returns>
		public static T Substitute<T>(this IReadOnlyDictionary<T, T> dictionary, T key) =>
			dictionary
			.NotNull(nameof(dictionary))
			.TryGetValue(key, out var retval)
				? retval
				: key;

		/// <summary>Tries to find a substitute value in a dictionary for the key provided.</summary>
		/// <typeparam name="TKey">The type for the key.</typeparam>
		/// <typeparam name="TValue">The type for the value.</typeparam>
		/// <param name="dictionary">The dictionary to search.</param>
		/// <param name="key">The key to look for.</param>
		/// <param name="defaultValue">The default value to use if the key is not found.</param>
		/// <returns>The substitute value if one is found; otherwise, the default value.</returns>
		public static TValue Substitute<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) =>
			dictionary
			.NotNull(nameof(dictionary))
			.TryGetValue(key, out var retval)
				? retval
				: defaultValue;
		#endregion

		#region LinkedList<T> Extensions

		/// <summary>Adds a collection of values to the list in the order provided.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="list">The list to add the value to.</param>
		/// <param name="node">The node to add the value after.</param>
		/// <param name="values">The values to add.</param>
		public static void AddAfter<T>(this LinkedList<T> list, LinkedListNode<T> node, IEnumerable<T> values)
		{
			list.ThrowNull(nameof(list));
			node.ThrowNull(nameof(node));
			foreach (var newNode in values.NotNull(nameof(values)))
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
			list.ThrowNull(nameof(list));
			node.ThrowNull(nameof(node));
			foreach (var newNode in values.NotNull(nameof(values)))
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
		public static LinkedListNode<T> AddAfter<T>(this LinkedListNode<T> node, T value) =>
			node.NotNull(nameof(node)).List is LinkedList<T> list
				? list.AddAfter(node, value)
				: throw new InvalidOperationException(Resources.NoNodeList);

		/// <summary>Adds a collection of values to the list in the order provided.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="node">The node to add the values after.</param>
		/// <param name="values">The values to add.</param>
		public static void AddAfter<T>(this LinkedListNode<T> node, IEnumerable<T> values)
		{
			if (node.NotNull(nameof(node)).List is LinkedList<T> list)
			{
				foreach (var newNode in values.NotNull(nameof(values)))
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
		public static LinkedListNode<T> AddBefore<T>(this LinkedListNode<T> node, T value) =>
			node.NotNull(nameof(node)).List is LinkedList<T> list
				? list.AddBefore(node, value)
				: throw new InvalidOperationException(Resources.NoNodeList);

		/// <summary>Adds a collection of values to the list in the order provided.</summary>
		/// <typeparam name="T">The element type of the linked list.</typeparam>
		/// <param name="node">The node to add the values before.</param>
		/// <param name="values">The values to add.</param>
		public static void AddBefore<T>(this LinkedListNode<T> node, IEnumerable<T> values)
		{
			if (node.NotNull(nameof(node)).List is LinkedList<T> list)
			{
				foreach (var newNode in values.NotNull(nameof(values)))
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
		[return: NotNullIfNotNull("text")]
		public static string? Ellipsis(this string? text, int maxLength) =>
			text is string testString &&
			testString.Length > maxLength
				? text.Substring(0, maxLength) + "..."
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
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			var retval = text.Substring(0, 1).ToLower(culture.NotNull(nameof(culture)));
			return text.Length == 1 ? retval : retval + text[1..];
		}

		/// <summary>Extension shortcut to explicitly compare two strings ordinally.</summary>
		/// <param name="a">The string to compare.</param>
		/// <param name="b">The string to compare the first one with.</param>
		/// <returns><see langword="true"/>if the strings are identical; otherwise, <see langword="false"/>.</returns>
		public static bool OrdinalEquals(this string? a, string? b) => string.Equals(a, b, StringComparison.Ordinal);

		/// <summary>Takes a camel-case text and adds spaces before each block of one or more upper-case letters.</summary>
		/// <param name="text">The text to convert.</param>
		/// <returns>The original text with spaces inserted.</returns>
		[return: NotNullIfNotNull("text")]
		public static string? UnCamelCase(this string text)
		{
			if (text == null)
			{
				return null;
			}

			text = text.UpperFirst(CultureInfo.CurrentUICulture);
			var i = text.Length - 2;
			while (i > 0)
			{
				if (char.IsUpper(text[i]) &&
						(char.IsLower(text[i - 1]) ||
						(char.IsLower(text[i + 1]) && char.IsLetter(text[i - 1]))))
				{
					text = text.Insert(i, " ");
				}

				i--;
			}

			return text;
		}

		/// <summary>Converts the first character of a string to upper-case.</summary>
		/// <param name="text">The string to alter.</param>
		/// <returns>A copy of the original string, with the first charcter converted to upper-case.</returns>
		public static string UpperFirst(this string text) => UpperFirst(text, CultureInfo.InvariantCulture);

		/// <summary>Converts the first character of a string to upper-case.</summary>
		/// <param name="text">The string to alter.</param>
		/// <param name="culture">The culture to use for converting the first character to upper-case.</param>
		/// <returns>A copy of the original string, with the first charcter converted to upper-case.</returns>
		public static string UpperFirst(this string text, CultureInfo culture)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			var retval = text.Substring(0, 1).ToUpper(culture.NotNull(nameof(culture)));
			return text.Length == 1 ? retval : retval + text[1..];
		}
		#endregion

		#region Honeypot Methods
#if DEBUG
#pragma warning disable MA0016 // Prefer return collection abstraction instead of implementation

		// Any calls to any of these methods should be replaced by native methods/properties.
		public static void AddRange<T>(this List<T> list, params T[] values) => list.NotNull(nameof(list)).AddRange(values.NotNull(nameof(values)));

		public static IReadOnlyList<T> AsReadOnlyList<T>(this List<T>? list) => list?.AsReadOnly() ?? Array.Empty<T>() as IReadOnlyList<T>;

		[return: MaybeNull]
		public static T First<T>(this IReadOnlyList<T> list) => list == null ? default : list[0];

		[return: MaybeNull]
		public static T First<T>(this IReadOnlyList<T> list, [AllowNull] T defaultValue) => list == null ? defaultValue : list[0];

		public static bool IsEmpty(this ICollection? collection) => collection == null || collection.Count == 0;

#pragma warning restore MA0016 // Prefer return collection abstraction instead of implementation
#endif
		#endregion
	}
}