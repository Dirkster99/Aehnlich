namespace AehnlichLib.Text
{
	using AehnlichLib.Binaries;
	using AehnlichLib.Dir;
	using AehnlichLib.Enums;
	using AehnlichLib.Files;
	using AehnlichLib.Interfaces;
	using AehnlichLib.Models;
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
		}
		#endregion ctors

		#region properties
		public EditScript Script { get; protected set; }

		public IList<string> ListA { get; private set; }

		public IList<string> ListB { get; private set; }

		public bool IsBinaryCompare { get; private set; }

		public bool IgnoreCase { get; private set; }

		public bool IgnoreTextWhitespace { get; private set; }
		#endregion properties

		#region methods
		public IDiffProgress ProcessDiff(IDiffProgress progress)
		{
			progress.ShowIndeterminatedProgress();

			try
			{
				DiffBinaryTextResults result = null;

				if (_Args.DiffType == DiffType.File)
				{
					var fileA = new FileCompInfo(_Args.A);
					var fileB = new FileCompInfo(_Args.B);

					result = GetFileLines(fileA, fileB, _Args, progress);
				}
				else
				{
					// DiffType Text
					result = GetTextLines(_Args.A, _Args.B, _Args, progress);
				}

				IsBinaryCompare = result.IsBinaryCompare;
				IgnoreCase = result.IgnoreCase;
				IgnoreTextWhitespace = result.IgnoreTextWhitespace;
				TextDiff diff = new TextDiff(_Args.HashType, IgnoreCase, IgnoreTextWhitespace,
											 result.LeadingCharactersToIgnore, !_Args.ShowChangeAsDeleteInsert);

				ListA = result.a;
				ListB = result.b;
				Script = diff.Execute(ListA, ListB, progress);

				progress.ResultData = this;

				return progress;
			}
			finally
			{
				progress.ProgressDisplayOff();
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
		private DiffBinaryTextResults GetFileLines(FileCompInfo fileA
													,FileCompInfo fileB
													,TextBinaryDiffArgs args
													,IDiffProgress progress)
		{
			DiffBinaryTextResults result = new DiffBinaryTextResults(false);
			result.a = null;
			result.b = null;
			CompareType compareType = args.CompareType;

			// Nothing to compare if both files do not exist
			if (fileA.FileExists == false && fileB.FileExists == false)
			{
				result.a = new List<string>();
				result.b = new List<string>();
				return result;
			}

			if (compareType == CompareType.Binary ||
				(args.IsAuto && fileA.Is == FileType.Binary || fileB.Is == FileType.Binary))
			{
				return GetBinaryFileLines(fileA, fileB, args, progress);
			}

			if (compareType == CompareType.Xml || (args.IsAuto && (result.a == null || result.b == null)))
			{
				result.a = fileA.TryGetXmlLines(DiffUtility.GetXmlTextLines, !args.IsAuto, args, progress);

				// If A failed to parse with Auto, then there's no reason to try B.
				if (fileA.Is == FileType.Xml)
					result.b = fileB.TryGetXmlLines(DiffUtility.GetXmlTextLines, !args.IsAuto, args, progress);

				// If we get here and the compare type was XML, then both
				// inputs parsed correctly, and both lists should be non-null.
				// If we get here and the compare type was Auto, then one
				// or both lists may be null, so we'll fallthrough to the text
				// handling logic.
			}

			if (fileA.Is != FileType.Xml || fileB.Is != FileType.Xml)
			{
				if (fileA.FileExists)
					result.a = AsyncPump.Run(() => FileEx.GetFileTextLinesAsync(fileA.FileNamePath));
				else
					result.a = new List<string>();

				if (fileB.FileExists)
					result.b = AsyncPump.Run(() => FileEx.GetFileTextLinesAsync(fileB.FileNamePath));
				else
					result.b = new List<string>();
			}

			return result;
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
		private DiffBinaryTextResults GetBinaryFileLines(FileCompInfo fileA, FileCompInfo fileB
														,TextBinaryDiffArgs args
														,IDiffProgress progress)
		{
			DiffBinaryTextResults result = new DiffBinaryTextResults(true);
			result.a = new List<string>();
			result.b = new List<string>();
			result.LeadingCharactersToIgnore = BinaryDiffLines.PrefixLength;

			// Neither left nor right file exist or cannot be accessed
			if (fileA.FileExists == false && fileB.FileExists == false)
				return result;

			Stream fileStreamA = null, fileStreamB = null;

			try
			{
				// Open the file or an internal empty stream to compare against
				if (fileA.FileExists)
					fileStreamA = File.OpenRead(fileA.FileNamePath);
				else
					fileStreamA = Assembly.GetExecutingAssembly().GetManifestResourceStream("AehnlichLib.Binaries.Resources.NonExistingFile.bin");

				// Open the file or an internal empty stream to compare against
				if (fileB.FileExists)
					fileStreamB = File.OpenRead(fileB.FileNamePath);
				else
					fileStreamB = Assembly.GetExecutingAssembly().GetManifestResourceStream("AehnlichLib.Binaries.Resources.NonExistingFile.bin");

				BinaryDiff diff = new BinaryDiff
				{
					FootprintLength = args.BinaryFootprintLength
				};

				AddCopyCollection addCopy = diff.Execute(fileStreamA, fileStreamB, progress);

				BinaryDiffLines lines = new BinaryDiffLines(fileStreamA, addCopy, args.BinaryFootprintLength);
				result.a = lines.BaseLines;
				result.b = lines.VersionLines;
			}
			finally
			{
				if (fileStreamA != null)
					fileStreamA.Dispose();

				if (fileStreamB != null)
					fileStreamB.Dispose();
			}

			return result;
		}

		private DiffBinaryTextResults GetTextLines(string textA, string textB
												, TextBinaryDiffArgs args
												, IDiffProgress progress)
		{
			DiffBinaryTextResults result = new DiffBinaryTextResults(false);
			result.a = null;
			result.b = null;
			CompareType compareType = args.CompareType;
			bool isAuto = compareType == CompareType.Auto;

			if (compareType == CompareType.Xml || isAuto)
			{
				result.a = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the left side text", textA, !isAuto, args, progress);

				// If A failed to parse with Auto, then there's no reason to try B.
				if (result.a != null)
				{
					result.b = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the right side text", textB, !isAuto, args, progress);
				}

				// If we get here and the compare type was XML, then both
				// inputs parsed correctly, and both lists should be non-null.
				// If we get here and the compare type was Auto, then one
				// or both lists may be null, so we'll fallthrough to the text
				// handling logic.
			}

			if (result.a == null || result.b == null)
			{
				result.a = DiffUtility.GetStringTextLines(textA, progress);
				result.b = DiffUtility.GetStringTextLines(textB, progress);
			}

			return result;
		}

		private static IList<string> TryGetXmlLines(
			Func<string, bool, IDiffProgress, IList<string>> converter,
			string name,
			string input,
			bool throwOnError,
			TextBinaryDiffArgs args,
			IDiffProgress progress)
		{
			IList<string> result = null;
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
		#endregion TextLineConverter
		#endregion methods
	}
}
