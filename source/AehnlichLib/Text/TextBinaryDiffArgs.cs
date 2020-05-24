namespace AehnlichLib.Models
{
	using AehnlichLib.Enums;
	using System.Collections.Generic;

	/// <summary>
	/// Class defines core properties that can be defined as arguments when diffing text or binary files
	/// or text in general (original name ShowDiffArgs).
	/// </summary>
	public class TextBinaryDiffArgs
	{
		#region Constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="itemA">Is either a path to the left text A or the text itself directly.
		/// The interpretation of the string content depends on the setting in <paramref name="diffType"/>.
		/// </param>
		/// <param name="itemB">Is either a path to the right text B or the text itself directly.
		/// The interpretation of the string content depends on the setting in <paramref name="diffType"/>.
		/// </param>
		/// <param name="diffType">This should be either set to <see cref="DiffType.File"/> or <see cref="DiffType.Text"/>
		/// to interprete values in <paramref name="itemA"/> and <paramref name="itemB"/> either as file
		/// based input or direct string based input.</param>
		/// <param name="spacesPerTab">number of spaces that should be applied to a TAB character.</param>
		public TextBinaryDiffArgs(string itemA, string itemB, DiffType diffType, int spacesPerTab)
			: this()
		{
			this.A = itemA;
			this.B = itemB;
			this.DiffType = diffType;
			this.SpacesPerTab = spacesPerTab;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="itemA">Is either a path to the left text A or the text itself directly.
		/// The interpretation of the string content depends on the setting in <paramref name="diffType"/>.
		/// </param>
		/// <param name="itemB">Is either a path to the right text B or the text itself directly.
		/// The interpretation of the string content depends on the setting in <paramref name="diffType"/>.
		/// </param>
		/// <param name="diffType">This should be either set to <see cref="DiffType.File"/> or <see cref="DiffType.Text"/>
		/// to interprete values in <paramref name="itemA"/> and <paramref name="itemB"/> either as file
		/// based input or direct string based input.</param>
		public TextBinaryDiffArgs(string itemA, string itemB, DiffType diffType)
			: this()
		{
			this.A = itemA;
			this.B = itemB;
			this.DiffType = diffType;
		}

		/// <summary>
		/// Hidden standard constructor
		/// </summary>
		protected TextBinaryDiffArgs()
		{
			SpacesPerTab = 4;
			IgnoreCase = true;
			IgnoreTextWhitespace = true;
			ShowChangeAsDeleteInsert = false;

			IgnoreXmlWhitespace = true;

			HashType = HashType.HashCode;
			CompareType = CompareType.Auto;

			BinaryFootprintLength = 8;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets either a path to the left text A or the text itself directly.
		/// The interpretation of the properties string content depends on the
		/// setting in <see cref="DiffType"/> property.
		/// </summary>
		public string A { get; }

		/// <summary>
		/// Gets either a path to the left text B or the text itself directly.
		/// The interpretation of the properties string content depends on the
		/// setting in <see cref="DiffType"/> property.
		/// </summary>
		public string B { get; }

		/// <summary>
		/// This should be either set to <see cref="DiffType.File"/> or <see cref="DiffType.Text"/>
		/// to interprete values in properties <see cref="A"/> and <see cref="B"/> either as file
		/// based input or direct string based input.
		/// </summary>
		public DiffType DiffType { get; }

		/// <summary>
		/// Gets the number of spaces that should be applied to a TAB character.
		/// </summary>
		public int SpacesPerTab { get; }

		/// <summary>
		/// Gets/sets wether the text should by compared text case-sensitive
		/// or not (case-insensitive).
		/// </summary>
		public bool IgnoreCase { get; set; }

		/// <summary>
		/// Gets/sets whether to ignore starting and ending white spaces when comparing strings.
		/// 
		/// Turn this on to get these equalities:
		/// - Strings that contain only whitespaces are equal.
		/// - Two strings like '  A' and 'A  ' are equal.
		/// </summary>
		public bool IgnoreTextWhitespace { get; set; }

		/// <summary>
		/// Gets/sets whether lines that can be aligned as change are displayed as
		/// changed lines with in-line differences, or whether line differences are
		/// only compared and displayed with inserted and deleted lines.
		/// </summary>
		public bool ShowChangeAsDeleteInsert { get; set; }

		/// <summary>
		/// Gets/sets whether to ignore insignificant white space when comparing Xml content.
		/// </summary>
		public bool IgnoreXmlWhitespace { get; set; }

		public HashType HashType { get; }

		/// <summary>
		/// Gets/sets whether the type of media being compared is determined automatically,
		/// or should be interpreted as Text, XML, or Binary.
		/// </summary>
		public CompareType CompareType { get; set; }

		/// <summary>
		/// Gets whether the type of media information content being compared should be
		/// determined automatically, or not.
		/// </summary>
		public bool IsAuto { get { return this.CompareType == CompareType.Auto; } }

		/// <summary>
		/// Number of bytes to be displayed per line in binary view rendered as text output.
		/// </summary>
		public int BinaryFootprintLength { get; }
		#endregion
	}

	internal class DiffBinaryTextResults
	{
		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="isBinaryCompar">Whether the data stored in this object should be interpreted as binary or not.</param>
		public DiffBinaryTextResults(bool isBinaryCompar)
		{
			IsBinaryCompare = isBinaryCompar;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		protected DiffBinaryTextResults()
		{
			a = null; //new List<string>();
			b = null; //new List<string>();
		}
		#endregion ctors

		/// <summary>
		/// Output parameter from binary reader to hint correct display/diff of lines.
		/// Leading number of characters to ignore for diff in each line.
		/// This space is used in binary diff to display 8 digit line number and 4 digit space.
		/// </summary>
		public int LeadingCharactersToIgnore { get; set; }

		/// <summary>
		/// Gets whether the returned data should be interpreted as binary or not.
		/// </summary>
		public bool IsBinaryCompare { get; }

		public bool IgnoreCase => IsBinaryCompare? false : true;

		public bool IgnoreTextWhitespace => IsBinaryCompare? false : true;


		/// <summary>
		/// Gets a list of lines (text or binary rendered as text) for the left side of the comparison.
		/// </summary>
		public IList<string> a { get; set; }

		/// <summary>
		/// Gets a list of lines (text or binary rendered as text) for the right side of the comparison.
		/// </summary>
		public IList<string> b { get; set; }
	}
}
