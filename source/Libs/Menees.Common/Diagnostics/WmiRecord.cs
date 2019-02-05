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
	/// Represents a single result record for a WMI query executed by
	/// <see cref="WmiUtility.QueryForRecords"/> or <see cref="WmiUtility.TryQueryForRecords"/>.
	/// </summary>
	/// <devnote>
	/// This class doesn't implement IDataRecord because that requires integer index accessors
	/// by ordinal, whereas WMI objects only expose string index accessors by property name.
	/// </devnote>
	public sealed class WmiRecord : IDisposable
	{
		#region Private Data Members

		private ManagementObject wmiObject;

		#endregion

		#region Constructors

		// Note: This is internal so callers won't have to reference the System.Management.dll.
		internal WmiRecord(ManagementObject wmiObject)
		{
			this.wmiObject = wmiObject;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the WMI object that this record is using.
		/// </summary>
		public object WrappedObject => this.wmiObject;

		/// <summary>
		/// Gets the specified property's value by calling <see cref="GetValue"/>.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public object this[string propertyName]
		{
			get
			{
				object result = this.GetValue(propertyName);
				return result;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Disposes of any associated WMI resources.
		/// </summary>
		public void Dispose()
		{
			this.wmiObject.Dispose();
		}

		/// <summary>
		/// Gets the specified property's value as a bool.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public bool GetBoolean(string propertyName)
		{
			object value = this.GetRawValue(propertyName);
			string text = value as string;

			// ConvertUtility.ToBoolean handles more text values than the standard .NET conversion method.
			bool result = text != null ? ConvertUtility.ToBoolean(text) : Convert.ToBoolean(value);
			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a nullable bool.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public bool? GetBooleanN(string propertyName)
		{
			bool? result = null;

			object value = this.GetRawValue(propertyName);
			if (value != null)
			{
				string text = value as string;

				// ConvertUtility.ToBoolean handles more text values than the standard .NET conversion method.
				result = text != null ? ConvertUtility.ToBoolean(text) : Convert.ToBoolean(value);
			}

			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a DateTime.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public DateTime GetDateTime(string propertyName)
		{
			string value = this.GetString(propertyName);
			DateTime result = ManagementDateTimeConverter.ToDateTime(value);
			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a nullable DateTime.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public DateTime? GetDateTimeN(string propertyName)
		{
			DateTime? result = null;

			string value = this.GetString(propertyName);
			if (!string.IsNullOrEmpty(value))
			{
				result = ManagementDateTimeConverter.ToDateTime(value);
			}

			return result;
		}

		/// <summary>
		/// Gets the specified property's value as an int.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public int GetInt32(string propertyName)
		{
			object value = this.GetRawValue(propertyName);
			int result = Convert.ToInt32(value);
			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a nullable int.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public int? GetInt32N(string propertyName)
		{
			int? result = null;

			object value = this.GetRawValue(propertyName);
			if (value != null)
			{
				result = Convert.ToInt32(value);
			}

			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a long.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public long GetInt64(string propertyName)
		{
			object value = this.GetRawValue(propertyName);
			long result = Convert.ToInt64(value);
			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a nullable long.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public long? GetInt64N(string propertyName)
		{
			long? result = null;

			object value = this.GetRawValue(propertyName);
			if (value != null)
			{
				result = Convert.ToInt64(value);
			}

			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a string.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public string GetString(string propertyName)
		{
			string result = null;

			object value = this.GetRawValue(propertyName);
			if (value != null)
			{
				result = Convert.ToString(value);
			}

			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a TimeSpan.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public TimeSpan GetTimeSpan(string propertyName)
		{
			string value = this.GetString(propertyName);
			TimeSpan result = ManagementDateTimeConverter.ToTimeSpan(value);
			return result;
		}

		/// <summary>
		/// Gets the specified property's value as a nullable TimeSpan.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <returns>The value of the specified property.</returns>
		public TimeSpan? GetTimeSpanN(string propertyName)
		{
			TimeSpan? result = null;

			string value = this.GetString(propertyName);
			if (!string.IsNullOrEmpty(value))
			{
				result = ManagementDateTimeConverter.ToTimeSpan(value);
			}

			return result;
		}

		/// <summary>
		/// Gets the specified property's value in its most applicable .NET format.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <remarks>
		/// Some values, such as dates and times, have native WMI formats different
		/// from their typical .NET formats.  This method will attempt to detect native
		/// WMI property values and convert them to their expected .NET types.
		/// </remarks>
		/// <returns>The value of the specified property in a .NET format.</returns>
		public object GetValue(string propertyName)
		{
			object result = this.GetRawValue(propertyName);
			if (result != null)
			{
				string text = result as string;
				if (text != null)
				{
					// If we have a string property, we need to look at the property metadata
					// to determine if it should be interpreted as a DateTime or a TimeSpan.
					PropertyData data = this.wmiObject.Properties[propertyName];
					switch (data.Type)
					{
						// The "interval" format is a sub-type of the CIM_DATETIME format, which is a
						// crazy format that can contain asterisks in many places for optional time portions.
						// CIM_DATETIME (Windows) - http://msdn.microsoft.com/en-us/library/windows/desktop/aa387237(v=vs.85).aspx
						// Interval Format (Windows) - http://msdn.microsoft.com/en-us/library/windows/desktop/aa390895(v=vs.85).aspx
						case CimType.DateTime:
							// The Qualifiers have to be matched case-insensitively since the names seem to be all uppercase at run-time.
							// Standard WMI Qualifiers (Windows) - http://msdn.microsoft.com/en-us/library/windows/desktop/aa393651(v=vs.85).aspx
							const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
							bool isInterval = data.Qualifiers.Cast<QualifierData>().Any(
								q => string.Equals(q.Name, "SubType", Comparison) && string.Equals(q.Value as string, "interval", Comparison));
							if (isInterval)
							{
								result = ManagementDateTimeConverter.ToTimeSpan(text);
							}
							else
							{
								result = ManagementDateTimeConverter.ToDateTime(text);
							}

							break;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Executes a method on the current record using the specified parameters.
		/// </summary>
		/// <param name="methodName">The name of a method to invoke.</param>
		/// <param name="parameters">The method parameters.</param>
		/// <returns>The object value returned by the method.</returns>
		public object InvokeMethod(string methodName, object[] parameters)
		{
			object result = this.wmiObject.InvokeMethod(methodName, parameters);
			return result;
		}

		/// <summary>
		/// Executes a method on the current record using the specified input parameters.
		/// </summary>
		/// <param name="methodName">The name of a method to invoke.</param>
		/// <param name="inputParameters">A dictionary of input parameter names and values.
		/// This can be null if the method takes no parameters.</param>
		/// <returns>A dictionary of output parameter names and values.</returns>
		public IDictionary<string, object> InvokeMethod(string methodName, IDictionary<string, object> inputParameters)
		{
			ManagementBaseObject wmiInputParameters = null;
			if (inputParameters != null)
			{
				wmiInputParameters = this.wmiObject.GetMethodParameters(methodName);
				foreach (var pair in inputParameters)
				{
					wmiInputParameters[pair.Key] = pair.Value;
				}
			}

			ManagementBaseObject wmiOutputParameters = this.wmiObject.InvokeMethod(methodName, wmiInputParameters, null);
			Dictionary<string, object> result = new Dictionary<string, object>(wmiOutputParameters.Properties.Count);
			foreach (var property in wmiOutputParameters.Properties)
			{
				result[property.Name] = property.Value;
			}

			return result;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the specified property's value in its native WMI format.
		/// </summary>
		/// <param name="propertyName">The name of a property.</param>
		/// <remarks>
		/// Some values, such as dates and times, have native WMI formats different
		/// from their typical .NET formats.  This method will not do any conversion.
		/// However, the <see cref="GetValue"/> method and the typed accessor methods
		/// (e.g., <see cref="GetDateTime"/>) will convert the WMI values to their expected
		/// .NET types.
		/// </remarks>
		/// <returns>The raw/native value of the specified property.</returns>
		private object GetRawValue(string propertyName)
		{
			object result = this.wmiObject.GetPropertyValue(propertyName);
			return result;
		}

		#endregion
	}
}
