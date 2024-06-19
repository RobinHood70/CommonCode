﻿namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;

	#region Public Delegates

	/// <summary>A strongly typed event handler delegate.</summary>
	/// <typeparam name="TSender">The type of the sender.</typeparam>
	/// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
	/// <param name="sender">The sender.</param>
	/// <param name="eventArgs">The event data.</param>
	// From: http://stackoverflow.com/questions/1046016/event-signature-in-net-using-a-strong-typed-sender and http://msdn.microsoft.com/en-us/library/sx2bwtw7.aspx. Originally had a TEventArgs : EventArgs constraint, but mirroring EventHandler<TEventArgs>, I removed it.
	[Serializable]
	public delegate void StrongEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs eventArgs);
	#endregion

	#region Public Enumerations

	/// <summary>The hash type to return from the GetHash() functions.</summary>
	public enum HashType
	{
		/// <summary>Message Digest 5 (MD5) hash.</summary>
		Md5,

		/// <summary>Secure Hash Algorithm 1 (SHA1) hash.</summary>
		Sha1
	}
	#endregion

	/// <summary>Global helper methods that are useful in a variety of scenarios.</summary>
	public static class Globals
	{
		#region Public Constants

		/// <summary>Constant text "Unknown" for times when localized text is unwanted/unavailable.</summary>
		public const string Unknown = "Unknown";
		#endregion

		#region Public Properties

		/// <summary>Gets a <see cref="TimeSpan"/> that is a good general time to abort Regex operations after.</summary>
		public static TimeSpan DefaultRegexTimeout { get; } = TimeSpan.FromSeconds(5);
		#endregion

		#region Public Methods

		/// <summary>Throws an exception if the input value is null.</summary>
		/// <param name="name">The name of the parameter in the original method.</param>
		/// <exception cref="ArgumentNullException">Always thrown.</exception>
		/// <returns>An <see cref="ArgumentNullException"/> for the specified parameter name.</returns>
		public static ArgumentNullException ArgumentNull(string name) => new(name);

		/// <summary>A CompareTo function that returns null instead of zero.</summary>
		/// <typeparam name="T">The type of the values to compare.</typeparam>
		/// <param name="x">The first value.</param>
		/// <param name="y">The second value.</param>
		/// <returns><see langword="null"/> if the values are identical; otherwise, 1 or -1 as a normal <see cref="IComparable{T}.CompareTo"/> function would.</returns>
		/// <remarks>The purpose of this function is to allow simple comparisons of the form <c>x1.ChainedCompareTo(x2) ?? y1.ChainedCompareTo(y2) ?? 0</c>, such that each CompareTo falls through to the next one if the values are equal. The final value should always be zero in order to correctly return zero (instead of null) if all values are equal.</remarks>
		public static int? ChainedCompareTo<T>(T x, T y)
			where T : IComparable<T>
		{
			var retval = x.CompareTo(y);
			return retval == 0 ? null : retval;
		}

		/// <summary>Convenience method so that CurrentCulture and Invariant are all in the same class for both traditional and formattable strings, and are used the same way.</summary>
		/// <param name="text">The text to format.</param>
		/// <param name="value">The value of the parameter in the <paramref name="text" /> parameter.</param>
		/// <returns>The formatted text.</returns>
		public static string CurrentCulture(string text, object? value) => string.Format(CultureInfo.CurrentCulture, text, value);

		/// <summary>Convenience method so that CurrentCulture and Invariant are all in the same class for both traditional and formattable strings, and are used the same way.</summary>
		/// <param name="text">The text to format.</param>
		/// <param name="value1">The value of the first parameter in the <paramref name="text" /> parameter.</param>
		/// <param name="value2">The value of the second parameter in the <paramref name="text" /> parameter.</param>
		/// <returns>The formatted text.</returns>
		public static string CurrentCulture(string text, object? value1, object? value2) => string.Format(CultureInfo.CurrentCulture, text, value1, value2);

		/// <summary>Convenience method so that CurrentCulture and Invariant are all in the same class for both traditional and formattable strings, and are used the same way.</summary>
		/// <param name="text">The text to format.</param>
		/// <param name="firstValue">The first value of any parameters in the <paramref name="text"/>.</param>
		/// <param name="values">The values of any parameters in the <paramref name="text" /> parameter.</param>
		/// <returns>The formatted text.</returns>
		/// <remarks>The method signature is done this way because calling CurrentCulture with no parameters is nearly unintended, since no parameters require formatting. In the rare event where you're formatting a constant value according via CurrentCulture (e.g., <c>{ 1.0:2 }</c>), use the long-form <c>string.Format(CultureInfo.CurrentCulture, text)</c> to achieve the same effect.</remarks>
		public static string CurrentCulture(string text, object? firstValue, params object?[] values)
		{
			static string ManyValues(string text, object? firstValue, object?[] values)
			{
				List<object?>? combinedValues =
				[
					firstValue, .. values
				];

				return string.Format(CultureInfo.CurrentCulture, text, combinedValues);
			}

			ArgumentNullException.ThrowIfNull(values);
			return values.Length switch
			{
				0 => string.Format(CultureInfo.CurrentCulture, text, firstValue),
				1 => string.Format(CultureInfo.CurrentCulture, text, firstValue, values[0]),
				2 => string.Format(CultureInfo.CurrentCulture, text, firstValue, values[0], values[1]),
				_ => ManyValues(text, firstValue, values)
			};
		}

		/// <summary>Works around Uri.EscapeDataString's length limits.</summary>
		/// <param name="dataString">The string to escape.</param>
		/// <returns>The escaped string.</returns>
		public static string EscapeDataString(string dataString)
		{
			if (string.IsNullOrEmpty(dataString))
			{
				return dataString;
			}

			StringBuilder sb = new(dataString.Length * 2);
			var offset = 0;
			while (offset < dataString.Length)
			{
				var length = 65000;
				if ((offset + length) > dataString.Length)
				{
					length = dataString.Length - offset;
				}

				var chunk = dataString.Substring(offset, length);
				sb.Append(Uri.EscapeDataString(chunk));
				offset += length;
			}

			return sb.ToString();
		}

		/// <summary>Performs a standard comparison for nullable types and if both are non-null, returns the result of the check provided.</summary>
		/// <typeparam name="T">The data type.</typeparam>
		/// <param name="x">The value to compare.</param>
		/// <param name="y">The value to compare to.</param>
		/// <param name="nonNull">The value to return if both x and y are non-null.</param>
		/// <returns>A standard <see cref="IComparable.CompareTo(object?)"/> value.</returns>
		public static int GenericComparer<T>(T? x, T? y, Func<T, T, int> nonNull)
		{
			ArgumentNullException.ThrowIfNull(nonNull);
			return (x, y) switch
			{
				(not null, not null) => nonNull(x, y),
				(not null, null) => 1,
				(null, not null) => -1,
				_ => 0
			};
		}

		/// <summary>Attempts to figure out the culture associated with the language code, falling back progressively through parent languages.</summary>
		/// <param name="languageCode">The language code to try.</param>
		/// <returns>The nearest CultureInfo possible to the given <paramref name="languageCode"/> or CurrentCulture if nothing is found.</returns>
		public static CultureInfo GetCulture(string? languageCode)
		{
			// Try to figure out wiki culture; otherwise revert to CurrentCulture.
			if (!string.IsNullOrWhiteSpace(languageCode))
			{
				do
				{
					try
					{
						return new CultureInfo(languageCode);
					}
					catch (CultureNotFoundException)
					{
						var lastDash = languageCode!.LastIndexOf('-');
						if (lastDash > -1)
						{
							languageCode = languageCode[..lastDash];
						}
					}
				}
				while (languageCode.Length > 0);
			}

			return CultureInfo.CurrentCulture;
		}

		/// <summary>Gets the requested type of hash for the byte data provided.</summary>
		/// <param name="data">The byte data.</param>
		/// <param name="hashType">The type of the hash.</param>
		/// <returns>The hash, represented as a <see cref="string"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="hashType"/> is neither Md5 nor Sha1.</exception>
		public static string GetHash(byte[] data, HashType hashType)
		{
			ArgumentNullException.ThrowIfNull(data);

#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
			using var hash = hashType switch
			{
				HashType.Md5 => MD5.Create(),
				HashType.Sha1 => SHA1.Create() as HashAlgorithm,
				_ => throw new ArgumentOutOfRangeException(nameof(hashType)),
			};
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms

			StringBuilder sb = new(40);
			foreach (var b in hash.ComputeHash(data))
			{
				sb.AppendFormat(CultureInfo.InvariantCulture, "{0:x2}", b);
			}

			return sb.ToString();
		}

		/// <summary>Gets the requested type of hash for the byte data provided.</summary>
		/// <param name="data">The byte data.</param>
		/// <param name="hashType">The type of the hash.</param>
		/// <returns>The hash, represented as a <see cref="string"/>.</returns>
		public static string GetHash(this string data, HashType hashType) => GetHash(Encoding.UTF8.GetBytes(data ?? string.Empty), hashType);

		/// <summary>Convenience method so that CurrentCulture and Invariant are all in the same class for both traditional and formattable strings, and are used the same way.</summary>
		/// <param name="formattable">A formattable string.</param>
		/// <returns>The formatted text.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="formattable"/> is null.</exception>
		// Copy of the same-named method from the FormattableString code so that all culture methods are in the same library.
		public static string Invariant(FormattableString formattable)
		{
			ArgumentNullException.ThrowIfNull(formattable);
			return formattable.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>Compares nullable types based only on the nullability of the values provided.</summary>
		/// <typeparam name="T">The data type.</typeparam>
		/// <param name="x">The value to compare.</param>
		/// <param name="y">The value to compare to.</param>
		/// <returns>A standard <see cref="IComparable.CompareTo(object?)"/> value based only on the nullability of the values provided.</returns>
		public static int? NullComparer<T>(T? x, T? y) => (x, y) switch
		{
			(not null, not null) => null,
			(not null, null) => 1,
			(null, not null) => -1,
			_ => 0
		};

		/// <summary>Creates an empty read-only dictionary of the specified type.</summary>
		/// <typeparam name="TKey">The key type.</typeparam>
		/// <typeparam name="TValue">The value type.</typeparam>
		/// <returns>An empty read-only dictionary.</returns>
		public static IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary<TKey, TValue>()
			where TKey : notnull => new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());

		/// <summary>Replaces invalid characters in a file name with underscores.</summary>
		/// <param name="filename">The file name to sanitize.</param>
		/// <returns>The sanitized file name.</returns>
		public static string SanitizeFilename(string filename)
		{
			ArgumentNullException.ThrowIfNull(filename);
			var invalidChars = Path.GetInvalidFileNameChars();
			var split = filename.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries);
			return string.Join('_', split).TrimEnd(TextArrays.Period);
		}

		#endregion
	}
}