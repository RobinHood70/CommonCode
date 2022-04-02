namespace RobinHood70.CommonCode
{
	// TODO: Review all methods in this and the generic class to ensure consistent interace. Currently stripped down to only those methods that are actually in use.
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;
	using RobinHood70.CommonCode.Properties;

	#region Public Enumerations
	public enum ValidationType
	{
		Argument,
		Method,
		Property,
		Value,
	}
	#endregion

	/// <summary>This class contains some convenient one-off validation tests, as well as instantiators for the multi-test Validator{T} class.</summary>
	/// <remarks>While these tests can be used in sequence with one another, this will require repeating <c>nameof()</c> and possibly other values with each call. Validator{T} works around these issues, though may be slightly slower due to an increased need to cast the input being tested.</remarks>
	public static class Validator
	{
		#region Fields
		private static readonly IReadOnlyDictionary<ValidationType, string> ValueTypeTexts = new Dictionary<ValidationType, string>()
		{
			[ValidationType.Argument] = ValidatorMessages.Argument,
			[ValidationType.Property] = ValidatorMessages.Property,
			[ValidationType.Value] = ValidatorMessages.Value,
		};
		#endregion

		#region Public Methods
		public static T NotNull<T>([NotNull][ValidatedNotNull] this T? item, [CallerArgumentExpression("item")] string? name = null)
			where T : class? =>
			NotNull(item, ValidationType.Argument, name);

		public static T NotNull<T>([NotNull][ValidatedNotNull] this T? item, ValidationType validationType, [CallerArgumentExpression("item")] string? name = null)
			where T : class?
		{
			if (item is object)
			{
				return item;
			}

			name ??= Globals.Unknown;
			var message = MessageText(validationType, ValidatorMessages.NullMessage, name);
			Exception exception = validationType == ValidationType.Argument
				? new ArgumentNullException(name, message)
				: GetException(message);
			throw exception;
		}

		public static string NotNullOrEmpty([NotNull][ValidatedNotNull] this string? item, [CallerArgumentExpression("item")] string? name = null) =>
			NotNullOrEmpty(item, ValidationType.Argument, name);

		public static string NotNullOrEmpty([NotNull][ValidatedNotNull] this string? item, ValidationType validationType, [CallerArgumentExpression("item")] string? name = null) =>
			string.IsNullOrEmpty(item)
				? throw ValidatorException(validationType, ValidatorMessages.StringEmptyMessage, name)
				: item;

		public static T NotNullOrEmpty<T>([NotNull][ValidatedNotNull] this T? item, [CallerArgumentExpression("item")] string? name = null)
			where T : IEnumerable =>
			NotNullOrEmpty(item, ValidationType.Argument, name);

		public static T NotNullOrEmpty<T>([NotNull][ValidatedNotNull] this T? item, ValidationType validationType, [CallerArgumentExpression("item")] string? name = null)
			where T : IEnumerable =>
			(item != null && item.GetEnumerator().MoveNext())
				? item
				: throw ValidatorException(validationType, ValidatorMessages.StringEmptyMessage, name);

		public static string NotNullOrWhiteSpace([NotNull][ValidatedNotNull] this string? item, [CallerArgumentExpression("item")] string? name = null) =>
			NotNullOrWhiteSpace(item, ValidationType.Argument, name);

		public static string NotNullOrWhiteSpace([NotNull][ValidatedNotNull] this string? item, ValidationType validationType, [CallerArgumentExpression("item")] string? name = null) =>
			string.IsNullOrWhiteSpace(item)
				? throw ValidatorException(validationType, ValidatorMessages.NullOrWhitespaceMessage, name)
				: item;

		public static IEnumerable<string> NotNullOrWhiteSpace([NotNull][ValidatedNotNull] this IEnumerable<string>? item, [CallerArgumentExpression("item")] string? name = null) =>
			NotNullOrWhiteSpace(item, ValidationType.Argument, name);

		public static IEnumerable<string> NotNullOrWhiteSpace([NotNull][ValidatedNotNull] this IEnumerable<string>? item, ValidationType validationType, [CallerArgumentExpression("item")] string? name = null)
		{
			if (item is null)
			{
				throw ValidatorException(validationType, ValidatorMessages.CollectionEmptyMessage, name);
			}

			foreach (var s in item)
			{
				if (string.IsNullOrWhiteSpace(s))
				{
					throw ValidatorException(validationType, ValidatorMessages.CollectionEmptyMessage, name);
				}
			}

			return item;
		}

		public static T PropertyNotNull<T>([NotNull][ValidatedNotNull] this T? item, string className, [CallerArgumentExpression("item")] string? propertyNull = null)
			where T : class? =>
			NotNull(item, ValidationType.Property, className + '.' + propertyNull);

		public static T PropertyNotNull<T>([NotNull][ValidatedNotNull] this T? item, ValidationType validationType, string className, [CallerArgumentExpression("item")] string? propertyName = null)
			where T : class? => NotNull(item, validationType, className + '.' + propertyName);

		public static string PropertyNotNullOrWhiteSpace([NotNull][ValidatedNotNull] this string? item, string className, [CallerArgumentExpression("item")] string? propertyName = null) =>
			NotNullOrWhiteSpace(item, ValidationType.Argument, className + '.' + propertyName);

		public static IEnumerable<string> PropertyNotNullOrWhiteSpace([NotNull][ValidatedNotNull] this IEnumerable<string>? item, string className, [CallerArgumentExpression("item")] string? propertyName = null) =>
			NotNullOrWhiteSpace(item, ValidationType.Property, className + '.' + propertyName);

		public static void PropertyThrowNull([NotNull][ValidatedNotNull] this object? item, string className, [CallerArgumentExpression("item")] string? propertyName = null) =>
			ThrowNull(item, ValidationType.Property, className + '.' + propertyName);

		public static void ThrowNull([NotNull][ValidatedNotNull] this object? item, [CallerArgumentExpression("item")] string? name = null) =>
			ThrowNull(item, ValidationType.Argument, name);

		public static void ThrowNull([NotNull][ValidatedNotNull] this object? item, ValidationType validationType, [CallerArgumentExpression("item")] string? name = null)
		{
			if (item is null)
			{
				throw new InvalidOperationException(MessageText(validationType, ValidatorMessages.NullMessage, name ?? Globals.Unknown));
			}
		}

		public static Validator<T> Validate<T>(this T? item, [CallerArgumentExpression("item")] string? name = null)
			where T : class? =>
			new(item, ValidationType.Value, name ?? Globals.Unknown);
		/*
				public static Validator<T> Validate<T>(this T? item, string name, string property)
					where T : class? =>
						new(item, ValidationType.Property, name + '.' + property);

				public static Validator<T> Validate<T>(this T? item, params string[] nameParts)
					where T : class? =>
						new(item, nameParts.Length > 1 ? ValidationType.Property : ValidationType.Value, Join(nameParts));

				public static Validator<T> Validate<T>(this T? item, ValidationType validationType, params string[] nameParts)
					where T : class? =>
						new(item, validationType, Join(nameParts));
		*/
		#endregion

		#region Internal Methods
		internal static InvalidOperationException GetException(string message) => new(message);

		internal static InvalidOperationException ValidatorException(ValidationType validationType, string message, string? name) => GetException(MessageText(validationType, message, name ?? Globals.Unknown));
		#endregion

		#region Private Methods
		private static string MessageText(ValidationType valueType, string message, string name) => Globals.CurrentCulture(ValidatorMessages.ErrorFormat, message, NameText(valueType, name));

		private static string NameText(ValidationType valueType, string name) =>
			Globals.CurrentCulture(
				ValidatorMessages.InfoFormat,
				ValueTypeTexts.TryGetValue(valueType, out var retval) ? retval : throw new KeyNotFoundException(),
				name);
		#endregion
	}
}
