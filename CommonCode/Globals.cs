namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Security.Cryptography;
	using System.Text;
	using RobinHood70.CommonCode.Properties;

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

		/// <summary>Convenience method so that CurrentCulture and Invariant are all in the same class for both traditional and formattable strings, and are used the same way.</summary>
		/// <param name="text">The text to format.</param>
		/// <param name="values">The values of any parameters in the <paramref name="text" /> parameter.</param>
		/// <returns>The formatted text.</returns>
		public static string CurrentCulture(string text, params object?[] values) => string.Format(CultureInfo.CurrentCulture, text, values);

		/// <summary>Works around Uri.EscapeDataString's length limits.</summary>
		/// <param name="dataString">The string to escape.</param>
		/// <returns>The escaped string.</returns>
		public static string EscapeDataString(string dataString)
		{
			if (string.IsNullOrEmpty(dataString))
			{
				return dataString;
			}

			var sb = new StringBuilder(dataString.Length * 2);
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
							languageCode = languageCode.Substring(0, lastDash);
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
			data.ThrowNull(nameof(data));
			using var hash = hashType switch
			{
				HashType.Md5 => MD5.Create(),
				HashType.Sha1 => SHA1.Create() as HashAlgorithm,
				_ => throw new ArgumentOutOfRangeException(nameof(hashType)),
			};

			var sb = new StringBuilder(40);
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

		/// <summary>Initializes .NET Core's encoding so that it can load code-page based encodings.</summary>
		[ModuleInitializer]
		public static void Initialize() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

		/// <summary>Convenience method so that CurrentCulture and Invariant are all in the same class for both traditional and formattable strings, and are used the same way.</summary>
		/// <param name="formattable">A formattable string.</param>
		/// <returns>The formatted text.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="formattable"/> is null.</exception>
		// Copy of the same-named method from the FormattableString code so that all culture methods are in the same library.
		public static string Invariant(FormattableString formattable) => (formattable ?? throw ArgumentNull(nameof(formattable))).ToString(CultureInfo.InvariantCulture);

		public static int? NullComparer<T>(T? x, T? y) => (x, y) switch
		{
			(object, object) => null,
			(object, null) => 1,
			(null, object) => -1,
			_ => 0
		};

		/// <summary>The error thrown when a property of an object was unexpectedly null.</summary>
		/// <param name="objectName">The name of the object in the original method.</param>
		/// <param name="propertyName">The property of the object which was found to be null.</param>
		/// <returns>An <see cref="InvalidOperationException"/> for the specified object and property.</returns>
		public static InvalidOperationException PropertyNull(string objectName, string propertyName) => new(CurrentCulture(Resources.PropertyNull, objectName, propertyName));

		/// <summary>Creates an empty read-only dictionary of the specified type.</summary>
		/// <typeparam name="TKey">The key type.</typeparam>
		/// <typeparam name="TValue">The value type.</typeparam>
		/// <returns>An empty read-only dictionary.</returns>
		public static IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary<TKey, TValue>()
			where TKey : notnull => new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());

		public static string SanitizeFilename(string filename)
		{
			var invalidChars = Path.GetInvalidFileNameChars();
			var split = filename.NotNull(nameof(filename)).Split(invalidChars, StringSplitOptions.RemoveEmptyEntries);
			return string.Join('_', split).TrimEnd(TextArrays.Period);
		}

		public static void ThrowCollectionEmpty<T>(IEnumerable<T> collection, string paramName)
		{
			if (collection.IsEmpty())
			{
				throw new ArgumentException(CurrentCulture(Resources.CollectionEmpty, paramName), nameof(paramName));
			}
		}

		/// <summary>Throws an exception if the input value is null.</summary>
		/// <param name="nullable">The value that may be null.</param>
		/// <param name="name">The name of the parameter in the original method.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="nullable" /> is null.</exception>
		public static void ThrowNull([ValidatedNotNull][NotNull] object? nullable, string name)
		{
			if (nullable is null)
			{
				throw ArgumentNull(name);
			}
		}

		/// <summary>Throws an exception if the property value is null.</summary>
		/// <param name="nullable">The value that may be null.</param>
		/// <param name="objectName">The name of the object in the original method.</param>
		/// <param name="propertyName">The property of the object which was found to be null.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="nullable" /> is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown when <paramref name="nullable"/> is null.</exception>
		public static void ThrowNull([ValidatedNotNull][NotNull] object? nullable, string objectName, string propertyName)
		{
			if (nullable is null)
			{
				throw PropertyNull(objectName, propertyName);
			}
		}

		public static void ThrowNullOrWhiteSpace(string text, string paramName)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new ArgumentException(CurrentCulture(Resources.StringInvalid, paramName), nameof(text));
			}
		}

		public static void ThrowNullOrWhiteSpace(IEnumerable<string> collection, string paramName)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(paramName);
			}

			foreach (var item in collection)
			{
				if (string.IsNullOrWhiteSpace(item))
				{
					throw new ArgumentException(CurrentCulture(Resources.CollectionInvalid, paramName), nameof(collection));
				}
			}
		}
		#endregion
	}
}