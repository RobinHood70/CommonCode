namespace RobinHood70.CommonCode;

using System;
using System.Collections.Generic;

/// <summary>Represents a range of values.</summary>
/// <typeparam name="T">The data type of the range.</typeparam>
/// <remarks>Initializes a new instance of the <see cref="ValueRange{T}"/> class.</remarks>
/// <param name="min">The minimumu value in the range.</param>
/// <param name="max">The maximum value in the range.</param>
public sealed class ValueRange<T>(T min, T max) : IEquatable<ValueRange<T>>
	where T : IComparable<T>
{
	#region Public Properties

	/// <summary>Gets or sets the maximum value in the range.</summary>
	public T Max { get; set; } = max;

	/// <summary>Gets or sets the minimum value in the range.</summary>
	public T Min { get; set; } = min;
	#endregion

	#region Public Operators

	/// <summary>Checks if two value ranges are the same.</summary>
	/// <param name="left">The first value range to check.</param>
	/// <param name="right">The second value range to check.</param>
	/// <returns><see langword="true"/> if the value ranges are the same; otherwise, <see langword="false"/>.</returns>
	public static bool operator ==(ValueRange<T> left, ValueRange<T> right) => left?.Equals(right) ?? right is null;

	/// <summary>Checks if two value ranges are different.</summary>
	/// <param name="left">The first value range to check.</param>
	/// <param name="right">The second value range to check.</param>
	/// <returns><see langword="true"/> if the value ranges are different; otherwise, <see langword="false"/>.</returns>
	public static bool operator !=(ValueRange<T> left, ValueRange<T> right) => !(left == right);
	#endregion

	#region Public Methods

	/// <inheritdoc/>
	public bool Equals(ValueRange<T>? other) =>
		other is not null &&
		EqualityComparer<T>.Default.Equals(this.Max, other.Max) &&
		EqualityComparer<T>.Default.Equals(this.Min, other.Min);

	/// <summary>Expands the minimum or maximum value of the range to include a new value.</summary>
	/// <param name="value">The value to include in the range.</param>
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

	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is ValueRange<T> range && this.Equals(range);

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(this.Max, this.Min);

	/// <inheritdoc/>
	public override string ToString() => (this.Min.Equals(this.Max) ? this.Min.ToString() : string.Concat(this.Min.ToString(), "-", this.Max.ToString())) ?? "Unknown";
	#endregion
}