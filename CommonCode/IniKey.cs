namespace RobinHood70.CommonCode
{
	using System;

	// Note that class assumes read-only use and trims excess spacing that could otherwise be important for formatting.

	/// <summary>Represents a single ini key.</summary>
	public class IniKey
	{
		#region Constructors

		/// <summary>Initializes a new instance of the <see cref="IniKey"/> class.</summary>
		/// <param name="line">The full line of text representing the key.</param>
		public IniKey(string line)
			: this(line, true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="IniKey"/> class.</summary>
		/// <param name="line">The full line of text representing the key.</param>
		/// <param name="allowComments">If set to <c>true</c>, allows trailing comments. Otherwise, the value is assumed to be everything after the equals sign.</param>
		/// <exception cref="ArgumentException">Invalid INI line. There is more than one equals sign on the line, or there are no equals signs, and <paramref name="allowComments"/> is false.</exception>
		public IniKey(string line, bool allowComments)
		{
			line.ThrowNull();
			if (allowComments)
			{
				var delimiters = new string[IniFile.CommentDelimiters.Count];
				IniFile.CommentDelimiters.CopyTo(delimiters, 0);
				var commentSplit = line.Split(delimiters, 2, StringSplitOptions.None);
				if (commentSplit[0].Length == 0)
				{
					this.Name = string.Empty;
					this.Value = string.Empty;
					this.Comment = line[1..];
					return;
				}

				if (commentSplit.Length == 2)
				{
					this.Comment = commentSplit[1];
					line = commentSplit[0];
				}
			}

			var split = line.Split(TextArrays.EqualsSign, 2);
			if (split.Length != 2)
			{
				throw new ArgumentException($"Invalid INI line: {line}", nameof(line));
			}

			this.Name = split[0].Trim();
			this.Value = split[1].Trim();
		}

		/// <summary>Initializes a new instance of the <see cref="IniKey"/> class.</summary>
		/// <param name="name">The key name.</param>
		/// <param name="value">The key value.</param>
		public IniKey(string name, string value)
			: this(name, value, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="IniKey"/> class.</summary>
		/// <param name="name">The key name.</param>
		/// <param name="value">The key value.</param>
		/// <param name="comment">The comment.</param>
		public IniKey(string name, string value, string? comment)
		{
			this.Name = name.NotNull().Trim();
			this.Value = value.NotNull().Trim();
			this.Comment = comment?.Trim();
		}
		#endregion

		#region Public Properties

		/// <summary>Gets the key's name.</summary>
		public string Name { get; }

		/// <summary>Gets the key's value.</summary>
		public string Value { get; }

		/// <summary>Gets any trailing comment.</summary>
		public string? Comment { get; }
		#endregion
	}
}