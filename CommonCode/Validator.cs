namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;
	using RobinHood70.CommonCode.Properties;

	#region Public Enumerations
	public enum ValidationType
	{
		Unknown,
		Argument,
		Property,
		Value,
	}
	#endregion

	public static class Validator
	{
		#region Fields
		private static readonly IReadOnlyDictionary<ValidationType, string> TypeTexts = new Dictionary<ValidationType, string>()
		{
			[ValidationType.Argument] = ValidatorMessages.ItemTypeArgument,
			[ValidationType.Property] = ValidatorMessages.ItemTypeProperty,
			[ValidationType.Unknown] = ValidatorMessages.ItemTypeUnknown,
			[ValidationType.Value] = ValidatorMessages.ItemTypeValue,
		};
		#endregion

		#region Public Properties
		public static string ValueNullText(ValidationType validationType) =>
			TypeTexts.TryGetValue(validationType, out var retval)
				? retval
				: throw new KeyNotFoundException();
		#endregion

		#region Public Methods
		public static string Join(params string[] nameParts) =>
			string.Join('.', nameParts ?? throw new ArgumentNullException(nameof(nameParts)));

		public static T NotNull<T>([NotNull][ValidatedNotNull] this T? item, string name, [CallerMemberName] string caller = Globals.Unknown)
			where T : class? =>
				NotNull(item, ValidationType.Argument, name, caller);

		public static T NotNull<T>([NotNull][ValidatedNotNull] this T? item, ValidationType validationType, string name, [CallerMemberName] string caller = Globals.Unknown)
			where T : class? =>
				item ?? throw new ArgumentNullException(Globals.CurrentCulture(ValueNullText(validationType), name, caller));

		public static void ThrowNull([NotNull][ValidatedNotNull] this object? item, string name, [CallerMemberName] string caller = Globals.Unknown) =>
			ThrowNull(item, ValidationType.Argument, name, caller);

		public static void ThrowNull([NotNull][ValidatedNotNull] this object? item, ValidationType validationType, string name, [CallerMemberName] string caller = Globals.Unknown)
		{
			if (item is null)
			{
				throw new ArgumentNullException(Globals.CurrentCulture(ValueNullText(validationType), name, caller));
			}
		}

		public static Validator<T> Validate<T>(this T? item, [CallerMemberName] string caller = Globals.Unknown)
			where T : class? =>
				new(item, ValidationType.Unknown, nameof(item), caller);

		public static Validator<T> Validate<T>(this T? item, string name, [CallerMemberName] string caller = Globals.Unknown)
			where T : class? =>
				new(item, ValidationType.Unknown, name, caller);

		public static Validator<T> Validate<T>(this T? item, ValidationType validationType, string name, [CallerMemberName] string caller = Globals.Unknown)
			where T : class? =>
				new(item, validationType, name, caller);
		#endregion
	}
}
