namespace RobinHood70.CommonCode
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using RobinHood70.CommonCode.Properties;
	using static RobinHood70.CommonCode.Globals;

	public static class FluentValidator
	{
		#region Generic Class Extensions

		/// <summary>Throws an exception if the argument is null.</summary>
		/// <typeparam name="T">The type of the object to check.</typeparam>
		/// <param name="instance">The value that may be null.</param>
		/// <param name="paramName">The name of the parameter in the original method.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="instance" /> is null.</exception>
		/// <remarks>This is functionally identical to <see cref="NotNull{T}(T?, string)"/> except that it's intended strictly for function arguments and throws an ArgumentNullException.</remarks>
		public static T ArgumentNotNull<T>([NotNull] this T? instance, string paramName)
			where T : class =>
			instance is object
				? instance
				: throw new ArgumentNullException(paramName);

		/// <summary>Throws an exception if the value is null.</summary>
		/// <typeparam name="T">The type of the value to check.</typeparam>
		/// <param name="instance">The value that may be null.</param>
		/// <param name="valueName">The name of the value in the original method.</param>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="instance" /> is null.</exception>
		public static T NotNull<T>([NotNull] this T? instance, string valueName)
			where T : class =>
			instance is object
				? instance
				: throw new InvalidOperationException(CurrentCulture(Resources.ValueNull, valueName));

		/// <summary>Throws an exception if the property value is null.</summary>
		/// <typeparam name="T">The type of the object to check.</typeparam>
		/// <param name="instance">The value that may be null.</param>
		/// <param name="nameParts">The parts that make up the name of the object, property, or method. These will be joined by periods before being displayed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="instance" /> is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown when <paramref name="instance"/> is null.</exception>
		public static T NotNull<T>([NotNull] T? instance, params string[] nameParts)
			where T : class =>
			instance is object
				? instance
				: throw new InvalidOperationException(CurrentCulture(Resources.ValueNull, string.Join('.', nameParts)));
		#endregion
	}
}
