namespace RobinHood70.CommonCodeTester.Tests
{
	using System.Text;
	using RobinHood70.CommonCode;

	internal class IStringBuilderTest1 : IStringBuilder
	{
		public StringBuilder BuildString(StringBuilder sb) => sb.Append("Hello world!");

		public override string? ToString() => ((IStringBuilder)this).BuildString();
	}
}
