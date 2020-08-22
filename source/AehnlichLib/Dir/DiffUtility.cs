namespace AehnlichLib.Dir
{
	using AehnlichLib.Files;
	using AehnlichLib.Interfaces;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Provides helper routines for working with files.
	/// </summary>
	public static class DiffUtility
	{
		#region Public Members
		public static IList<string> GetFileTextLines(string fileName, IDiffProgress progress)
		{
			using (StreamReader reader = new StreamReader(fileName, Encoding.Default, true))
			{
				return GetTextLines(reader, progress);
			}
		}

		public static IList<string> GetStringTextLines(string text,
													   IDiffProgress progress)
		{
			using (StringReader reader = new StringReader(text))
			{
				return GetTextLines(reader, progress);
			}
		}

		public static IList<string> GetTextLines(TextReader reader,
												 IDiffProgress progress)
		{
			IList<string> result = new List<string>();

			try
			{
				while (reader.Peek() > -1)
				{
					progress.Token.ThrowIfCancellationRequested();

					string line = reader.ReadLine();
					result.Add(line);
				}
			}
			catch
			{
				// Not catching this but returning at default empty list
			}

			return result;
		}

		public static IList<string> GetXmlTextLines(string fileName,
													bool ignoreInsignificantWhiteSpace,
													IDiffProgress progress)
		{
			using (StreamReader reader = new StreamReader(fileName, Encoding.Default, true))
			{
				return GetXmlTextLines(reader, ignoreInsignificantWhiteSpace, progress);
			}
		}

		/// <summary>
		/// Reads the text content of a file and determines its Encoding.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="ignoreInsignificantWhiteSpace"></param>
		/// <param name="progress"></param>
		/// <returns></returns>
		public static FileContentInfo GetXmlText(string fileName,
												bool ignoreInsignificantWhiteSpace,
												IDiffProgress progress)
		{
			var contentInfo = new FileContentInfo();

//// This should be created from in-memory text to save IO and support editing
////			using (StreamReader reader = new StreamReader(fileName, Encoding.Default, true))
////			{
////				contentInfo.Lines = GetXmlTextLines(reader, ignoreInsignificantWhiteSpace, progress);
////			}

			// Read the RAW text content
			const int DefaultBufferSize = 4096;
			const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

			using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
			{
				var bom = new byte[4];               // Decode bom (if any) and continue to read text content
				stream.Read(bom, 0, 4);
				stream.Seek(0, SeekOrigin.Begin);

				contentInfo.TextEncoding = FileEx.GetEncoding(bom);

				using (StreamReader reader = new StreamReader(stream, contentInfo.TextEncoding))
				{
					contentInfo.TextContent = reader.ReadToEnd();
				}
			}

			return contentInfo;
		}

		public static IList<string> GetXmlTextLines(XmlReader reader, IDiffProgress progress)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				CheckCharacters = false,
				CloseOutput = true,
				Indent = true,
				IndentChars = "\t",
				NewLineOnAttributes = true
			};

			StringBuilder sb = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(sb, settings))
			{
				writer.WriteNode(reader, false);
			}

			IList<string> result = GetStringTextLines(sb.ToString(), progress);
			return result;
		}

		public static IList<string> GetXmlTextLinesFromXml(string xml,
														   bool ignoreInsignificantWhiteSpace,
														   IDiffProgress progress)
		{
			using (StringReader reader = new StringReader(xml))
			{
				return GetXmlTextLines(reader, ignoreInsignificantWhiteSpace, progress);
			}
		}

		public static FileContentInfo GetXmlTextFromXml(string xml,
														bool ignoreInsignificantWhiteSpace,
														IDiffProgress progress)
		{
			var result = new FileContentInfo();

			result.TextContent = xml;

			using (StringReader reader = new StringReader(xml))
			{
				result.Lines = GetXmlTextLines(reader, ignoreInsignificantWhiteSpace, progress);
			}

			return result;
		}

		#endregion

		#region Private Methods

		private static IList<string> GetXmlTextLines(TextReader textReader,
													 bool ignoreInsignificantWhitespace,
													 IDiffProgress progress)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				CheckCharacters = false,
				CloseInput = true,
				DtdProcessing = DtdProcessing.Ignore,
				IgnoreWhitespace = ignoreInsignificantWhitespace,
				ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None,
				ValidationType = ValidationType.None,
				XmlResolver = null
			};

			using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
			{
				IList<string> result = GetXmlTextLines(xmlReader, progress);
				return result;
			}
		}

		#endregion
	}
}
