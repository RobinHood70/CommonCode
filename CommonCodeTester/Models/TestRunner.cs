namespace RobinHood70.CommonCodeTester.Models
{
	using System.Diagnostics;
	using RobinHood70.CommonCode;
	using RobinHood70.CommonCodeTester.Views;

	public static class TestRunner
	{
		public static void Initialize()
		{
		}

		public static void OpenFourCC() => new FourCCWindow().ShowDialog();

		public static void RunTest()
		{
			var list = new[] { "ASND", "AVFX", "BSND", "BVFX", "CSND", "CVFX", "DELE", "DESC", "HSND", "HVFX", "INDX", "ITEX", "MEDT", "PTEX" };
			foreach (var typeId in list)
			{
				Debug.WriteLine($"public const uint {typeId} = {FourCC.HexString(typeId)};");
			}
		}
	}
}
