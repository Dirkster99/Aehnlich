namespace Menees.Diagnostics
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Management;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods for working with WMI queries and their results.
	/// </summary>
	public static class WmiUtility
	{
		#region Public Methods

		/// <summary>
		/// Executes a WMI query and invokes an action for each result record.
		/// </summary>
		/// <param name="query">The WMI query to execute.</param>
		/// <param name="recordAction">The action to invoke for each record.</param>
		/// <exception cref="ManagementException">Raised when there is a problem
		/// with the WMI <paramref name="query"/> or with processing one of its result records.</exception>
		public static void QueryForRecords(string query, Action<WmiRecord> recordAction)
		{
			Conditions.RequireString(query, "query");
			Conditions.RequireReference(recordAction, "recordAction");

			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", query))
			{
				foreach (ManagementObject wmiObject in searcher.Get())
				{
					using (WmiRecord record = new WmiRecord(wmiObject))
					{
						recordAction(record);
					}
				}
			}
		}

		/// <summary>
		/// Tries to execute a WMI query and invoke an action for each result record.
		/// </summary>
		/// <param name="query">The WMI query to execute.</param>
		/// <param name="recordAction">The action to invoke for each record.</param>
		/// <returns>True if the query and all records were processed successfully.
		/// False if the query execution or any of the record processing caused a
		/// <see cref="ManagementException"/>.</returns>
		public static bool TryQueryForRecords(string query, Action<WmiRecord> recordAction)
		{
			Conditions.RequireString(query, "query");
			Conditions.RequireReference(recordAction, "recordAction");

			bool result = true;
			if (!TryAction(() =>
				{
					using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", query))
					{
						foreach (ManagementObject wmiObject in searcher.Get())
						{
							using (WmiRecord record = new WmiRecord(wmiObject))
							{
								if (!TryAction(() => recordAction(record)))
								{
									result = false;
								}
							}
						}
					}
				}))
			{
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Tries an action and returns false if a <see cref="ManagementException"/> occurs.
		/// </summary>
		/// <param name="action">The action to try.</param>
		/// <returns>True if the action finished successfully.  False if a ManagementException occurred.</returns>
		public static bool TryAction(Action action)
		{
			Conditions.RequireReference(action, "action");

			bool result = true;
			try
			{
				action();
			}
			catch (ManagementException ex)
			{
				// A variety of things can cause a WMI exception (e.g., if the WMI service is turned off
				// of if the user doesn't have permission to access the properties).  If that happens,
				// then we'll pretend like the WMI action returned no data.
				Log.Error(typeof(WmiUtility), "An error occurred trying a WMI action.", ex);
				result = false;
			}

			return result;
		}

		#endregion
	}
}
