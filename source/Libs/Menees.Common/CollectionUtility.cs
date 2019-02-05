namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods for collection handling.
	/// </summary>
	public static class CollectionUtility
	{
		#region Public Methods

		/// <summary>
		/// Gets a read-only dictionary.
		/// </summary>
		/// <param name="dictionary">The dictionary to wrap or return.  This must be non-null.</param>
		/// <returns>If the original <paramref name="dictionary"/> is already read-only, this method
		/// returns it as is.  If it is not read-only, this method returns a read-only wrapper around it.</returns>
		public static IDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			Conditions.RequireReference(dictionary, "dictionary");

			IDictionary<TKey, TValue> result = dictionary;

			if (result != null && !result.IsReadOnly)
			{
				result = new ReadOnlyDictionary<TKey, TValue>(dictionary);
			}

			return result;
		}

		/// <summary>
		/// Gets a read-only set.
		/// </summary>
		/// <param name="value">The set to wrap or return.  This must be non-null.</param>
		/// <returns>If the original <paramref name="value"/> is already read-only, this method
		/// returns it as is.  If it is not read-only, this method returns a read-only wrapper around it.</returns>
		public static ISet<T> AsReadOnly<T>(this ISet<T> value)
		{
			Conditions.RequireReference(value, "value");

			ISet<T> result = value;

			if (result != null && !result.IsReadOnly)
			{
				result = new ReadOnlySet<T>(value);
			}

			return result;
		}

		/// <summary>
		/// Returns an empty array of the specified type.
		/// </summary>
		/// <typeparam name="TResult">The type of elements in the returned array.</typeparam>
		/// <returns>An empty array of the specified type.</returns>
		/// <remarks>
		/// This method caches an empty array of each type requested, and it is more efficient
		/// than creating empty array instances repeatedly.
		/// <para/>
		/// This method is similar to <see cref="Enumerable.Empty{TResult}"/> but this returns
		/// an array instead of just an <see cref="IEnumerable{TResult}"/>.
		/// </remarks>
		public static TResult[] EmptyArray<TResult>()
		{
			TResult[] result = EmptyArrayCache<TResult>.GetInstance();
			return result;
		}

		#endregion

		#region Private Types

		private static class EmptyArrayCache<TElement>
		{
			#region Private Data Members

			// This is volatile because of how the GetInstance method sets it below.
			private static volatile TElement[] instance;

			#endregion

			#region Public Methods

			public static TElement[] GetInstance()
			{
				// The instance member is marked volatile so this will be "last one wins"
				// if multiple threads write to this at the same time.  That's ok.  We just
				// want to cache one, and for our purposes all empty TElement arrays
				// are considered the same.
				if (instance == null)
				{
					instance = new TElement[0];
				}

				return instance;
			}

			#endregion
		}

		#endregion
	}
}
