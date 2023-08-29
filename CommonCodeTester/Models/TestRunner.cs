namespace RobinHood70.CommonCodeTester.Models
{
	using System;
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
			var csv = new CsvFile();
			csv.Load(@"D:\Data\HoodBot\Starfield\Quests.csv", true);
		}
	}
}