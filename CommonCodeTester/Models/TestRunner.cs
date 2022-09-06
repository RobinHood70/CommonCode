namespace RobinHood70.CommonCodeTester.Models
{
	using System;
	using System.Diagnostics;
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

		/*
		public static void Initialize()
		{
		}
		*/

		public static void OpenFourCC() => new FourCCWindow().ShowDialog();

		public static void RunTests()
		{
			const string testText = "ON-trailer-Gates of Oblivion-Eveli.jpg";
			Debug.WriteLine(testText.UnCamelCase());

			var csv = new CsvFile() { EmptyFieldText = " " };
			csv.AddHeader("Test");
			csv.Add(string.Empty);
			csv.WriteFile(@"D:\Test.txt");
		}
	}
}