namespace AehnlichLib.Text
{
	using AehnlichLib.Binaries;
	using AehnlichLib.Dir;
	using AehnlichLib.Enums;
	using AehnlichLib.Files;
	using AehnlichLib.Interfaces;
	using AehnlichLib.Models;
	using FsDataLib.Interfaces.Dir;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Xml;

	public class ProcessTextDiff
	{
		#region fields
		private readonly TextBinaryDiffArgs _Args;
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		public ProcessTextDiff(TextBinaryDiffArgs args)
			: this()
		{
			_Args = args;
		}

		/// <summary>
		/// Hidden class constructor
		/// </summary>
		protected ProcessTextDiff()
		{
			TextContentA = TextContentB = null;
			TextEncodingA = Encoding.Default;
			TextEncodingB = Encoding.Default;
		}
		#endregion ctors

		#region properties
		/// <summary>
		/// Gets an edit script object that describes differences between <see cref="ListA"/> and <see cref="ListB"/>
		/// by stating edits (update, insert, delete) that are necessary to transform one list into the other.
		/// </summary>
		public EditScript Script { get; protected set; }

		/// <summary>Gets a list of lines (text or binary rendered as text) for the left side of the comparison.</summary>
		public IList<string> ListA { get; private set; }
		public string TextOriginalA { get; private set; }

		/// <summary>Gets the original text for the left side of the comparison.</summary>
		public string TextContentA { get; private set; }

		/// <summary>Gets the original text encoding for the left side of the comparison.</summary>
		public Encoding TextEncodingA { get; private set; }

		/// <summary>Gets/sets whether Text A has been changed without saving to disk or not.</summary>
		public bool TextIsDirtyA { get; private set; }

		public string TextOriginalB { get; private set; }

		/// <summary>Gets a list of lines (text or binary rendered as text) for the right side of the comparison.</summary>
		public IList<string> ListB { get; private set; }

		/// <summary>Gets the original text for the right side of the comparison.</summary>
		public string TextContentB { get; private set; }

		/// <summary>Gets the original text encoding for the right side of the comparison.</summary>
		public Encoding TextEncodingB { get; private set; }

		/// <summary>Gets/sets whether Text B has been changed without saving to disk or not.</summary>
		public bool TextIsDirtyB { get; private set; }

		/// <summary>Gets whether the returned data was interpreted as binary, text, or XML.</summary>
		public CompareType IsComparedAs { get; private set; }

		public bool IgnoreCase { get; private set; }

		public bool IgnoreTextWhitespace { get; private set; }
		#endregion properties

		#region methods
		/// <summary>Compare to files or string contents and return result via progress object.</summary>
		/// <param name="progress"></param>
		/// <param name="dataSource"></param>
		/// <returns></returns>
		public IDiffProgress ProcessDiff(IDiffProgress progress, IDataSource dataSource)
		{
			try
			{
				DiffBinaryTextResults result = null;

				if (_Args.DiffType == DiffType.File)
				{
					var fileA = dataSource.CreateFile(_Args.A);
					var fileB = dataSource.CreateFile(_Args.B);

					result = GetFileLines(fileA, fileB, _Args, progress);
				}
				else
				{
					// DiffType in-memory text content
					result = GetTextLines(TextContentA, TextContentB, _Args, progress);
				}

				if (result.IsComparedAs == CompareType.Text || result.IsComparedAs == CompareType.Xml)
				{
					// Render Text or XML lines from in-memory text content
					if (_Args.DiffType == DiffType.File)
						result = GetTextLines(result, _Args, progress);

					// Assumption: Binary cannot be edit and RAW data cannot be stored in string
					//             Therefore, binary lines are rendered only once directly from file
					TextOriginalA = result.A.TextContent;
					TextContentA = result.A.TextContent;
					TextEncodingA = result.A.TextEncoding;

					TextOriginalB = result.B.TextContent;
					TextContentB = result.B.TextContent;
					TextEncodingB = result.B.TextEncoding;

				}

				ListA = result.A.Lines;
				ListB = result.B.Lines;

				IsComparedAs = result.IsComparedAs;
				IgnoreCase = result.IgnoreCase;
				IgnoreTextWhitespace = result.IgnoreTextWhitespace;

				TextDiff diff = new TextDiff(_Args.HashType, IgnoreCase, IgnoreTextWhitespace,
											 result.LeadingCharactersToIgnore, !_Args.ShowChangeAsDeleteInsert);

				Script = diff.Execute(ListA, ListB, progress);

				progress.ResultData = this;

				return progress;
			}
			finally
			{
			}
		}

		#region TextLineConverter
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileA"></param>
		/// <param name="fileB"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="args"></param>
		/// <param name="progress"></param>
		private DiffBinaryTextResults GetFileLines(IFileInfo fileA
												, IFileInfo fileB
												,TextBinaryDiffArgs args
												,IDiffProgress progress)
		{
			// Nothing to compare if both files do not exist
			if (fileA.FileExists == false && fileB.FileExists == false)
				return new DiffBinaryTextResults(CompareType.Text, new FileContentInfo(), new FileContentInfo());

			if (args.CompareType == CompareType.Binary ||
				(args.IsAuto && fileA.Is == FileType.Binary || fileB.Is == FileType.Binary))
				return GetBinaryFileLines(fileA, fileB, args, progress);

			FileContentInfo af = null, bf = null;

			if (fileA.FileExists)
				af = AsyncPump.Run(() => FileEx.GetFileTextAsync(fileA.FullName));
			else
				af = new FileContentInfo();

			if (fileB.FileExists)
				bf = AsyncPump.Run(() => FileEx.GetFileTextAsync(fileB.FullName));
			else
				bf = new FileContentInfo();

			return new DiffBinaryTextResults(CompareType.Text, af, bf);
		}

		/// <summary>
		/// Get Binary file contents rendered as text lines with line number marker at beginning of each line.
		/// </summary>
		/// <param name="fileA"></param>
		/// <param name="fileB"></param>
		/// <param name="args"></param>
		/// <param name="progress"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="leadingCharactersToIgnore">Leading number of characters to ignore for diff in each line.
		/// This space is used in binary diff to display 8 digit line number and 4 digit space.</param>
		private DiffBinaryTextResults GetBinaryFileLines(IFileInfo fileA, IFileInfo fileB
														,TextBinaryDiffArgs args
														,IDiffProgress progress)
		{
			// Neither left nor right file exist or cannot be accessed
			if (fileA.FileExists == false && fileB.FileExists == false)
				return new DiffBinaryTextResults(CompareType.Binary, new FileContentInfo(), new FileContentInfo());

			Stream fileStreamA = null, fileStreamB = null;
			IList<string> a = null, b = null;

			try
			{
				// Open the file or an internal empty stream to compare against
				if (fileA.FileExists)
					fileStreamA = File.OpenRead(fileA.FullName);
				else
					fileStreamA = Assembly.GetExecutingAssembly().GetManifestResourceStream("AehnlichLib.Binaries.Resources.NonExistingFile.bin");

				// Open the file or an internal empty stream to compare against
				if (fileB.FileExists)
					fileStreamB = File.OpenRead(fileB.FullName);
				else
					fileStreamB = Assembly.GetExecutingAssembly().GetManifestResourceStream("AehnlichLib.Binaries.Resources.NonExistingFile.bin");

				BinaryDiff diff = new BinaryDiff
				{
					FootprintLength = args.BinaryFootprintLength
				};

				AddCopyCollection addCopy = diff.Execute(fileStreamA, fileStreamB, progress);

				BinaryDiffLines lines = new BinaryDiffLines(fileStreamA, addCopy, args.BinaryFootprintLength);
				a = lines.BaseLines;
				b = lines.VersionLines;
			}
			finally
			{
				if (fileStreamA != null)
					fileStreamA.Dispose();

				if (fileStreamB != null)
					fileStreamB.Dispose();
			}

			FileContentInfo af = new FileContentInfo(true, a);
			FileContentInfo bf = new FileContentInfo(true, b);

			return new DiffBinaryTextResults(CompareType.Binary, af, bf, BinaryDiffLines.PrefixLength);
		}

		private DiffBinaryTextResults GetTextLines(string textA, string textB
												, TextBinaryDiffArgs args
												, IDiffProgress progress)
		{
			FileContentInfo af = null, bf = null;

			bool isAuto = args.CompareType == CompareType.Auto;

			if (args.CompareType == CompareType.Xml || isAuto)
			{
				af = TryGetXmlText(DiffUtility.GetXmlTextFromXml, "the left side text", textA, !isAuto, args, progress);

				// If A failed to parse with Auto, then there's no reason to try B.
				if (af != null)
				{
					bf = TryGetXmlText(DiffUtility.GetXmlTextFromXml, "the right side text", textB, !isAuto, args, progress);
				}

				// If we get here and the compare type was XML, then both
				// inputs parsed correctly, and both lists should be non-null.
				// If we get here and the compare type was Auto, then one
				// or both lists may be null, so we'll fallthrough to the text
				// handling logic.
			}

			if (af == null || bf == null)
			{
				af = new FileContentInfo();
				bf = new FileContentInfo();

				af.Lines = DiffUtility.GetStringTextLines(textA, progress);
				bf.Lines = DiffUtility.GetStringTextLines(textB, progress);
			}

			af.TextContent = textA;
			bf.TextContent = textB;
			DiffBinaryTextResults result = new DiffBinaryTextResults(CompareType.Text, af, bf);
			return result;
		}

		private DiffBinaryTextResults GetTextLines(DiffBinaryTextResults src
												, TextBinaryDiffArgs args
												, IDiffProgress progress)
		{
			IList<string> a = null, b = null;

			bool isAuto = args.CompareType == CompareType.Auto;

			if (args.CompareType == CompareType.Xml || isAuto)
			{
				a = TryGetXmlText(DiffUtility.GetXmlTextLinesFromXml, "the left side text", src.A.TextContent, !isAuto, args, progress);

				// If A failed to parse with Auto, then there's no reason to try B.
				if (a != null)
				{
					b = TryGetXmlText(DiffUtility.GetXmlTextLinesFromXml, "the right side text", src.B.TextContent, !isAuto, args, progress);
				}

				// If we get here and the compare type was XML, then both
				// inputs parsed correctly, and both lists should be non-null.
				// If we get here and the compare type was Auto, then one
				// or both lists may be null, so we'll fallthrough to the text
				// handling logic.
			}

			if (a == null || b == null)
			{
				a = DiffUtility.GetStringTextLines(src.A.TextContent, progress);
				b = DiffUtility.GetStringTextLines(src.B.TextContent, progress);
			}
			else
				src.IsComparedAs = CompareType.Xml;

			src.A.Lines = a;
			src.B.Lines = b;

			return src;
		}

		private T TryGetXmlText<T>(
			Func<string, bool, IDiffProgress, T> converter,
			string name,
			string input,
			bool throwOnError,
			TextBinaryDiffArgs args,
			IDiffProgress progress)
		{
			T result = default(T);
			try
			{
				result = converter(input, args.IgnoreXmlWhitespace, progress);
			}
			catch (XmlException ex)
			{
				if (throwOnError)
				{
					StringBuilder sb = new StringBuilder("An XML comparison was attempted, but an XML exception occurred while parsing ");
					sb.Append(name).AppendLine(".").AppendLine();
					sb.AppendLine("Exception Message:").Append(ex.Message);

					throw new XmlException(sb.ToString(), ex);
				}
			}

			return result;
		}

		public void SetupForTextComparison(FileContentInfo fileA, FileContentInfo fileB)
		{
			this.TextOriginalA = fileA.TextContent;
			this.TextContentA = fileA.TextContent;
			this.TextEncodingA = fileA.TextEncoding;
			this.TextIsDirtyA = fileA.IsDirty;

			this.TextOriginalB = fileA.TextContent;
			this.TextContentB = fileB.TextContent;
			this.TextEncodingB = fileB.TextEncoding;
			this.TextIsDirtyB = fileB.IsDirty;
		}
		#endregion TextLineConverter
		#endregion methods
	}
}
