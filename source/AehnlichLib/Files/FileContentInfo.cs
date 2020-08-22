namespace AehnlichLib.Files
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Implements a text content container class for the text, lines an other
	/// related content specific to computing diffs from texts.
	/// </summary>
	public class FileContentInfo
	{
		#region constructors
		/// <summary>Class constructor</summary>
		/// <param name="isBinary">Whether result should be rendered as binary comparison with a bytewise view of the file content.</param>
		/// <param name="lines"></param>
		/// <param name="filePath"></param>
		public FileContentInfo(bool isBinary, IList<string> lines, string filePath)
			: this(isBinary, lines)
		{
			FilePath = filePath;
		}

		/// <summary>Class constructor</summary>
		/// <param name="isBinary"></param>
		/// <param name="lines"></param>
		public FileContentInfo(bool isBinary, IList<string> lines)
			: this()
		{
			TextContent = null;
			Lines = lines;
		}
		/// <summary>Class constructor</summary>
		/// <param name="filePath"></param>
		public FileContentInfo(string filePath)
			: this()
		{
			// Just use a different constructor if a valid path was not required.
			if (string.IsNullOrEmpty(filePath))
				throw new NotSupportedException("This constructor should not be used without valid path.");

			FilePath = filePath;
		}

		/// <summary>Class constructor</summary>
		public FileContentInfo()
		{
			IsBinary = false;
			TextEncoding = Encoding.Default;
			TextContent = string.Empty;
			Lines = new List<string>();
		}
		#endregion constructors

		#region properties
		/// <summary>Gets the detected encoding of the text file content</summary>
		public Encoding TextEncoding { get; set; }

		/// <summary>Gets the text file content</summary>
		public string TextContent { get; set; }

		public IList<string> Lines { get; set; }

		/// <summary>Gets the full path to file A</summary>
		public string FilePath { get; }

		/// <summary>Get whether the content should be treated as binary or not.</summary>
		public bool IsBinary { get; }

		/// <summary>Gets/sets whether the currently shown text in the textedior has been changed
		/// without saving or not.</summary>
		public bool IsDirty { get; set; }
		#endregion properties
	}
}
