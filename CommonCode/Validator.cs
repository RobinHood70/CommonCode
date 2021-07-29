namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using RobinHood70.CommonCode.Properties;

	#region Public Enumerations
	public enum ValidationType
	{
		Argument,
		Property,
		Other,
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
			[ValidationType.Other] = ValidatorMessages.Value,
		};
		#endregion

		#region Public Methods
		public static string Join(params string[] nameParts) =>
			string.Join('.', nameParts ?? throw new ArgumentNullException(nameof(nameParts)));

		public static T NotNull<T>(
			[NotNull][ValidatedNotNull] this T? item,
			string name)
			where T : class? =>
				NotNull(item, ValidationType.Argument, name);

		public static T NotNull<T>(
			[NotNull][ValidatedNotNull] this T? item,
			ValidationType validationType,
			string name)
			where T : class?
		{
			if (item is object)
			{
				return item;
			}

			var message = MessageText(validationType, ValidatorMessages.NullMessage, name);
			throw validationType == ValidationType.Argument
				? new ArgumentNullException(name, message)
				: GetException(message);
		}

		public static string NotNullOrEmpty(
			[NotNull][ValidatedNotNull] this string? item,
			string name) =>
				NotNullOrEmpty(item, ValidationType.Argument, name);

		public static string NotNullOrEmpty(
			[NotNull][ValidatedNotNull] this string? item,
			ValidationType validationType,
			string name) => string.IsNullOrEmpty(item)
				? throw ValidatorException(validationType, ValidatorMessages.StringEmptyMessage, name)
				: item;

		public static T NotNullOrEmpty<T>(
			[NotNull][ValidatedNotNull] this T? item,
			string name)
			where T : IEnumerable =>
				NotNullOrEmpty(item, ValidationType.Argument, name);

		public static T NotNullOrEmpty<T>(
			[NotNull][ValidatedNotNull] this T? item,
			ValidationType validationType,
			string name)
			where T : IEnumerable => (item != null && item.GetEnumerator().MoveNext())
			? item
			: throw ValidatorException(validationType, ValidatorMessages.StringEmptyMessage, name);

		public static string NotNullOrWhiteSpace(
			[NotNull][ValidatedNotNull] this string? item,
			string name) =>
				NotNullOrWhiteSpace(item, ValidationType.Argument, name);

		public static string NotNullOrWhiteSpace(
			[NotNull][ValidatedNotNull] this string? item,
			ValidationType validationType,
			string name) => string.IsNullOrWhiteSpace(item)
				? throw ValidatorException(validationType, ValidatorMessages.StringEmptyMessage, name)
				: item;

		public static IEnumerable<string> NotNullOrWhiteSpace(
			[NotNull][ValidatedNotNull] this IEnumerable<string>? item,
			string name) =>
				NotNullOrWhiteSpace(item, ValidationType.Argument, name);

		public static IEnumerable<string> NotNullOrWhiteSpace(
			[NotNull][ValidatedNotNull] this IEnumerable<string>? item,
			ValidationType validationType,
			string name)
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

		public static void ThrowNull(
			[NotNull][ValidatedNotNull] this object? item,
			string name) =>
				ThrowNull(item, ValidationType.Argument, name);

		public static void ThrowNull(
			[NotNull][ValidatedNotNull] this object? item,
			ValidationType validationType,
			string name)
		{
			if (item is null)
			{
				throw new ArgumentNullException(Globals.CurrentCulture(MessageText(validationType, ValidatorMessages.NullMessage, name)));
			}
		}

		public static Validator<T> Validate<T>(this T? item, string name)
			where T : class? =>
				new(item, ValidationType.Other, name);

		public static Validator<T> Validate<T>(this T? item, ValidationType validationType, string name)
			where T : class? =>
				new(item, validationType, name);
		#endregion

		#region Internal Methods
		internal static string MessageText(ValidationType valueType, string message, string name)
		{
			var valueTypeText = ValueTypeTexts.TryGetValue(valueType, out var retval)
				? retval
				: throw new KeyNotFoundException();
			valueTypeText = Globals.CurrentCulture(ValidatorMessages.InfoFormat, valueTypeText, name);
			valueTypeText = Globals.CurrentCulture(ValidatorMessages.ErrorFormat, message, valueTypeText);
			return valueTypeText;
		}

		internal static InvalidOperationException GetException(string message) => new(message);

		internal static InvalidOperationException ValidatorException(ValidationType validationType, string message, string name) => GetException(MessageText(validationType, message, name));
		#endregion
	}
}
