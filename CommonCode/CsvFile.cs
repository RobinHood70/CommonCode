﻿namespace RobinHood70.CommonCode;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

/// <summary>Reads or writes a CSV file, including tab-separated files and similar formats.</summary>
/// <remarks>This class is primarily designed to handle entire files at once. <see cref="TextReader"/>- and <see cref="TextWriter"/>-based methods are also available to support streaming and the like.</remarks>
/// <remarks>Initializes a new instance of the <see cref="CsvFile"/> class.</remarks>
/// <param name="fullPath">The full path of the file.</param>
public sealed class CsvFile(string fullPath) : IList<CsvRow>
{
	#region Fields
	private readonly Dictionary<string, int> nameMap = new(StringComparer.Ordinal);
	private readonly List<CsvRow> rows = [];

	private IEnumerable<string>? headerRow;
	#endregion

	#region Public Properties

	/// <summary>Gets a value indicating whether fields should always be delimited as text.</summary>
	/// <value><see langword="true"/> if fields should always be delimited; otherwise, <see langword="false"/> (in which case, delimiting is automatic as needed).</value>
	public bool AlwaysDelimitFields { get; init; } // CONSIDER: Replacing (or supplementing) this with a column format scheme. Note, however, that Excel doesn't always respect this in any event, so it should not be a priority.

	/// <summary>Gets a value indicating whether to trim whitespace surrounding a field.</summary>
	/// <value><see langword="true"/> to trim undelimited whitespace surrounding a field; otherwise, <see langword="false"/>.</value>
	/// <remarks>When this is set to <see langword="true"/>, a row of <c>ABC, DEF</c> will result in values of "ABC" and "DEF"; when false, the second value will be " DEF". Note that a delimited value, <c>"ABC"," DEF"</c> will always return " DEF" for the second value, regardless of this setting.</remarks>
	public bool AutoTrim { get; init; } = true;

	/// <summary>Gets the number of rows currently in the file.</summary>
	public int Count => this.rows.Count;

	/// <summary>Gets a value indicating whether to double up the <see cref="FieldDelimiter"/> character if emitted as part of the field value or use the <see cref="EscapeCharacter"/>.</summary>
	/// <value>
	///   <see langword="true"/> if a delimiter character should be emitted twice; <see langword="false"/> if it should be escaped instead.</value>
	public bool DoubleUpDelimiters { get; init; } = true;

	/// <summary>Gets the file encoding.</summary>
	/// <remarks>Currently, this can be specified either in the constructor or in the Load method; in future, it will only be available in the constructor.</remarks>
	public Encoding Encoding { get; init; } = Encoding.UTF8;

	/// <summary>Gets the text to emit if a field is present but is an empty string.</summary>
	/// <value>The text to use for empty fields.</value>
	/// <remarks>If this field is null, empty fields will be treated the same as null fields. If it's an empty string, two field delimiters will be emitted with nothing between them. For any other value, that value will be emitted, with field delimiters emitted (or not) as normal.</remarks>
	public string EmptyFieldText { get; init; } = string.Empty;

	/// <summary>Gets the escape character.</summary>
	/// <value>The escape character.</value>
	public char? EscapeCharacter { get; init; }

	/// <summary>Gets the field delimiter.</summary>
	/// <value>The field delimiter. Defaults to a double-quote (<c>"</c>).</value>
	public char? FieldDelimiter { get; init; } = '"';

	/// <summary>Gets a dictionary that maps original field names (typically from the file) to new names.</summary>
	/// <remarks>Field names not present in the dictionary will retain their original names. Entries here will only be read when the Header property is set, so they should usually be set after opening the file but before reading from it, then left alone.</remarks>
	public IDictionary<string, string> HeaderFieldMap { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

	/// <summary>Gets the field separator.</summary>
	/// <value>The field separator. Defaults to a comma (<c>,</c>).</value>
	public char FieldSeparator { get; init; } = ',';

	/// <summary>Gets the full path to the file to load.</summary>
	/// <remarks>Currently, this can be specified either in the constructor or in the Load method; in future, it will only be available in the constructor.</remarks>
	public string FullPath { get; } = fullPath;

	/// <summary>Gets a value indicating whether the text file contains a header or not.</summary>
	public bool HasHeader { get; init; } = true;

	/// <summary>Gets or sets the header row.</summary>
	/// <value>The header row. <see langword="null"/> if there is no header row (<c>HasHeader = false</c> or there are no rows in the file).</value>
	public IEnumerable<string>? Header
	{
		get => this.headerRow;
		set
		{
			this.headerRow = value;
			this.nameMap.Clear();
			if (value is not null)
			{
				foreach (var fieldName in value)
				{
					var trimmed = this.AutoTrim
						? fieldName.Trim()
						: fieldName;
					if (this.HeaderFieldMap.TryGetValue(trimmed, out var field))
					{
						trimmed = field;
					}

					this.nameMap.Add(trimmed, this.nameMap.Count);
				}
			}
		}
	}

	/// <summary>Gets the number of lines to skip if the file has rows before the header row or first line of data.</summary>
	public int SkipLines { get; init; }
	#endregion

	#region Interface Properties
	bool ICollection<CsvRow>.IsReadOnly => false;
	#endregion

	#region Public Indexers

	/// <summary>Gets the <see cref="CsvRow"/> at the specified index.</summary>
	/// <param name="index">The index.</param>
	/// <value>The <see cref="CsvRow"/>.</value>
	/// <returns>The row at the specified index.</returns>
	public CsvRow this[int index]
	{
		get => this.rows[index];
		set => this.rows[index] = value;
	}
	#endregion

	#region Public Methods

	/// <summary>Adds the specified field values.</summary>
	/// <param name="fields">The field values, converted to strings using the default ToString() method for the object.</param>
	/// <returns>The CsvRow that was added.</returns>
	public CsvRow Add(IEnumerable<object> fields)
	{
		ArgumentNullException.ThrowIfNull(fields);
		List<string> list = [];
		foreach (var item in fields)
		{
			if (item.ToString() is string value)
			{
				list.Add(value);
			}
		}

		return this.Add(list);
	}

	/// <summary>Adds the specified field values.</summary>
	/// <param name="fields">The field values.</param>
	/// <returns>The CsvRow that was added.</returns>
	public CsvRow Add(params string[] fields) => this.Add(fields as IEnumerable<string>);

	/// <summary>Adds the specified field values.</summary>
	/// <param name="fields">The field values.</param>
	/// <returns>The CsvRow that was added.</returns>
	public CsvRow Add(IEnumerable<string> fields)
	{
		ArgumentNullException.ThrowIfNull(fields);
		return this.Add(new CsvRow(fields, this.nameMap));
	}

	/// <summary>Adds a <see cref="CsvRow"/> directly to the file.</summary>
	/// <param name="item">The row to add to the file.</param>
	/// <returns>The original <paramref name="item"/> parameter.</returns>
	public CsvRow Add(CsvRow item)
	{
		ArgumentNullException.ThrowIfNull(item);
		this.rows.Add(item);
		return item;
	}

	/// <summary>Adds a header with the specified field names.</summary>
	/// <param name="fieldNames">The field names.</param>
	public void AddHeader(params string[] fieldNames) => this.Header = fieldNames;

	/// <summary>Removes all items from the file and clears the header.</summary>
	public void Clear()
	{
		this.rows.Clear();
		this.Header = null;
	}

	/// <summary>Copies the rows of the file to an array, starting at a particular array index.</summary>
	/// <param name="array">The one-dimensional array that is the destination of the elements copied from collection. The array must have zero-based indexing.</param>
	/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
	public void CopyTo(CsvRow[] array, int arrayIndex) => this.rows.CopyTo(array, arrayIndex);

	/// <summary>Returns an enumerator that iterates through the collection.</summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<CsvRow> GetEnumerator() => this.rows.GetEnumerator();

	/// <summary>Inserts a row into the file at the specified index.</summary>
	/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
	/// <param name="item">The row to insert into the file.</param>
	public void Insert(int index, CsvRow item) => this.rows.Insert(index, item);

	/// <summary>Reads and parses a CSV file.</summary>
	public void Load()
	{
		using StreamReader reader = new(this.FullPath, this.Encoding);
		foreach (var row in this.ReadRows(reader))
		{
			this.Add(row);
		}
	}

	/// <summary>Reads a single row from a <see cref="TextReader"/>.</summary>
	/// <param name="reader">The <see cref="TextReader"/> to read from.</param>
	/// <returns>A list of the field values as strings or null if the end of the file has been reached.</returns>
	public IList<string>? ReadFields(TextReader reader)
	{
		ArgumentNullException.ThrowIfNull(reader);
		if (reader.Peek() == -1)
		{
			return null;
		}

		List<string> fields = new(this.nameMap.Count);
		var endOfLine = false;
		do
		{
			var value = this.ReadField(reader, ref endOfLine);
			//// Debug.Write(value+ ",");
			fields.Add(value);
		}
		while (!endOfLine);

		return fields;
	}

	/// <summary>Reads a single row from a <see cref="TextReader"/>.</summary>
	/// <param name="reader">The <see cref="TextReader"/> to read from.</param>
	/// <returns>A <see cref="CsvRow"/> with the field values or null if the end of the file has been reached. If names are provided but not enough fields are present to match the name count, the row will be padded with empty strings.</returns>
	public CsvRow? ReadRow(TextReader reader)
	{
		ArgumentNullException.ThrowIfNull(reader);
		var fields = this.ReadFields(reader);
		return fields is null
			? null
			: new CsvRow(fields, this.nameMap);
	}

	/// <summary>Iterates through the rows of the file sequentially but does not add them to the collection.</summary>
	/// <returns>The <see cref="CsvRow"/>s from the file.</returns>
	public IEnumerable<CsvRow> ReadRows()
	{
		using StreamReader reader = new(this.FullPath, this.Encoding);
		foreach (var row in this.ReadRows(reader))
		{
			yield return row;
		}
	}

	/// <summary>Iterates through the rows of the file sequentially but does not add them to the collection.</summary>
	/// <param name="reader">The <see cref="TextReader"/> to read from.</param>
	/// <returns>The <see cref="CsvRow"/>s from the file.</returns>
	public IEnumerable<CsvRow> ReadRows(TextReader reader)
	{
		ArgumentNullException.ThrowIfNull(reader);
		for (var i = 0; i < this.SkipLines; i++)
		{
			reader.ReadLine();
		}

		this.Header = null;
		this.Clear();
		if (this.HasHeader)
		{
			var header = this.ReadFields(reader);
			if (header is null)
			{
				yield break;
			}

			this.Header = header;
		}

		var row = this.ReadRow(reader);
		while (row is not null)
		{
			yield return row;
			row = this.ReadRow(reader);
		}
	}

	/// <summary>Removes the first occurrence of a specific row from the file.</summary>
	/// <param name="item">The row to remove from the file.</param>
	/// <returns><see langword="true"/> if <paramref name="item" /> was successfully removed from the file; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="item" /> is not found in the original file.</returns>
	public bool Remove(CsvRow item) => this.rows.Remove(item);

	/// <summary>Removes the row at the specified index.</summary>
	/// <param name="index">The zero-based index of the row to remove.</param>
	public void RemoveAt(int index) => this.rows.RemoveAt(index);

	/// <summary>Removes a row based on an exact value match.</summary>
	/// <param name="values">The values to match.</param>
	/// <param name="removeCount">The maximum number of rows to remove. Use -1 to remove all matching rows.</param>
	/// <returns>True if one or more rows were removed.</returns>
	public bool RemoveByValue(IEnumerable<string> values, int removeCount)
	{
		var retval = false;
		var valueList = (values as IReadOnlyList<string>) ?? [.. values];
		for (var index = 0; index < this.Count; index++)
		{
			var row = this[index];
			if (valueList.Count == row.Count)
			{
				var match = true;
				for (var i = 0; i < valueList.Count; i++)
				{
					if (!string.Equals(valueList[i], row[i], StringComparison.Ordinal))
					{
						match = false;
						break;
					}
				}

				if (match)
				{
					retval = true;
					this.RemoveAt(index);
					if (removeCount > 0)
					{
						removeCount--;
						if (removeCount == 0)
						{
							break;
						}
					}
				}
			}
		}

		return retval;
	}

	/// <summary>Saves a CSV file to the specified file with UTF-8 encoding.</summary>
	public void Save()
	{
		using FileStream fileStream = new(this.FullPath, FileMode.Create, FileAccess.Write, FileShare.None);
		using StreamWriter writeStream = new(fileStream, this.Encoding);
		this.WriteText(writeStream);
	}

	/// <summary>Writes a row to the specified <see cref="TextWriter"/> derivative.</summary>
	/// <param name="writer">The <see cref="TextWriter"/> derivative to write to.</param>
	/// <param name="row">The values for the row. This parameter allows for any string enumeration and may thus be either plain data or a <see cref="CsvRow"/>.</param>
	public void WriteRow(TextWriter writer, IEnumerable<string> row)
	{
		ArgumentNullException.ThrowIfNull(writer);
		ArgumentNullException.ThrowIfNull(row);
		this.InternalWriteRow(writer, row, 0, this.GetSpecialCharacters());
	}

	/// <summary>Writes the file to the specified <see cref="TextWriter"/> derivative.</summary>
	/// <param name="writer">The <see cref="TextWriter"/> to write to.</param>
	public void WriteText(TextWriter writer)
	{
		// We're allowing rows to be ragged internally, so figure out the highest column count and use that. If a header is specified, that always takes priority. Count could, of course, just be assumed from the first row, but even in a large list, the scan is very quick, so there's no reason not to.
		ArgumentNullException.ThrowIfNull(writer);
		int columnCount;
		var specialChars = this.GetSpecialCharacters();
		if (this.Header != null)
		{
			columnCount = ((this.Header as IReadOnlyCollection<string>) ?? [.. this.Header]).Count;
			this.InternalWriteRow(writer, this.Header, columnCount, specialChars);
		}
		else
		{
			columnCount = 0;
			foreach (var row in this)
			{
				if (row.Count > columnCount)
				{
					columnCount = row.Count;
				}
			}
		}

		foreach (var row in this)
		{
			this.InternalWriteRow(writer, row, columnCount, specialChars);
		}
	}
	#endregion

	#region Interface Methods
	void ICollection<CsvRow>.Add(CsvRow item) => this.rows.Add(item);

	bool ICollection<CsvRow>.Contains(CsvRow item) => this.rows.Contains(item);

	IEnumerator IEnumerable.GetEnumerator() => this.rows.GetEnumerator();

	int IList<CsvRow>.IndexOf(CsvRow item) => this.rows.IndexOf(item);
	#endregion

	#region Private Methods
	private char[] GetSpecialCharacters()
	{
		List<char> specialList = ['\n', '\r', '\u2028', '\u2029', this.FieldSeparator];
		if (this.EscapeCharacter.HasValue)
		{
			specialList.Add(this.EscapeCharacter.Value);
		}

		if (this.FieldDelimiter.HasValue)
		{
			specialList.Add(this.FieldDelimiter.Value);
		}

		return [.. specialList];
	}

	private void InternalWriteRow(TextWriter textWriter, IEnumerable<string> row, int columnCount, char[] specialChars)
	{
		List<string> rewriteFields = new(columnCount);
		if (columnCount == 0)
		{
			columnCount = int.MaxValue;
		}

		var columnNumber = 0;
		foreach (var field in row)
		{
			if (columnNumber < columnCount)
			{
				var value = field switch
				{
					null => string.Empty,
					string when field.Length == 0 => this.EmptyFieldText ?? string.Empty,
					_ => field
				};

				if (this.AlwaysDelimitFields ||
					!string.Equals(value.Trim(), value, StringComparison.Ordinal) ||
					value.IndexOfAny(specialChars) >= 0)
				{
					value = this.RewriteField(value);
				}

				rewriteFields.Add(value);
				columnNumber++;
			}
		}

		var line = string.Join(this.FieldSeparator.ToString(CultureInfo.InvariantCulture), rewriteFields);
		textWriter.WriteLine(line);
	}

	private string ReadField(TextReader reader, ref bool endOfLine)
	{
		StringBuilder field = new();
		var insideDelims = false;
		var outsideValue = true;
		var endOfField = false;
		while (!endOfField)
		{
			var character = (char)reader.Read();
			switch (character)
			{
				case '\uFFFF':
					endOfField = true;
					break;
				case '\n':
				case '\r':
				case '\u2028':
				case '\u2029':
					if (insideDelims)
					{
						field.Append(character);
					}
					else
					{
						if (character == '\r' && reader.Peek() == '\n')
						{
							reader.Read();
						}

						endOfField = true;
						endOfLine = true;
					}

					break;
				case var _ when character == this.FieldDelimiter:
					if (!outsideValue && this.DoubleUpDelimiters && reader.Peek() == this.FieldDelimiter)
					{
						reader.Read();
						field.Append('"');
					}
					else
					{
						outsideValue = insideDelims;
						insideDelims = !insideDelims;
					}

					break;
				case var _ when character == this.FieldSeparator:
					if (insideDelims)
					{
						outsideValue = false;
						field.Append(character);
					}
					else
					{
						endOfField = true;
					}

					break;
				case var _ when character == this.EscapeCharacter:
					var newChar = reader.Read();
					outsideValue = newChar == -1;
					field.Append(newChar == -1 ? character : newChar);
					break;
				default:
					if (outsideValue)
					{
						if (!this.AutoTrim || !char.IsWhiteSpace(character))
						{
							outsideValue = false;
							field.Append(character);
						}
					}
					else
					{
						field.Append(character);
					}

					break;
			}
		}

		return field.ToString();
	}

	private string RewriteField(string field)
	{
		StringBuilder sb = new();
		foreach (var character in field)
		{
			if (character == this.FieldDelimiter)
			{
				if (this.DoubleUpDelimiters)
				{
					sb.Append(new string(this.FieldDelimiter.Value, 2));
				}
				else
				{
					sb.Append(this.EscapeCharacter + this.FieldDelimiter.Value);
				}
			}
			else if (character == this.EscapeCharacter)
			{
				sb.Append(new string(this.EscapeCharacter.Value, 2));
			}
			else if (character == this.FieldSeparator)
			{
				sb.Append(this.FieldSeparator);
			}
			else
			{
				sb.Append(character);
			}
		}

		return this.FieldDelimiter.HasValue ? this.FieldDelimiter.Value + sb.ToString() + this.FieldDelimiter.Value : sb.ToString();
	}
	#endregion
}