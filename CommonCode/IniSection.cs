namespace RobinHood70.CommonCode
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>Represents an ini file's section as a collection of ini keys, along with the section name.</summary>
	/// <remarks>While this acts similar to a keyed collection, it isn't one due to the fact that keys can be duplicated in a file.</remarks>
	public class IniSection : IReadOnlyList<IniKey>
	{
		#region Fields
		private readonly List<IniKey> keys = [];
		#endregion

		#region Constructors

		/// <summary>Initializes a new instance of the <see cref="IniSection"/> class.</summary>
		/// <param name="name">The name.</param>
		/// <param name="keys">The section's keys.</param>
		public IniSection(string name, IEnumerable<string> keys)
		{
			ArgumentNullException.ThrowIfNull(name);
			ArgumentNullException.ThrowIfNull(keys);
			this.Name = name;
			List<IniKey> list = [];
			foreach (var key in keys)
			{
				list.Add(new IniKey(key.Trim()));
			}

			this.keys = list;
		}
		#endregion

		#region Public Properties

		/// <summary>Gets the number of elements in the collection.</summary>
		public int Count => this.keys.Count;

		/// <summary>Gets the section name.</summary>
		public string Name { get; }
		#endregion

		#region Indexers

		/// <summary>Gets the ini key at the specified index.</summary>
		/// <param name="index">The index.</param>
		/// <returns>The <see cref="IniKey"/> at the specified index.</returns>
		public IniKey this[int index] => this.keys[index];

		/// <summary>Gets the ini key with the specified name.</summary>
		/// <param name="name">The name.</param>
		/// <returns>The <see cref="IniKey"/> with the specified name.</returns>
		public IniKey? this[string name] => this.keys.Find(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		#endregion

		#region Public Methods

		/// <summary>Adds the specified ini key.</summary>
		/// <param name="key">The key.</param>
		public void Add(IniKey key) => this.keys.Add(key);

		/// <summary>Finds all keys with a given name.</summary>
		/// <param name="name">The name.</param>
		/// <returns>A list of all keys in this section that have the same name.</returns>
		public IniKey? Find(string name) => this.keys.Find(s => s.Value.Equals(name, StringComparison.OrdinalIgnoreCase));

		/// <summary>Finds all keys with a given name.</summary>
		/// <param name="name">The name.</param>
		/// <returns>A list of all keys in this section that have the same name.</returns>
		public IList<IniKey> FindAll(string name) => this.keys.FindAll(s => s.Value.Equals(name, StringComparison.OrdinalIgnoreCase));

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<IniKey> GetEnumerator() => this.keys.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.keys.GetEnumerator();

		/// <summary>Removes the specified key.</summary>
		/// <param name="key">The key.</param>
		public void Remove(IniKey key) => this.keys.Remove(key);

		/// <summary>Removes the specified key by name.</summary>
		/// <param name="name">The name.</param>
		/// <remarks>In the event that there is more than one key with the same name, all of them will be removed.</remarks>
		public void Remove(string name) => this.keys.RemoveAll(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		#endregion
	}
}