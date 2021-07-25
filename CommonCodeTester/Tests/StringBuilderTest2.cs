namespace RobinHood70.CommonCodeTester.Tests
{
	using System.Text;
	using RobinHood70.CommonCode;

	public class StringBuilderTest2 : IStringBuilder
	{
		public StringBuilder BuildString(StringBuilder sb) => sb.Append("Built");

		public override string ToString() => "ToString'd";
	}
}
