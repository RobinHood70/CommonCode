namespace RobinHood70.CommonCode;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>A read-only version of the KeyedCollection class.</summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <seealso cref="IReadOnlyList{TItem}" />
/// <seealso cref="IReadOnlyDictionary{TKey, TValue}"/>
public class ReadOnlyKeyedCollection<TKey, TItem> : IReadOnlyDictionary<TKey, TItem>, IReadOnlyList<TItem>
	where TKey : notnull
{
	#region Fields
	private readonly Dictionary<TKey, TItem> dictionary;
	private readonly List<TItem> items;
	#endregion

	#region Constructors

	/// <summary>Initializes a new instance of the <see cref="ReadOnlyKeyedCollection{TKey, TItem}" /> class that uses the default equality comparer.</summary>
	/// <param name="keyFunc">The function that provides the key for the collection.</param>
	/// <param name="items">The items.</param>
	public ReadOnlyKeyedCollection(Func<TItem, TKey> keyFunc, IEnumerable<TItem> items)
		: this(keyFunc, items, comparer: null)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="ReadOnlyKeyedCollection{TKey, TItem}"/> class that uses the specified equality comparer.</summary>
	/// <param name="keyFunc">The function that provides the key for the collection.</param>
	/// <param name="items">The items.</param>
	/// <param name="comparer">The implementation of the <see cref="IEqualityComparer{TKey}"/> generic interface to use when comparing keys, or null to use the default equality comparer for the type of the key, obtained from <see cref="EqualityComparer{TKey}.Default"/>.</param>
	public ReadOnlyKeyedCollection(Func<TItem, TKey> keyFunc, IEnumerable<TItem> items, IEqualityComparer<TKey>? comparer)
	{
		ArgumentNullException.ThrowIfNull(keyFunc);
		ArgumentNullException.ThrowIfNull(items);
		this.items = new List<TItem>(items);
		this.dictionary = new Dictionary<TKey, TItem>(comparer ?? EqualityComparer<TKey>.Default);

		// We iterate over our own list in case the original is slow or a one-shot.
		foreach (var item in this.items)
		{
			this.dictionary.Add(keyFunc(item), item);
		}
	}
	#endregion

	#region Public Properties

	/// <summary>Gets the number of elements in the collection.</summary>
	/// <value>The number of elements in the collection.</value>
	public int Count => this.items.Count;

	/// <summary>Gets an enumerable collection that contains the keys in the collection.</summary>
	/// <value>The keys.</value>
	public IEnumerable<TKey> Keys => this.dictionary.Keys;

	/// <summary>Gets an enumerable collection that contains the values in the collection.</summary>
	/// <value>The values.</value>
	public IEnumerable<TItem> Values => this.dictionary.Values;
	#endregion

	#region Public Indexers

	/// <summary>Gets the <typeparamref name="TItem"/> with the specified key.</summary>
	/// <param name="key">The key of the element to get.</param>
	/// <returns>The element with the specified key. If an element with the specified key is not found, an exception is thrown.</returns>
	/// <exception cref="ArgumentNullException">key is null.</exception>
	/// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
	public TItem this[TKey key] => this.dictionary[key];

	/// <summary>Gets the <typeparamref name="TItem"/> at the specified index.</summary>
	/// <param name="index">The index.</param>
	/// <returns>The <typeparamref name="TItem"/>.</returns>
	public TItem this[int index] => this.items[index];
	#endregion

	#region Public Methods

	/// <summary>Determines whether the collection contains an element with the specified key.</summary>
	/// <param name="key">The key to locate in the <see cref="ReadOnlyKeyedCollection{TKey, TItem}"/>.</param>
	/// <returns><see langword="true"/> if the <see cref="ReadOnlyKeyedCollection{TKey, TItem}"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentNullException">key is null.</exception>
	public bool Contains(TKey key) => this.dictionary.ContainsKey(key);

	/// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
	/// <param name="key">The key to locate.</param>
	/// <returns><see langword="true"/> if the read-only dictionary contains an element that has the specified key; otherwise, <see langword="false"/>.</returns>
	public bool ContainsKey(TKey key) => this.dictionary.ContainsKey(key);

	/// <summary>Returns an enumerator that iterates through the collection.</summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	public IEnumerator<TItem> GetEnumerator() => this.items.GetEnumerator();

	IEnumerator<KeyValuePair<TKey, TItem>> IEnumerable<KeyValuePair<TKey, TItem>>.GetEnumerator() => this.dictionary.GetEnumerator();

	/// <summary>Returns an enumerator that iterates through a collection.</summary>
	/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
	IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();

	/// <summary>Comparable to <see cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)" />, attempts to get the value associated with the specified key.</summary>
	/// <param name="key">The key of the value to get.</param>
	/// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
	/// <returns><see langword="true" /> if the collection contains an element with the specified key; otherwise, <see langword="false" />.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key" /> is <see langword="null" />.</exception>
	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TItem value) => this.dictionary.TryGetValue(key, out value);

	/// <summary>Returns the value associated with the specified key.</summary>
	/// <param name="key">The key of the value to get.</param>
	/// <returns>The value associated with the specified key, or <see langword="default"/> if not found.</returns>
	[return: MaybeNull]
	public TItem ValueOrDefault(TKey key) =>
		!EqualityComparer<TKey>.Default.Equals(key, default) &&
		this.dictionary.TryGetValue(key, out var value)
			? value
			: default;
	#endregion
}