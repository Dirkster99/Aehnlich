namespace Menees.Diagnostics
{
	#region Using Directives

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Provides the global log context properties for <see cref="Menees.Log.GlobalContext"/>.
	/// </summary>
	public sealed class GlobalLogContext : ILogContext
	{
		#region Private Data Members

		private ConcurrentDictionary<string, object> entries = new ConcurrentDictionary<string, object>();

		#endregion

		#region Constructors

		internal GlobalLogContext()
		{
			// Default this to a meaningful value, so it will always be set to something.
			string applicationName = GetDefaultApplicationName();
			this.SetApplicationName(applicationName);
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
					this.entries[key] = value;
				}
			}
		}

		/// <summary>
		/// Removes all entries from the current context.
		/// </summary>
		public void Clear()
		{
			this.entries.Clear();
		}

		/// <summary>
		/// Removes the specified key from the current context.
		/// </summary>
		public void Remove(string key)
		{
			object value;
			this.entries.TryRemove(key, out value);
		}

		/// <summary>
		/// Checks whether the context contains the specified key.
		/// </summary>
		public bool ContainsKey(string key)
		{
			bool result = this.entries.ContainsKey(key);
			return result;
		}

		/// <summary>
		/// Tries to get the value associated with the specified key.
		/// </summary>
		public bool TryGetValue<T>(string key, out T value)
		{
			object propertyValue;
			bool result = this.entries.TryGetValue(key, out propertyValue) && propertyValue != null;
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

		#region Internal Methods

		internal static string GetDefaultApplicationName()
		{
			// NOTE: This method is in a logging class instead of ApplicationInfo
			// because during static initialization our logging classes are the lowest
			// level (i.e., they shouldn't depend on any of our other classes).
			// It wouldn't be safe to call back up to ApplicationInfo since its static
			// initialization can call into the Log class, which uses GlobalLogContext.
			AppDomain domain = AppDomain.CurrentDomain;
			string result = domain.FriendlyName;

			// If this is the default app domain, then the friendly name will
			// include the executable's file extension, which we don't want.
			if (domain.IsDefaultAppDomain())
			{
				result = Path.GetFileNameWithoutExtension(result);
			}

			return result;
		}

		internal void SetApplicationName(string applicationName)
		{
			this["Application"] = applicationName;
		}

		internal void MergeEntriesInto(IDictionary<string, object> target)
		{
			foreach (var pair in this.entries)
			{
				target[pair.Key] = this.entries.Count;
			}
		}

		#endregion
	}
}
