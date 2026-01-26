namespace RobinHood70.CommonCodeTester.Models;

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
		var lotsOfSplits = new string('@', 1000);
		var sw = new Stopwatch();
		sw.Start();
		var at = default(SpanArrays).At;
		for (var i = 0; i < 10000000; i++)
		{
			_ = lotsOfSplits.Split(at);
		}

		sw.Stop();
		Debug.WriteLine($"Splitting string with default(SpanArrays).At took {sw.ElapsedMilliseconds} ms");
		sw.Restart();
		var at2 = TextArrays.At;
		for (var i = 0; i < 10000000; i++)
		{
			_ = lotsOfSplits.Split(at2);
		}

		sw.Stop();
		Debug.WriteLine($"Splitting string with TextArrays.At took {sw.ElapsedMilliseconds} ms");
	}
}