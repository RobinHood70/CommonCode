namespace RobinHood70.CommonCodeTester.Models
{
	using System;
	using System.IO;
	using System.Text;
	using System.Windows;
	using RobinHood70.CommonCode;
	using RobinHood70.CommonCodeTester.Views;

	public static class TestRunner
	{
		[Flags]
		private enum TestAnimals
		{
			None = 0,
			Cat = 1,
			Dog = 1 << 1,
			Fish = 1 << 2,
			Mammals = Cat | Dog,
			All = Cat | Dog | Fish,
		}

		public static void Initialize()
		{
		}

		public static void OpenFourCC() => new FourCCWindow().ShowDialog();

		public static void RunTest()
		{
			var testText = "[Hello]\r\nTest=value";
			using var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(testText));
			using var stream = new StreamReader(memoryStream);
			var iniFile = new IniFile(stream);
			MessageBox.Show(iniFile[0].Name);
		}
	}
}
