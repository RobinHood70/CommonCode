namespace RobinHood70.CommonCode;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>A dictionary that allows a mix of case-sensitive and case-insensitive searches.</summary>
/// <typeparam name="T">The type of values in the dictionary.</typeparam>
/// <remarks>Due to the requirement of knowing what should be case-sensitive and what not, this only implements <see cref="IReadOnlyDictionary{TKey, TValue}"/>, but supports a custom Add function as well as a Clear function.</remarks>
public class MixedSensitivityDictionary<T> : IReadOnlyDictionary<string, T>
{
	#region Fields
	private readonly Dictionary<string, T> wordsCI = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, T> wordsCS = new(StringComparer.Ordinal);
	#endregion

	#region Public Properties

	/// <inheritdoc/>
	public int Count => this.wordsCS.Count + this.wordsCI.Count;

	/// <inheritdoc/>
	public IEnumerable<string> Keys
	{
		get
		{
			foreach (var word in this.wordsCS)
			{
				yield return word.Key;
			}

			foreach (var word in this.wordsCI)
			{
				yield return word.Key;
			}
		}
	}

	/// <inheritdoc/>
	public IEnumerable<T> Values
	{
		get
		{
			foreach (var word in this.wordsCS)
			{
				yield return word.Value;
			}

			foreach (var word in this.wordsCI)
			{
				yield return word.Value;
			}
		}
	}
	#endregion

	#region Public Indexers

	/// <inheritdoc/>
	public T this[string key] => this.wordsCS.GetValueOrDefault(key) ?? this.wordsCI[key];
	#endregion

	#region Public Methods

	/// <summary>Adds the specified key and value to the dictionary using the specified case-sensitivity.</summary>
	/// <param name="caseSensitive">The case-sensitivity of the element to add.</param>
	/// <param name="key">The key of the element to add.</param>
	/// <param name="value">The value of the element to add. The value can be null for reference types.</param>
	public void Add(bool caseSensitive, string key, T value)
	{
		var dict = caseSensitive ? this.wordsCS : this.wordsCI;
		dict.Add(key, value);
	}

	/// <summary>Removes all items from the <see cref="MixedSensitivityDictionary{T}"/>.</summary>
	public void Clear()
	{
		this.wordsCS.Clear();
		this.wordsCI.Clear();
	}

	/// <inheritdoc/>
	public bool ContainsKey(string key) => this.wordsCS.ContainsKey(key) || this.wordsCI.ContainsKey(key);

	/// <inheritdoc/>
	public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
	{
		foreach (var word in this.wordsCS)
		{
			yield return word;
		}

		foreach (var word in this.wordsCI)
		{
			yield return word;
		}
	}

	/// <inheritdoc/>
	public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value) =>
		this.wordsCS.TryGetValue(key, out value) ||
		this.wordsCI.TryGetValue(key, out value);

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	#endregion
}