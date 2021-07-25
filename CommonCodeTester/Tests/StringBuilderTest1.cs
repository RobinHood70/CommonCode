namespace RobinHood70.CommonCodeTester.Tests
{
	using System.Text;
	using RobinHood70.CommonCode;

	internal sealed class StringBuilderTest1 : IStringBuilder
	{
		public StringBuilder BuildString(StringBuilder sb) => sb.Append("Hello world!");

		public override string? ToString() => ((IStringBuilder)this).BuildString();
	}
}
