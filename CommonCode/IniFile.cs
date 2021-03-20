namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using static RobinHood70.CommonCode.Globals;

	/// <summary>A class for basic ini file management.</summary>
	public class IniFile : IReadOnlyList<IniSection>
	{
		#region Fields
		private readonly List<IniSection> sections = new();
		#endregion

		#region Constructors

		/// <summary>Initializes a new instance of the <see cref="IniFile"/> class.</summary>
		/// <param name="iniFileName">The file name to parse.</param>
		public IniFile(string iniFileName)
		{
			ThrowNull(iniFileName, nameof(iniFileName));
			if (Path.GetExtension(iniFileName).Length == 0)
			{
				iniFileName += ".ini";
			}

			using var stream = File.OpenText(Path.GetFullPath(iniFileName));
			this.ReadStream(stream);
		}

		public IniFile(StreamReader stream)
		{
			ThrowNull(stream, nameof(stream));
			this.ReadStream(stream);
		}
		#endregion

		#region Public Static Properties

		/// <summary>Gets the characters that constitute comment delimiters. Note that at this point, the handling is very simple, so strings like // or /* */ cannot be dealt with.</summary>
		public static IList<string> CommentDelimiters { get; } = new List<string> { ";" };
		#endregion

		#region Public Properties

		/// <summary>Gets the number of sections in the file.</summary>
		public int Count => this.sections.Count;

		/// <summary>Gets the name of the ini file.</summary>
		public string? FileName { get; private set; }
		#endregion

		#region Indexers

		/// <summary>Gets a section by its offset.</summary>
		/// <param name="index">The numeric index.</param>
		/// <returns>The relevant section.</returns>
		public IniSection this[int index] => this.sections[index];

		/// <summary>Gets a section by name.</summary>
		/// <param name="name">The name of the section.</param>
		/// <returns>The relevant section.</returns>
		public IniSection this[string name] => this.sections.Find(s => s.Name.Equals(name, StringComparison.Ordinal)) ?? throw new KeyNotFoundException();
		#endregion

		#region Public Methods

		/// <summary>Returns an enumerator that iterates through each section.</summary>
		/// <returns>An enumerator that iterates through each section.</returns>
		public IEnumerator<IniSection> GetEnumerator() => this.sections.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.sections.GetEnumerator();

		/// <summary>Finds all sections with the given name, in the event that there is more than one identically named section.</summary>
		/// <param name="name">The section name to search for.</param>
		/// <returns>A list of sections.</returns>
		private IList<IniSection> FindAll(string name) => this.sections.FindAll(s => s.Name.Equals(name, StringComparison.Ordinal));

		private void ReadStream(StreamReader stream)
		{
			var name = string.Empty;
			var lines = new List<string>();
			if (stream.BaseStream is FileStream fileStream)
			{
				this.FileName = fileStream.Name;
			}

			while (stream.ReadLine() is string line)
			{
				var trimmedLine = line.TrimEnd();
				if (trimmedLine.Length > 0)
				{
					if (trimmedLine[0] == '[' && trimmedLine[^1] == ']')
					{
						if (name.Length > 0 || lines.Count > 0)
						{
							this.sections.Add(new IniSection(name, lines));
						}

						name = trimmedLine[1..^1];
						lines = new List<string>();
					}
					else
					{
						lines.Add(trimmedLine);
					}
				}
			}

			if (name.Length > 0 || lines.Count > 0)
			{
				this.sections.Add(new IniSection(name, lines));
			}
		}
		#endregion
	}
}