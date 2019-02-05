namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods for working with comma-separated, double-quote delimited data in "CSV" format.
	/// </summary>
	public static class CsvUtility
	{
		#region Public Constants

		/// <summary>
		/// A comma, which is used to separate CSV fields.
		/// </summary>
		public const char FieldSeparator = ',';

		/// <summary>
		/// A double quote, which is used to delimit CSV fields that contain special characters
		/// such as comma, double quote, carriage return, and linefeed.
		/// </summary>
		public const char FieldDelimiter = '"';

		#endregion

		#region Private Data Members

		private static readonly char[] SpecialCharacters = new[] { FieldSeparator, FieldDelimiter, '\r', '\n' };
		private static readonly string FieldDelimiterString = new string(FieldDelimiter, 1);

		#endregion

		#region Public Methods

		/// <summary>
		/// Reads a line from the text reader and returns its field values.
		/// </summary>
		/// <param name="reader">The reader to read the next line from.</param>
		/// <returns>The next line's trimmed field values if a line was read,
		/// or null if no line was available.</returns>
		/// <remarks>
		/// If the line contains a quoted field value with embedded newlines,
		/// then this will keep reading lines until the record is complete.
		/// </remarks>
		[SuppressMessage("", "CC0039", Justification = "Concatenation inside the loop should be extremely rare.")]
		public static IList<string> ReadLine(TextReader reader)
		{
			Conditions.RequireReference(reader, "reader");

			IList<string> result = null;

			string line = reader.ReadLine();
			if (line != null)
			{
				string text = line;
				const int DefaultCapacity = 16;
				result = new List<string>(DefaultCapacity);
				bool properlyTerminated = TextUtility.SplitIntoTokens(text, FieldSeparator, FieldDelimiter, token => result.Add(token.Trim()));
				while (line != null && !properlyTerminated)
				{
					// Add on the next line and see if it is properly terminated.  Note that we can't
					// just remove the last unclosed token and add it to the next line because that
					// token may already have had escaped quotes replaced within it.  For safety, we'll
					// just reparse the entire text data.  This isn't the most efficient implementation,
					// but in practice we shouldn't see data like this very often.
					line = reader.ReadLine();
					if (line != null)
					{
						// We can't tell whether TextReader found "\r\n" or just "\n", so we have to
						// normalize to Environment.NewLine.  If the actual newline makes a difference,
						// then the caller will have to use something else to process the data.
						text += Environment.NewLine + line;
						result.Clear();
						properlyTerminated = TextUtility.SplitIntoTokens(text, FieldSeparator, FieldDelimiter, token => result.Add(token.Trim()));
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Reads a line of text and returns its trimmed field values.
		/// </summary>
		/// <param name="text">The text to parse.</param>
		/// <returns>The line's trimmed field values.</returns>
		public static IList<string> ReadLine(string text) => ReadLine(text, true);

		/// <summary>
		/// Reads a line of text and returns its field values.
		/// </summary>
		/// <param name="text">The text to parse.</param>
		/// <param name="trimValues">Whether to trim each field value.</param>
		/// <returns>The line's field values.</returns>
		public static IList<string> ReadLine(string text, bool trimValues)
		{
			IList<string> result = TextUtility.SplitIntoTokens(text, FieldSeparator, FieldDelimiter, trimValues);
			return result;
		}

		/// <summary>
		/// Loads a data table from the specified file.
		/// </summary>
		/// <remarks>
		/// The first row is assumed to be a header row.
		/// </remarks>
		/// <param name="fileName">The file to load from.</param>
		/// <returns>
		/// If the file is non-empty, then this returns a new data table with the file's data loaded.
		/// If the file is empty, then this returns null.
		/// </returns>
		public static DataTable ReadTable(string fileName)
		{
			Conditions.RequireString(fileName, "fileName");

			using (StreamReader reader = new StreamReader(fileName))
			{
				DataTable result = ReadTable(reader, null);
				return result;
			}
		}

		/// <summary>
		/// Loads a data table from the specified reader.
		/// </summary>
		/// <remarks>
		/// The first row is assumed to be a header row.
		/// </remarks>
		/// <param name="reader">The text reader to load from.</param>
		/// <returns>
		/// If data is found, then this returns a new data table with the reader's data loaded.
		/// If no data is found, then this returns null.
		/// </returns>
		public static DataTable ReadTable(TextReader reader) => ReadTable(reader, null);

		/// <summary>
		/// Loads a data table from the specified reader and optionally allows
		/// adjusting the table's columns prior to loading.
		/// </summary>
		/// <remarks>
		/// The first row is assumed to be a header row.
		/// </remarks>
		/// <param name="reader">The text reader to load from.</param>
		/// <param name="prepareToLoad">An optional action that is called after
		/// the table's columns are added (as string columns) but before any data is
		/// loaded.  This allows the column types to be changed and the table's
		/// primary key to be set.  This parameter can be null.</param>
		/// <returns>
		/// If data is found, then this returns a new data table with the reader's data loaded.
		/// If no data is found, then this returns null.
		/// </returns>
		public static DataTable ReadTable(TextReader reader, Action<DataTable> prepareToLoad)
		{
			Conditions.RequireReference(reader, "reader");

			DataTable result = null;
			DataRowCollection rows = null;
			int columnCount = 0;
			IList<string> values;
			while ((values = ReadLine(reader)) != null)
			{
				// The first line should be a header row.
				if (result == null)
				{
					result = new DataTable();
					rows = result.Rows;
					DataColumnCollection columns = result.Columns;
					foreach (string name in values)
					{
						columns.Add(name, typeof(string));
					}

					if (prepareToLoad != null)
					{
						prepareToLoad(result);
					}

					// prepareToLoad might have changed the column count.
					columnCount = result.Columns.Count;
				}
				else
				{
					if (values.Count > columnCount)
					{
						using (StringWriter writer = new StringWriter())
						{
							WriteLine(writer, values);
							throw Exceptions.NewArgumentExceptionFormat(
								"The value count ({0}) should not exceed the column count ({1}).  Invalid line: {2}",
								values.Count,
								columnCount,
								writer.ToString());
						}
					}

					int columnIndex = 0;
					DataRow row = result.NewRow();
					foreach (string value in values)
					{
						// Treat an empty string as null because that's usually the desired behavior.
						// Note: If the columns are still string columns (e.g., if prepareToLoad is null),
						// then DataRow will treat null and DBNull the same.  However, for any
						// ValueType column (e.g., int, decimal), we must use DBNull.
						row[columnIndex++] = string.IsNullOrEmpty(value) ? (object)DBNull.Value : value;
					}

					rows.Add(row);
				}
			}

			return result;
		}

		/// <summary>
		/// Writes a value and delimits it if necessary.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="value">The value to write.</param>
		public static void WriteValue(TextWriter writer, string value)
		{
			Conditions.RequireReference(writer, "writer");
			WriteValueCore(writer, value);
		}

		/// <summary>
		/// Writes a line of values to a text writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="values">The values to write.</param>
		public static void WriteLine(TextWriter writer, IEnumerable<object> values)
		{
			Conditions.RequireReference(writer, "writer");
			Conditions.RequireReference(values, "values");

			bool firstValue = true;
			foreach (object value in values)
			{
				WriteValue(writer, value, firstValue);
				if (firstValue)
				{
					firstValue = false;
				}
			}

			writer.WriteLine();
		}

		/// <summary>
		/// Writes a line containing a record's field values.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="record">The record to read the values from.</param>
		public static void WriteLine(TextWriter writer, IDataRecord record)
		{
			Conditions.RequireReference(writer, "writer");
			Conditions.RequireReference(record, "record");

			WriteLine(writer, record, record.FieldCount);
		}

		/// <summary>
		/// Writes a line containing a row's field values.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="row">The row to read the values from.</param>
		public static void WriteLine(TextWriter writer, DataRow row)
		{
			Conditions.RequireReference(writer, "writer");
			Conditions.RequireReference(row, "row");

			WriteLine(writer, row, row.Table.Columns.Count);
		}

		/// <summary>
		/// Writes the current result set from a data reader.
		/// </summary>
		/// <remarks>
		/// This will also write a header row using the field names.
		/// </remarks>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="reader">The reader to read from.</param>
		public static void WriteTable(TextWriter writer, IDataReader reader)
		{
			Conditions.RequireReference(writer, "writer");
			Conditions.RequireReference(reader, "reader");

			int fieldCount = reader.FieldCount;
			for (int i = 0; i < fieldCount; i++)
			{
				WriteValue(writer, reader.GetName(i), i == 0);
			}

			writer.WriteLine();

			while (reader.Read())
			{
				WriteLine(writer, reader, fieldCount);
			}
		}

		/// <summary>
		/// Writes a data table.
		/// </summary>
		/// <remarks>
		/// This will also write a header row using the column captions.
		/// </remarks>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="table">The table to read from.</param>
		public static void WriteTable(TextWriter writer, DataTable table)
		{
			Conditions.RequireReference(writer, "writer");
			Conditions.RequireReference(table, "table");

			int columnCount = table.Columns.Count;
			for (int i = 0; i < columnCount; i++)
			{
				WriteValue(writer, table.Columns[i].Caption, i == 0);
			}

			writer.WriteLine();

			foreach (DataRow row in table.Rows)
			{
				WriteLine(writer, row, columnCount);
			}
		}

		/// <summary>
		/// Writes the current result set from a data reader to the specified file.
		/// </summary>
		/// <remarks>
		/// This will also write a header row using the field names.
		/// </remarks>
		/// <param name="fileName">The name of the file to write to.</param>
		/// <param name="reader">The reader to read from.</param>
		public static void WriteTable(string fileName, IDataReader reader)
		{
			Conditions.RequireString(fileName, "fileName");

			// We have to use the default encoding here.  See comments in Write(string, DataTable).
			using (StreamWriter writer = new StreamWriter(fileName, false))
			{
				WriteTable(writer, reader);
			}
		}

		/// <summary>
		/// Writes a data table to the specified file.
		/// </summary>
		/// <remarks>
		/// This will also write a header row using the column captions.
		/// </remarks>
		/// <param name="fileName">The name of the file to write to.</param>
		/// <param name="table">The table to read from.</param>
		public static void WriteTable(string fileName, DataTable table)
		{
			Conditions.RequireString(fileName, "fileName");

			// We have to use the default encoding here because Excel doesn't like byte order marks
			// in CSV files.  For example, using the UTF8 encoding here causes Excel to prompt with
			// its text import wizard when opening the file.
			using (StreamWriter writer = new StreamWriter(fileName, false))
			{
				WriteTable(writer, table);
			}
		}

		#endregion

		#region Private Methods

		private static void WriteValue(TextWriter writer, object value, bool firstValueInLine)
		{
			if (!firstValueInLine)
			{
				writer.Write(FieldSeparator);
			}

			if (!ConvertUtility.IsNull(value))
			{
				string textValue = value as string;
				if (textValue != null)
				{
					// Call the private "Core" method here to skip precondition checking.
					WriteValueCore(writer, textValue);
				}
				else
				{
					writer.Write(value);
				}
			}
		}

		private static void WriteLine(TextWriter writer, IDataRecord record, int fieldCount)
		{
			for (int i = 0; i < fieldCount; i++)
			{
				object value = record[i];
				WriteValue(writer, value, i == 0);
			}

			writer.WriteLine();
		}

		private static void WriteLine(TextWriter writer, DataRow row, int columnCount)
		{
			for (int i = 0; i < columnCount; i++)
			{
				object value = row[i];
				WriteValue(writer, value, i == 0);
			}

			writer.WriteLine();
		}

		private static void WriteValueCore(TextWriter writer, string value)
		{
			string escapedText = value;

			// Excel only quotes text values that need it, so we'll do the same.
			if (!string.IsNullOrEmpty(escapedText) && escapedText.IndexOfAny(SpecialCharacters) >= 0)
			{
				escapedText = FieldDelimiter + value.Replace(FieldDelimiterString, FieldDelimiterString + FieldDelimiterString) + FieldDelimiter;
			}

			writer.Write(escapedText);
		}

		#endregion
	}
}
