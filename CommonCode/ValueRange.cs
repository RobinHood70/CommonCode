namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections.Generic;

	public class ValueRange<T> : IEquatable<ValueRange<T>>
		where T : IComparable<T>
	{
		#region Constructor
		public ValueRange(T min, T max)
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
		public static bool operator ==(ValueRange<T> left, ValueRange<T> right) => left?.Equals(right) ?? right is null;

		public static bool operator !=(ValueRange<T> left, ValueRange<T> right) => !(left == right);
		#endregion

		#region Public Methods
		public bool Equals(ValueRange<T>? other) =>
			!(other is null) &&
			EqualityComparer<T>.Default.Equals(this.Max, other.Max) &&
			EqualityComparer<T>.Default.Equals(this.Min, other.Min);

		public void ExpandToInclude(T value)
		{
			if (value.CompareTo(this.Min) < 0)
			{
				this.Min = value;
			}

			if (value.CompareTo(this.Max) > 0)
			{
				this.Max = value;
			}
		}
		#endregion

		#region Public Override Methods
		public override bool Equals(object? obj) => obj is ValueRange<T> range && this.Equals(range);

		public override int GetHashCode() => HashCode.Combine(this.Max, this.Min);

		public override string ToString() => (this.Min.Equals(this.Max) ? this.Min.ToString() : string.Concat(this.Min.ToString(), "-", this.Max.ToString())) ?? "Unknown";
		#endregion
	}
}