namespace RobinHood70.CommonCode
{
	using System.Text;

	public interface IStringBuilder
	{
		StringBuilder BuildString(StringBuilder sb);

		string? BuildString() => this.BuildString(new StringBuilder()).ToString();
	}
}
