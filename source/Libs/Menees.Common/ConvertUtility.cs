namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.ExceptionServices;
	using System.Runtime.Remoting.Metadata.W3cXsd2001;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods for converting data types from one form to another.
	/// </summary>
	public static class ConvertUtility
	{
		#region Private Data Members

		private static readonly HashSet<string> FalseValues = new HashSet<string>(
			new string[] { "false", "f", "no", "n", "0" }, StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> TrueValues = new HashSet<string>(
			new string[] { "true", "t", "yes", "y", "1" }, StringComparer.OrdinalIgnoreCase);

		#endregion

		#region Public Methods

		/// <summary>
		/// Converts a value into the specified type.
		/// </summary>
		/// <typeparam name="T">The type to convert the value into.</typeparam>
		/// <param name="value">The value to convert.</param>
		/// <returns>The converted value.</returns>
		/// <exception cref="InvalidCastException">If <paramref name="value"/>
		/// can't be converted into type T.</exception>
		public static T ConvertValue<T>(object value)
		{
			T result;

			// We can't do a C# "as" check because T may not be a reference type.
			// There's a small amount of duplicate work done by "is + Cast" (because
			// .NET will do the "is" check again during the cast), but it's unavoidable
			// in this case because we're using generics with no class constraint.
			if (value is T)
			{
				result = (T)value;
			}
			else
			{
				result = (T)ConvertValue(value, typeof(T));
			}

			return result;
		}

		/// <summary>
		/// Converts a value into the specified type.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="resultType">The type to convert the value into.</param>
		/// <returns>The converted value.</returns>
		/// <exception cref="InvalidCastException">If <paramref name="value"/>
		/// can't be converted into type T.</exception>
		public static object ConvertValue(object value, Type resultType)
		{
			Conditions.RequireReference(resultType, "resultType");

			bool converted = false;
			object result = null;

			if (value != null)
			{
				// Try using a TypeConverter instead of Convert.ChangeType because TypeConverter supports
				// a lot more types (including enums and nullable types).  Convert.ChangeType only supports
				// IConvertible (see http://aspalliance.com/852).
				TypeConverter converter = TypeDescriptor.GetConverter(resultType);
				if (converter.CanConvertFrom(value.GetType()))
				{
					try
					{
						object fromTypeConverter = converter.ConvertFrom(value);
						result = fromTypeConverter;
						converted = true;
					}
					catch (Exception ex)
					{
						// .NET's System.ComponentModel.BaseNumberConverter.ConvertFrom method catches
						// all exceptions and then rethrows a System.Exception, which is HORRIBLE!  We'll try to
						// undo Microsoft's mistake by re-throwing the original exception, so callers can catch
						// specific exception types.
						Exception inner = ex.InnerException;
						if (inner != null)
						{
							ExceptionDispatchInfo.Capture(inner).Throw();
						}

						throw;
					}
				}
			}
			else if (!resultType.IsValueType || Nullable.GetUnderlyingType(resultType) != null)
			{
				// http://stackoverflow.com/questions/374651/how-to-check-if-an-object-is-nullable
				result = null;
				converted = true;
			}

			if (!converted)
			{
				// The value was null (for a value type) or the type converter couldn't convert it,
				// so we have to fall back to ChangeType.
				result = Convert.ChangeType(value, resultType);
			}

			return result;
		}

		/// <summary>
		/// Gets whether a value is a null reference or DBNull.Value.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>True if the value is null or DBNull.Value.  False otherwise.</returns>
		public static bool IsNull(object value)
		{
			bool result = value == null || value == DBNull.Value;
			return result;
		}

		/// <summary>
		/// Converts a string value to a bool.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>True if the string case-insensitively matches "True", "T", "Yes", "Y", or "1".  False otherwise.</returns>
		public static bool ToBoolean(string value)
		{
			bool result = TrueValues.Contains(value);
			return result;
		}

		/// <summary>
		/// Converts a string value to a bool.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <param name="defaultValue">The default value to use if the string can't be converted.</param>
		/// <returns>
		/// True if the string case-insensitively matches "True", "T", "Yes", "Y", or "1".
		/// False if the string case-insensitively matches "False", "F", "No", "N", or "0".
		/// If the value is not one of the true values and not one of the false values,
		/// then this returns the defaultValue.  For example, ToBoolean("P", true) will return true,
		/// and ToBoolean("Q", false) will return false.
		/// </returns>
		public static bool ToBoolean(string value, bool defaultValue)
		{
			bool result = defaultValue;

			if (TrueValues.Contains(value))
			{
				result = true;
			}
			else
			{
				if (FalseValues.Contains(value))
				{
					result = false;
				}
			}

			return result;
		}

		/// <summary>
		/// Converts a string value to an int.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <param name="defaultValue">The default value to use if the string can't be converted.</param>
		/// <returns>The int value.</returns>
		public static int ToInt32(string value, int defaultValue)
		{
			int result = defaultValue;

			int parsed;
			if (int.TryParse(value, out parsed))
			{
				result = parsed;
			}

			return result;
		}

		#endregion

		#region Internal Methods

		internal static T GetValue<T>(string textValue, T defaultValue)
			where T : struct
		{
			T result = defaultValue;
			if (!string.IsNullOrEmpty(textValue))
			{
				Type type = typeof(T);

				// Enums are the most common case, so handle them specially.
				if (type.IsEnum)
				{
					T parsedValue;
					if (Enum.TryParse<T>(textValue, out parsedValue))
					{
						result = parsedValue;
					}
				}
				else
				{
					// Handle other simple types like Double and DateTime along with types
					// that support TypeConverters (e.g., System.Windows.Forms.Color).
					try
					{
						result = ConvertValue<T>(textValue);
					}
					catch (Exception ex)
					{
						if (!(ex is ArgumentException || ex is ArithmeticException || ex is FormatException || ex is IndexOutOfRangeException))
						{
							throw;
						}
					}
				}
			}

			return result;
		}

		#endregion
	}
}
