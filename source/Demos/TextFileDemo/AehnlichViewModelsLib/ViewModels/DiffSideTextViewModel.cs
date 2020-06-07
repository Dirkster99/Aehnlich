using AehnlichViewLib.Controls.AvalonEditEx;
using AehnlichViewLib.Enums;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Text;

namespace AehnlichViewModelsLib.ViewModels
{
	/// <summary>
	/// Implements a text viewmodel for AvalonEdit that supports requirements for
	/// 1) Comparing text (with background diff highlighting) or
	/// 2) Editing text (without background diff highlighting).
	/// </summary>
	public class DiffSideTextViewModel : Base.ViewModelBase
	{
		#region fields
		private TextDocument _document = null;
		private bool _isDirty = false;
		private bool _IsReadOnly;
		private string _IsReadOnlyReason = string.Empty;
		private string _FileName;
		private TextBoxController _TxtControl;
		private int _Column;
		private int _Line;
		#endregion fields

		#region ctors
		/// <summary>Class constructor</summary>
		/// <param name="viewMode">Sets the type of view mode (compare, edit) that is associated with this document instance.</param>
		/// <param name="text"></param>
		/// <param name="originalText"></param>
		/// <param name="originalTextEncoding"></param>
		public DiffSideTextViewModel(DisplayMode viewMode, string text, Encoding originalTextEncoding, string originalText)
			: this(viewMode, text)
		{
			OriginalText = originalText;
			TextEncoding = originalTextEncoding;
		}

		/// <summary>Hidden Class constructor</summary>
		/// <param name="viewMode">Sets the type of view mode (compare, edit) that is associated with this document instance.</param>
		/// <param name="text"></param>
		protected DiffSideTextViewModel(DisplayMode viewMode, string text)
			: this()
		{
			ViewMode = viewMode;
			Document = new TextDocument(text);

			switch (viewMode)
			{
				case DisplayMode.Comparing:
					IsReadOnly = true;
					IsReadOnlyReason = "Editing is not supported while comparing content.";
					break;

				case DisplayMode.Editing:
					IsReadOnly = false;
					break;

				default:
					throw new NotImplementedException(viewMode.ToString());
			}
		}

		/// <summary>Hidden Class constructor</summary>
		protected DiffSideTextViewModel()
		{
			Document = new TextDocument(string.Empty);
			_TxtControl = new TextBoxController();

			_Line = _Column = 1;
			TextEncoding = Encoding.Default;
		}
		#endregion ctors

		#region properties
		/// <summary>
		/// Gets/sets the <see cref="TextDocument"/> viewmodel of the attached AvalonEdit
		/// text editor control.
		/// </summary>
		public TextDocument Document
		{
			get => _document;
			set
			{
				if (this._document != value)
				{
					this._document = value;
					NotifyPropertyChanged(nameof(Document));
				}
			}
		}

		/// <summary>Gets the encoding of the text hosted in this viewmodel</summary>
		public Encoding TextEncoding { get; }

		/// <summary>
		/// Gets/sets whether the currently shown text in the textedior has been changed
		/// without saving or not.
		/// </summary>
		public bool IsDirty
		{
			get => _isDirty;
			set
			{
				if (_isDirty != value)
				{
					_isDirty = value;
					NotifyPropertyChanged(nameof(IsDirty));
				}
			}
		}

		/// <summary>
		/// Gets whether the text displayed in the diff should be editable or not.
		/// </summary>
		public bool IsReadOnly
		{
			get => _IsReadOnly;
			protected set
			{
				if (_IsReadOnly != value)
				{
					_IsReadOnly = value;
					NotifyPropertyChanged(nameof(IsReadOnly));
				}
			}
		}

		/// <summary>Gets a human readable reason why this might be readonly.</summary>
		public string IsReadOnlyReason
		{
			get => _IsReadOnlyReason;
			internal set
			{
				if (_IsReadOnlyReason != value)
				{
					_IsReadOnlyReason = value;
					NotifyPropertyChanged(nameof(IsReadOnlyReason));
				}
			}
		}

		/// <summary>Gets the name of the file from which the content in this viewmodel was red.</summary>
		public string FileName
		{
			get => _FileName;
			set
			{
				if (_FileName != value)
				{
					_FileName = value;

					NotifyPropertyChanged(nameof(FileName));
				}
			}
		}

		/// <summary>Gets a view mode indicating whether we are comparing text or editing text.</summary>
		public DisplayMode ViewMode { get; }

		/// <summary>Gets the original text (not necessarily changed by editor).</summary>
		public string OriginalText { get; internal set; }

		/// <summary>
		/// Gets/sets the textbox controller that is used to drive the view
		/// from within the viewmodel (with event based commands like goto line x,y).
		/// </summary>
		public TextBoxController TxtControl
		{
			get { return _TxtControl; }

			private set
			{
				if (_TxtControl != value)
				{
					_TxtControl = value;
					NotifyPropertyChanged(nameof(TxtControl));
				}
			}
		}

		/// <summary>Gets the column of the text cursor position.</summary>
		public int Column
		{
			get
			{
				return _Column;
			}

			internal set
			{
				if (_Column != value)
				{
					_Column = value;
					NotifyPropertyChanged(nameof(Column));
				}
			}
		}

		/// <summary>Gets the line of the text cursor position.</summary>
		public int Line
		{
			get
			{
				return _Line;
			}

			internal set
			{
				if (_Line != value)
				{
					_Line = value;
					NotifyPropertyChanged(nameof(Line));
				}
			}
		}
		#endregion properties

		#region methods
		#endregion methods
	}
}
