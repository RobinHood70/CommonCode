namespace RobinHood70.CommonCodeTester.Models
{
	using System;
	using System.Diagnostics;
	using System.Text;
	using RobinHood70.CommonCode;
	using RobinHood70.CommonCodeTester.Tests;
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

		public static void RunTests()
		{
			IStringBuilder sbClass = new IStringBuilderTest1();
			Debug.WriteLine("== Class 1 ==");
			Debug.WriteLine(sbClass);
			Debug.WriteLine(sbClass.ToString());
			Debug.WriteLine(sbClass.BuildString(new StringBuilder()));

			sbClass = new IStringBuilderTest2();
			Debug.WriteLine("\n== Class 2 ==");
			Debug.WriteLine(sbClass);
			Debug.WriteLine(sbClass.ToString());
			Debug.WriteLine(sbClass.BuildString(new StringBuilder()));

			var testText = "ON-trailer-Gates of Oblivion-Eveli.jpg";
			Debug.WriteLine(testText.UnCamelCase());
		}
	}
}