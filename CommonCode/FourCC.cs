namespace RobinHood70.CommonCode
{
	using System;
	using System.Globalization;
	using System.Text;

	public static class FourCC
	{
		public static string HexString(string text) => "0x" + Value(text).ToString("X8", CultureInfo.InvariantCulture);

		public static uint Value(string text) => Value(Encoding.ASCII.GetBytes(text));

		public static uint Value(byte[] bytes) => BitConverter.ToUInt32(bytes, 0);
	}
}
