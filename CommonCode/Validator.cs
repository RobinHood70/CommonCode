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
		public static T NotNull<T>([NotNull] this T? item, [CallerMemberName] string caller = Globals.Unknown)
			where T : class? => NotNull(item, nameof(item), caller);

		public static T NotNull<T>([NotNull] this T? item, string name, [CallerMemberName] string caller = Globals.Unknown)
			where T : class? =>
			item ?? throw new InvalidOperationException(Globals.CurrentCulture(ValidatorMessages.ItemTypeValue, name, caller));

		public static Validator<T> Validate<T>(this T? item)
			where T : class? => new(item, nameof(item));

		public static Validator<T> Validate<T>(this T? item, params string[] nameParts)
			where T : class? => new(item, string.Join('.', nameParts));

		public static Validator<T> Validate<T>(this T? item, ValidationType validationType, params string[] nameParts)
			where T : class? => new(item, validationType, string.Join('.', nameParts));
	}
}
