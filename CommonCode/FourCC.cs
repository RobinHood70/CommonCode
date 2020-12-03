namespace RobinHood70.CommonCode
{
	using System;
	using System.Globalization;
	using System.Text;

	public static class FourCC
	{
		#region Public Static Properties
		public static Encoding DefaultEncoding { get; set; } = Encoding.ASCII;

		public static string HexString(string text) => "0x" + Value(text).ToString("X8", CultureInfo.InvariantCulture);

		public static string ToString(uint value) => DefaultEncoding.GetString(BitConverter.GetBytes(value));

		public static uint Value(string text) => Value(DefaultEncoding.GetBytes(text));

		public static uint Value(byte[] bytes) => BitConverter.ToUInt32(bytes, 0);
		#endregion
	}
}
