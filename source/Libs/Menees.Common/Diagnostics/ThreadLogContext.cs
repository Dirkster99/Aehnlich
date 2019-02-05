namespace Menees.Diagnostics
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Text;
	using System.Threading;

	#endregion

	/// <summary>
	/// Provides the current thread's log context properties for <see cref="Menees.Log.ThreadContext"/>.
	/// </summary>
	[SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "Only a single instance of this class is ever created, and it lives the life of the process.")]
	public sealed class ThreadLogContext : ILogContext
	{
		#region Private Data Members

		// This will create a dictionary for each thread that requests one.
		private readonly ThreadLocal<Dictionary<string, object>> entries = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());

		#endregion

		#region Constructors

		internal ThreadLogContext()
		{
		}

		#endregion

		#region ILogContext Members

		/// <summary>
		/// Gets or sets the specified key/value pair in the current context.
		/// </summary>
		public object this[string key]
		{
			get
			{
				object result;
				if (!this.TryGetValue(key, out result))
				{
					throw Exceptions.Log(new KeyNotFoundException("The specified key was not found: " + key));
				}

				return result;
			}

			set
			{
				// Ignore null values, so ContainsKey and TryGetValue can determine
				// whether a key/value pair is really in the map or not.
				if (value != null)
				{
					this.entries.Value[key] = value;
				}
			}
		}

		/// <summary>
		/// Removes all entries from the current context.
		/// </summary>
		public void Clear()
		{
			if (this.entries.IsValueCreated)
			{
				this.entries.Value.Clear();
			}
		}

		/// <summary>
		/// Removes the specified key from the current context.
		/// </summary>
		public void Remove(string key)
		{
			if (this.entries.IsValueCreated)
			{
				this.entries.Value.Remove(key);
			}
		}

		/// <summary>
		/// Checks whether the context contains the specified key.
		/// </summary>
		public bool ContainsKey(string key)
		{
			bool result = false;

			if (this.entries.IsValueCreated)
			{
				result = this.entries.Value.ContainsKey(key);
			}

			return result;
		}

		/// <summary>
		/// Tries to get the value associated with the specified key.
		/// </summary>
		public bool TryGetValue<T>(string key, out T value)
		{
			object propertyValue = null;
			bool result = this.entries.IsValueCreated && this.entries.Value.TryGetValue(key, out propertyValue) && propertyValue != null;
			if (result)
			{
				// Cast the object value into type T.  If the key was associated with a value
				// of a different type, then this will throw an InvalidCastException.
				value = (T)propertyValue;
			}
			else
			{
				value = default(T);
			}

			return result;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Inserts a key/value pair into this context.
		/// </summary>
		/// <param name="key">The key to insert.</param>
		/// <param name="value">The value to associate with the key.</param>
		/// <returns>A disposable object that can be used to pop the new key/value pair out
		/// and restore any previous value associated with the key.</returns>
		/// <remarks>
		/// Key/value pairs pushed into the thread context should always be strictly scoped.
		/// In other words, the method that pushes a value into the thread context should
		/// always pop it back out.  This Push method returns a disposable object that can be
		/// used with C#'s using statement to guarantee that the key/value pair will be
		/// added and removed correctly.
		/// </remarks>
		/// <example>
		/// Pushing and popping a key/value pair.
		/// <code>
		/// using (Log.ThreadContext.Push("test", myTestObject))
		/// {
		///     // Any messages logged here will contain the "test" context data.
		/// }
		/// // Now the "test" thread context data will be restored to its previous value or removed.
		/// </code>
		/// </example>
		public IDisposable Push(string key, object value)
		{
			object previousValue;
			bool hasPreviousValue = this.TryGetValue(key, out previousValue);
			this[key] = value;
			return new Disposer(() =>
				{
					if (hasPreviousValue)
					{
						this[key] = previousValue;
					}
					else
					{
						this.Remove(key);
					}
				});
		}

		#endregion

		#region Internal Methods

		internal void MergeEntriesInto(IDictionary<string, object> target)
		{
			// Don't force a dictionary to be created for this thread if it hasn't been already.
			if (this.entries.IsValueCreated)
			{
				foreach (var pair in this.entries.Value)
				{
					target[pair.Key] = pair.Value;
				}
			}
		}

		#endregion
	}
}
