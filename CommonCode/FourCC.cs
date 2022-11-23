namespace RobinHood70.CommonCode
{
	using System;
	using System.Globalization;
	using System.Text;
	using RobinHood70.CommonCode.Properties;

	/// <summary>This static class provides several methods that are useful when working with <see href="https://en.wikipedia.org/wiki/FourCC"> four-character code</see> data.</summary>
	public static class FourCC
	{
		#region Public Static Properties

		/// <summary>Gets or sets the encoding to be used. Defaults to ASCII.</summary>
		public static Encoding DefaultEncoding { get; set; } = Encoding.ASCII;

		/// <summary>Convert a FourCC string to its hex equivalent.</summary>
		/// <param name="text">The text to convert.</param>
		/// <returns>The hex string of the specified FourCC.</returns>
		public static string HexString(string text) => "0x" + Value(text).ToString("X8", CultureInfo.InvariantCulture);

		/// <summary>Convert a signed integer to a FourCC string.</summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>The FourCC represented by the value.</returns>
		public static string ToString(int value) => DefaultEncoding.GetString(BitConverter.GetBytes(value));

		/// <summary>Convert an unsigned integer to a FourCC string.</summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>The FourCC represented by the value.</returns>
		[CLSCompliant(false)]
		public static string ToString(uint value) => DefaultEncoding.GetString(BitConverter.GetBytes(value));

		/// <summary>Convert a FourCC to an unsigned integer.</summary>
		/// <param name="text">The bytes to convert.</param>
		/// <returns>The integer value of the bytes.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the decoded text evaluates to something other than four bytes.</exception>
		[CLSCompliant(false)]
		public static uint Value(string text)
		{
			var bytes = DefaultEncoding.GetBytes(text);
			return bytes.Length != 4
				? throw new InvalidOperationException(Resources.InvalidFourCC)
				: BitConverter.ToUInt32(bytes);
		}

		/// <summary>Convert four bytes to an unsigned integer.</summary>
		/// <param name="bytes">The bytes to convert.</param>
		/// <returns>The unsigned integer value of the bytes.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="bytes"/> is not four bytes in length.</exception>
		[CLSCompliant(false)]
		public static uint Value(byte[] bytes) => bytes.Length != 4
			? throw new ArgumentException(Resources.InvalidFourCC, nameof(bytes))
			: BitConverter.ToUInt32(bytes, 0);

		/// <summary>Convert a string to a signed integer.</summary>
		/// <param name="text">The bytes to convert.</param>
		/// <returns>The signed integer value of the bytes.</returns>
		public static int ValueInt(string text)
		{
			var bytes = DefaultEncoding.GetBytes(text);
			return bytes.Length != 4
				? throw new InvalidOperationException(Resources.InvalidFourCC)
				: BitConverter.ToInt32(bytes, 0);
		}

		/// <summary>Convert four bytes to a signed integer.</summary>
		/// <param name="bytes">The bytes to convert.</param>
		/// <returns>The signed integer value of the bytes.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="bytes"/> is not four bytes in length.</exception>
		public static int ValueInt(byte[] bytes) => bytes.Length != 4
			? throw new ArgumentException(Resources.InvalidFourCC, nameof(bytes))
			: BitConverter.ToInt32(bytes, 0);
		#endregion
	}
}
