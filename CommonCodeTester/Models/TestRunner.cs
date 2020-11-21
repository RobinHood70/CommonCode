namespace RobinHood70.CommonCodeTester.Models
{
	using System;
	using System.Collections.Generic;
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
			var names = new List<string>
			{
				"Lore:Crafting Motif 56, Apostle",
				"Lore:Crafting Motif 48, Ashlander",
				"Lore:Crafting Motif 54, Bloodforge",
				"Lore:Crafting Motif 69, Dead-Water",
				"Lore:Crafting Motif 55, Dreadhorn",
				"Lore:Crafting Motif 57, Ebonshadow",
				"Lore:Crafting Motif 70, Elder Argonian",
				"Lore:Crafting Motif 58, Fang Lair",
				"Lore:Crafting Motif 43, Harlequin Style",
				"Lore:Crafting Motif 51, Hlaalu",
				"Lore:Crafting Motif 65, Huntsman",
				"Lore:Crafting Motif 45, Mazzatun",
			};

			names.Sort(NaturalSort.Instance);
		}
	}
}