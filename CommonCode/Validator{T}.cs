#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using RobinHood70.CommonCode.Properties;

	[Obsolete("Replace with standard methods where possible.", false)]
	public class Validator<T>(T? item, ValidationType validationType, string name)
		where T : class?
	{
		#region Public Properties
		public T? NullableValue { get; } = item;

		public T Value => this.NullableValue ?? throw Validator.GetException(ValidatorMessages.NullMessage);
		#endregion

		#region Public Methods
		public Validator<TWanted> CastTo<TWanted>()
			where TWanted : class? =>
			this.Value is TWanted output
				? new(output, validationType, name)
				: throw Validator.GetException(ValidatorMessages.InvalidCast);

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Rule bug")]
		public Validator<TWanted> CastToSameType<TWanted>(TWanted _)
			where TWanted : class? => this.CastTo<TWanted>();

		public Validator<T> Is<TWanted>()
			where TWanted : class? =>
			this.Value is TWanted
				? this
				: throw Validator.GetException(ValidatorMessages.InvalidCast);

		public Validator<T> NotNullOrEmpty()
		{
			var retval = this.NullableValue switch
			{
				null => null,
				ICollection collection when collection.Count > 0 => this,
				IEnumerable ienum when ienum.GetEnumerator().MoveNext() => this,
				string s when s.Length > 0 => this,
				_ => null
			};

			return retval ?? throw Validator.GetException(ValidatorMessages.NullOrEmptyMessage);
		}

		/// <summary>Gets a Validator if the provided value is not null.</summary>
		/// <exception cref="InvalidOperationException">Thrown if the value is null.</exception>
		// [MemberNotNull(nameof(NullableValue))]
		public Validator<T> NotNull() => this.NullableValue is object
			? this
			: validationType == ValidationType.Argument
				? throw new ArgumentNullException(name)
				: throw Validator.GetException(ValidatorMessages.NullMessage);

		public Validator<T> NotNullOrWhiteSpace()
		{
			var retval = this.NullableValue switch
			{
				null => null,
				string s when s.Trim().Length == 0 => null,
				IEnumerable<string> ienum => CheckStrings(ienum),
				_ => this
			};

			return retval ?? throw Validator.ValidatorException(validationType, ValidatorMessages.NullOrWhitespaceMessage, name);

			Validator<T>? CheckStrings(IEnumerable<string> ienum)
			{
				foreach (var s in ienum)
				{
					if (string.IsNullOrWhiteSpace(s))
					{
						return null;
					}
				}

				return this;
			}
		}
		#endregion
	}
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member