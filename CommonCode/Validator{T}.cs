namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;
	using RobinHood70.CommonCode.Properties;
	using static RobinHood70.CommonCode.Globals;

	public class Validator<T>
		where T : class?
	{
		#region Static fields
		private static readonly IReadOnlyDictionary<ValidationType, string> TypeText = new Dictionary<ValidationType, string>()
		{
			[ValidationType.Argument] = ValidatorMessages.ItemTypeArgument,
			[ValidationType.Property] = ValidatorMessages.ItemTypeProperty,
			[ValidationType.Unknown] = ValidatorMessages.ItemTypeUnknown,
			[ValidationType.Value] = ValidatorMessages.ItemTypeValue,
		};
		#endregion

		#region Fields
		private readonly string caller;
		private readonly ValidationType validationType;
		private readonly string name;
		#endregion

		#region Constructors
		public Validator(T? value, string name, [CallerMemberName] string caller = Unknown)
			: this(value, ValidationType.Unknown, name, caller)
		{
		}

		public Validator(T? value, ValidationType validationType, string name, [CallerMemberName] string caller = Unknown)
		{
			this.Item = value;
			this.validationType = validationType;
			this.name = name;
			this.caller = caller;
		}
		#endregion

		#region Public Properties

		public T? Item { get; }

		// Always true because if it didn't validate, it threw an error. Useful for cases like .Validate ? TrueAction : FalseAction.
		public bool Validated => true;

		public T Value => this.Item ?? throw this.ValidatorException;
		#endregion

		#region Private Properties
		private InvalidOperationException ValidatorException => new(CurrentCulture(TypeText[this.validationType], this.name, this.caller));
		#endregion

		#region Public Methods

		public Validator<TWanted> CastTo<TWanted>()
			where TWanted : class? =>
			this.Value is TWanted output
				? new Validator<TWanted>(output, this.name, this.caller)
				: throw InvalidParameterType(this.name, typeof(TWanted).FullName, (this.Value?.GetType() ?? typeof(T)).FullName ?? Unknown, this.caller);

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Rule bug")]
		public Validator<TWanted> CastToSameType<TWanted>(TWanted _)
			where TWanted : class? =>
			this.Value is TWanted output
				? new Validator<TWanted>(output, this.name, this.caller)
				: throw InvalidParameterType(this.name, typeof(TWanted).FullName, (this.Value?.GetType() ?? typeof(T)).FullName ?? Unknown, this.caller);

		public Validator<T> Is<TWanted>()
			where TWanted : class? =>
			this.Value is TWanted
				? this
				: throw InvalidParameterType(this.name, typeof(TWanted).FullName, (this.Value?.GetType() ?? typeof(T)).FullName ?? Unknown, this.caller);

		public Validator<T> NotEmpty() =>
			(this.Item is ICollection collection && collection.Count > 0) ||
			(this.Item is IEnumerable ienum && ienum.GetEnumerator().MoveNext()) ||
			(this.Item is string s && !s.IsEmpty())
				? this
				: throw this.ValidatorException;

		public Validator<T> NotIsNullOrEmpty() =>
			this.Item is string s && !string.IsNullOrWhiteSpace(s)
				? this
				: throw this.ValidatorException;

		public Validator<T> NotIsNullOrWhiteSpace() =>
			this.Item is string s && !string.IsNullOrWhiteSpace(s)
				? this
				: throw this.ValidatorException;

		/// <summary>Gets a Validator if the provided value is not null.</summary>
		/// <typeparam name="T">The type of the value to check.</typeparam>
		/// <param name="instance">The value that may be null.</param>
		/// <param name="valueName">The name of the value in the original method.</param>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="instance" /> is null.</exception>
		[MemberNotNull(nameof(Item))]
		public Validator<T> NotNull() => this.Item is object
			? this
			: this.validationType == ValidationType.Argument
				? throw new ArgumentNullException(this.name)
				: throw this.ValidatorException;
		#endregion

		#region Private Methods

		/// <summary>The error thrown when a parameter could not be cast to the expected type.</summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="wantedType">The type that was wanted.</param>
		/// <param name="actualType">The actual type of the parameter passed.</param>
		/// <param name="caller">The caller.</param>
		/// <returns>An <see cref="InvalidCastException"/>.</returns>
		private static InvalidCastException InvalidParameterType(string parameterName, string? wantedType, string? actualType, [CallerMemberName] string caller = "Unknown") => new(CurrentCulture(Resources.ParameterInvalidCast, parameterName, caller, actualType ?? Unknown, wantedType ?? Unknown));
		#endregion
	}
}
