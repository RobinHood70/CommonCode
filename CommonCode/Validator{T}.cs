namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using RobinHood70.CommonCode.Properties;

	public class Validator<T>
		where T : class?
	{
		#region Fields
		private readonly ValidationType validationType;
		private readonly string name;
		#endregion

		#region Constructors
		public Validator(T? item, string name)
			: this(item, ValidationType.Other, name)
		{
		}

		public Validator(T? item, ValidationType validationType, string name)
		{
			this.NullableValue = item;
			this.validationType = validationType;
			this.name = name;
		}
		#endregion

		#region Public Properties

		public T? NullableValue { get; }

		// Always true because if it didn't validate, it threw an error. Useful for cases like .Validate ? TrueAction : FalseAction.
		public bool Validated => true;

		public T Value => this.NullableValue ?? throw Validator.GetException(ValidatorMessages.NullMessage);
		#endregion

		#region Public Methods

		public Validator<TWanted> CastTo<TWanted>()
			where TWanted : class? =>
			this.Value is TWanted output
				? new Validator<TWanted>(output, this.name)
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
		/// <typeparam name="T">The type of the value to check.</typeparam>
		/// <param name="instance">The value that may be null.</param>
		/// <param name="valueName">The name of the value in the original method.</param>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="instance" /> is null.</exception>
		// [MemberNotNull(nameof(NullableValue))]
		public Validator<T> NotNull() => this.NullableValue is object
			? this
			: this.validationType == ValidationType.Argument
				? throw new ArgumentNullException(this.name)
				: throw Validator.GetException(ValidatorMessages.NullMessage);

		public Validator<T> NotNullOrWhiteSpace()
		{
			var retval = this.NullableValue switch
			{
				null => null,
				string s when string.IsNullOrWhiteSpace(s) => null,
				IEnumerable<string> ienum => CheckStrings(ienum),
				_ => this
			};

			return retval ?? throw Validator.ValidatorException(this.validationType, ValidatorMessages.NullOrWhitespaceMessage, this.name);

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
