namespace RobinHood70.CommonCode
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.CompilerServices;
	using RobinHood70.CommonCode.Properties;

	public enum ValidationType
	{
		Unknown,
		Argument,
		Property,
		Value,
	}

	public static class Validator
	{
		public static T NotNull<T>([NotNull][ValidatedNotNull] this T? item, string name, [CallerMemberName] string caller = Globals.Unknown)
			where T : class? =>
			item ?? throw new ArgumentNullException(Globals.CurrentCulture(ValidatorMessages.ItemTypeValue, name, caller));

		public static void ThrowNull([NotNull][ValidatedNotNull] this object item, string name, [CallerMemberName] string caller = Globals.Unknown)
		{
			if (item is null)
			{
				throw new ArgumentNullException(Globals.CurrentCulture(ValidatorMessages.ItemTypeValue, name, caller));
			}
		}

		public static Validator<T> Validate<T>(this T? item)
			where T : class? => new(item, nameof(item));

		public static Validator<T> Validate<T>(this T? item, params string[] nameParts)
			where T : class? => new(item, string.Join('.', nameParts));

		public static Validator<T> Validate<T>(this T? item, ValidationType validationType, params string[] nameParts)
			where T : class? => new(item, validationType, string.Join('.', nameParts));
	}
}
