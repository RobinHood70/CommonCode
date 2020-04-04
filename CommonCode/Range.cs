namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections.Generic;

	public class Range<T> : IEquatable<Range<T>>
		where T : notnull
	{
		#region Constructor
		public Range(T min, T max)
		{
			this.Max = max;
			this.Min = min;
		}
		#endregion

		#region Public Properties
		public T Max { get; set; }

		public T Min { get; set; }
		#endregion

		#region Public Operators
		public static bool operator ==(Range<T> left, Range<T> right) => left?.Equals(right) ?? right is null;

		public static bool operator !=(Range<T> left, Range<T> right) => !(left == right);
		#endregion

		#region Public Methods
		public bool Equals(Range<T>? other) => other is null ? false : EqualityComparer<T>.Default.Equals(this.Max, other.Max) && EqualityComparer<T>.Default.Equals(this.Min, other.Min);
		#endregion

		#region Public Override Methods
		public override bool Equals(object? obj) => obj is Range<T> range && this.Equals(range);

		public override int GetHashCode() => HashCode.Combine(this.Max, this.Min);

		public override string ToString() => (this.Min.Equals(this.Max) ? this.Min.ToString() : string.Concat(this.Min.ToString(), "-", this.Max.ToString())) ?? "Unknown";
		#endregion
	}
}