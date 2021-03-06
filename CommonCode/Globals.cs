﻿namespace RobinHood70.CommonCode
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
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
	public delegate void StrongEventHandler<TSender, TEventArgs>(TSender sender, TEventArgs eventArgs);
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
		#region Public Properties

		/// <summary>Gets a <see cref="TimeSpan"/> that is a good general time to abort Regex operations after.</summary>
		public static TimeSpan DefaultRegexTimeout { get; } = TimeSpan.FromSeconds(5);
		#endregion

		#region Public Methods

		/// <summary>Throws an exception if the input value is null.</summary>
		/// <param name="name">The name of the parameter in the original method.</param>
		/// <exception cref="ArgumentNullException">Always thrown.</exception>
		/// <returns>An <see cref="ArgumentNullException"/> for the specified parameter name.</returns>
		public static ArgumentNullException ArgumentNull(string name) => new ArgumentNullException(name);

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
		public static string GetHash(byte[] data, HashType hashType)
		{
			// At one point, this was a try/finally block because mono wasn't quite implementing IDisposable properly. This doesn't appear to be the case anymore, so doing it the traditional method. It looks like a using block would probably have worked in any event, since that would presumably coerce hash to IDisposable.
			// See https://xamarin.github.io/bugzilla-archives/33/3375/bug.html
			using var hash = hashType switch
			{
				HashType.Md5 => MD5.Create(),
				HashType.Sha1 => SHA1.Create() as HashAlgorithm,
				_ => throw new InvalidOperationException(),
			};

			var sb = new StringBuilder(40);
			var hashBytes = hash.ComputeHash(data);
			foreach (var b in hashBytes)
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
		// Copy of the same-named method from the FormattableString code so that all culture methods are in the same library.
		public static string Invariant(FormattableString formattable) => (formattable ?? throw ArgumentNull(nameof(formattable))).ToString(CultureInfo.InvariantCulture);

		/// <summary>The error thrown when a property of an object was unexpectedly null.</summary>
		/// <param name="objectName">The name of the object in the original method.</param>
		/// <param name="propertyName">The property of the object which was found to be null.</param>
		/// <returns>An <see cref="InvalidOperationException"/> for the specified object and property.</returns>
		public static InvalidOperationException PropertyNull(string objectName, string propertyName) => new InvalidOperationException(CurrentCulture(Resources.PropertyNull, objectName, propertyName));

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
		public static void ThrowNull([ValidatedNotNull][NotNull] object? nullable, string objectName, string propertyName)
		{
			if (nullable is null)
			{
				throw PropertyNull(objectName, propertyName);
			}
		}
		#endregion
	}
}