namespace Menees.Collections
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Provides a generic read-only set.
	/// </summary>
	/// <typeparam name="T">
	/// The type of items in the set.
	/// </typeparam>
	/// <remarks>
	/// An instance of the ReadOnlySet generic class is always read-only.
	/// A set that is read-only is simply a set with a wrapper that prevents
	/// modifying the set; therefore, if changes are made to the underlying
	/// set, the read-only dictionary reflects those changes.  See
	/// <see cref="HashSet{T}"/> or <see cref="SortedSet{T}"/>
	/// for a modifiable version of this class.
	/// </remarks>
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(ReadOnlySetDebugView<>))]
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
		Justification = "This must end with 'Set' instead of 'Collection' because it implements the standard ISet<T> interface.")]
	public sealed class ReadOnlySet<T> : ISet<T>
	{
		#region Private Data Members

		private ISet<T> source;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance that wraps the supplied <paramref name="source"/>.
		/// </summary>
		/// <param name="source">The set to wrap.</param>
		public ReadOnlySet(ISet<T> source)
		{
			Conditions.RequireReference(source, "source");
			this.source = source;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the number of items in the set.
		/// </summary>
		public int Count
		{
			get { return this.source.Count; }
		}

		/// <summary>
		/// Gets whether this set is read-only, which will always return true.
		/// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}

		#endregion

		#region ISet<T> Members

		bool ISet<T>.Add(T item)
		{
			throw NewNotSupportedException();
		}

		void ISet<T>.ExceptWith(IEnumerable<T> other)
		{
			throw NewNotSupportedException();
		}

		void ISet<T>.IntersectWith(IEnumerable<T> other)
		{
			throw NewNotSupportedException();
		}

		/// <summary>
		/// Determines whether the current set is a proper (strict) subset of a specified collection.
		/// </summary>
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			bool result = this.source.IsProperSubsetOf(other);
			return result;
		}

		/// <summary>
		/// Determines whether the current set is a proper (strict) superset of a specified collection.
		/// </summary>
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			bool result = this.source.IsProperSupersetOf(other);
			return result;
		}

		/// <summary>
		/// Determines whether a set is a subset of a specified collection.
		/// </summary>
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			bool result = this.source.IsSubsetOf(other);
			return result;
		}

		/// <summary>
		/// Determines whether the current set is a superset of a specified collection.
		/// </summary>
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			bool result = this.source.IsSupersetOf(other);
			return result;
		}

		/// <summary>
		/// Determines whether the current set overlaps with the specified collection.
		/// </summary>
		public bool Overlaps(IEnumerable<T> other)
		{
			bool result = this.source.Overlaps(other);
			return result;
		}

		/// <summary>
		/// Determines whether the current set and the specified collection contain the same elements.
		/// </summary>
		public bool SetEquals(IEnumerable<T> other)
		{
			bool result = this.source.SetEquals(other);
			return result;
		}

		void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
		{
			throw NewNotSupportedException();
		}

		void ISet<T>.UnionWith(IEnumerable<T> other)
		{
			throw NewNotSupportedException();
		}

		void ICollection<T>.Add(T item)
		{
			throw NewNotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw NewNotSupportedException();
		}

		/// <summary>
		/// Determines whether the set contains a specific value.
		/// </summary>
		public bool Contains(T item)
		{
			bool result = this.source.Contains(item);
			return result;
		}

		/// <summary>
		/// Copies the elements of the set to an array, starting at a particular array index.
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.source.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T item)
		{
			throw NewNotSupportedException();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			return this.source.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region Private Methods

		private static NotSupportedException NewNotSupportedException()
		{
			return Exceptions.NewException(new NotSupportedException("The set is read-only."));
		}

		#endregion
	}
}
