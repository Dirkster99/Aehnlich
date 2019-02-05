namespace Menees.Diagnostics
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	#region ILogContext

	/// <summary>
	/// Defines the API for <see cref="Menees.Log.GlobalContext"/> and <see cref="Menees.Log.ThreadContext"/>.
	/// </summary>
	public interface ILogContext
	{
		/// <summary>
		/// Gets or sets the specified key/value pair in the current context.
		/// </summary>
		object this[string key]
		{
			get; set;
		}

		/// <summary>
		/// Removes all entries from the current context.
		/// </summary>
		void Clear();

		/// <summary>
		/// Removes the specified key from the current context.
		/// </summary>
		void Remove(string key);

		/// <summary>
		/// Checks whether the context contains the specified key.
		/// </summary>
		bool ContainsKey(string key);

		/// <summary>
		/// Tries to get the value associated with the specified key.
		/// </summary>
		bool TryGetValue<T>(string key, out T value);
	}

	#endregion
}
