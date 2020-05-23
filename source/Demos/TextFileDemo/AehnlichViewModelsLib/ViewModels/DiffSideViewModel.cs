namespace AehnlichViewModelsLib.ViewModels
{
	using AehnlichViewLib.Controls.AvalonEditEx;
	using AehnlichViewLib.Events;
	using AehnlichViewLib.Interfaces;
	using AehnlichViewModelsLib.Enums;
	using AehnlichViewModelsLib.Events;
	using AehnlichViewModelsLib.Interfaces;
	using AehnlichViewModelsLib.Tasks;
	using AehnlichViewModelsLib.ViewModels.Base;
	using HL.Interfaces;
	using ICSharpCode.AvalonEdit.Document;
	using ICSharpCode.AvalonEdit.Highlighting;
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Input;
	using System.Windows.Media;

	/// <summary>
	/// Implements the viewmodel that controls one side of a text diff view with two sides
	/// (left view A and right view B) where both views are synchronized towards the displayed
	/// line numbers and content being highlighted to visual differences (add, remove, change, no chaneg).
	/// </summary>
	internal class DiffSideViewModel : Base.ViewModelBase, IDiffSideViewModel
	{
		#region fields
		private ChangeDiffOptions _ChangeDiffOptions;
		private TextDocument _document = null;
		private TextBoxController _TxtControl;

		private readonly DiffViewPosition _position;
		private int _Column;
		private int _Line;
		private int _spacesPerTab = 4;

		private DateTime _ViewActivation;
		private bool _isDirty = false;
		private string _FileName;
		private OneTaskLimitedScheduler _oneTaskScheduler;

		#region DiffLines
		private readonly object _DocLineDiffsLock;
		private readonly ObservableRangeCollection<IDiffLineViewModel> _DocLineDiffs;

		private int[] _diffEndLines = null;
		private int[] _diffStartLines = null;

		/// <summary>
		/// Maximum imaginary line number which incorporates not only real text lines
		/// but also imaginary line that where inserted on either side of the comparison
		/// view to sync both sides into a consistent display.
		/// </summary>
		private int _maxImaginaryLineNumber = 1;
		private bool _disposed;
		private IHighlightingDefinition _HighlightingDefinition;
		private ICommand _HighlightingChangeCommand;
		private bool _IsHighlightingDefinitionOff;
		#endregion DiffLines
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		public DiffSideViewModel()
		{
			_position = new DiffViewPosition(0, 0);

			_DocLineDiffsLock = new object();
			_DocLineDiffs = new ObservableRangeCollection<IDiffLineViewModel>();
			BindingOperations.EnableCollectionSynchronization(_DocLineDiffs, _DocLineDiffsLock);

			_Line = 0;
			_Column = 0;

			_TxtControl = new TextBoxController();
			_ViewActivation = DateTime.MinValue;

			_oneTaskScheduler = new OneTaskLimitedScheduler();
		}
		#endregion ctors

		#region Events
		/// <summary>
		/// Event is raised when the cursor position in the attached view is changed.
		/// </summary>
		public event EventHandler<CaretPositionChangedEvent> CaretPositionChanged;

		/// <summary>
		/// Event is raised when newly requested line diff edit script segments
		/// have been computed and are available for hightlighting.
		/// 
		/// <seealso cref="ILineDiffProvider"/>
		/// </summary>
		public event EventHandler<DiffLineInfoChangedEvent> DiffLineInfoChanged;
		#endregion Events

		#region properties
		/// <summary>
		/// Gets/sets the <see cref="TextDocument"/> viewmodel of the attached AvalonEdit
		/// text editor control.
		/// </summary>
		public TextDocument Document
		{
			get { return this._document; }
			set
			{
				if (this._document != value)
				{
					this._document = value;
					NotifyPropertyChanged(() => Document);
				}
			}
		}

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
					NotifyPropertyChanged(() => TxtControl);
				}
			}
		}

		#region Highlighting Definition
		/// <summary>
		/// AvalonEdit exposes a Highlighting property that controls whether keywords,
		/// comments and other interesting text parts are colored or highlighted in any
		/// other visual way. This property exposes the highlighting information for the
		/// text file managed in this viewmodel class.
		/// </summary>
		public IHighlightingDefinition HighlightingDefinition
		{
			get
			{
				return _HighlightingDefinition;
			}

			set
			{
				if (_HighlightingDefinition != value)
				{
					_HighlightingDefinition = value;
					NotifyPropertyChanged(() => HighlightingDefinition);
				}
			}
		}

		/// <summary>
		/// Gets a command that changes the currently selected syntax highlighting in the editor.
		/// </summary>
		public ICommand HighlightingChangeCommand
		{
			get
			{
				if (_HighlightingChangeCommand == null)
				{
					_HighlightingChangeCommand = new RelayCommand<object>((p) =>
					{
						var parames = p as object[];

						if (parames == null)
							return;

						if (parames.Length != 1)
							return;

						var param = parames[0] as IHighlightingDefinition;
						if (param == null)
							return;

						IsHighlightingDefinitionOff = false;
						HighlightingDefinition = param;
					});
				}

				return _HighlightingChangeCommand;
			}
		}

		/// <summary>
		/// Gets a value to indicate whether text highlighting should currently be shown or not.
		/// </summary>
		public bool IsHighlightingDefinitionOff
		{
			get { return _IsHighlightingDefinitionOff; }

			private set
			{
				if (_IsHighlightingDefinitionOff != value)
				{
					_IsHighlightingDefinitionOff = value;
					NotifyPropertyChanged(() => IsHighlightingDefinitionOff);
				}
			}
		}
		#endregion Highlighting Definition

		/// <summary>
		/// Gets/set the time stamp of the last time when the attached view
		/// has been activated (GotFocus).
		/// </summary>
		public DateTime ViewActivation
		{
			get
			{
				return _ViewActivation;
			}

			set
			{
				if (_ViewActivation != value)
				{
					_ViewActivation = value;
					NotifyPropertyChanged(() => ViewActivation);
				}
			}
		}

		#region Caret Position
		/// <summary>
		/// Gets/sets the column of a display position.
		/// </summary>
		public int Column
		{
			get
			{
				return _Column;
			}

			set
			{
				if (_Column != value)
				{
					_Column = value;
					NotifyPropertyChanged(() => Column);

					CaretPositionChanged?.Invoke(this,
						new CaretPositionChangedEvent(_Line, _Column, CaretChangeType.Column));
				}
			}
		}

		/// <summary>
		/// Gets/sets the line of a display position.
		/// </summary>
		public int Line
		{
			get
			{
				return _Line;
			}

			set
			{
				if (_Line != value)
				{
					_Line = value;
					NotifyPropertyChanged(() => Line);

					CaretPositionChanged?.Invoke(this,
						new CaretPositionChangedEvent(_Line, _Column, CaretChangeType.Line));
				}
			}
		}
		#endregion Caret Position

		/// <summary>
		/// Gets/sets whether the currently shown text in the textedior has been changed
		/// without saving or not.
		/// </summary>
		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				if (_isDirty != value)
				{
					_isDirty = value;
					NotifyPropertyChanged(() => IsDirty);
				}
			}
		}

		/// <summary>
		/// Gets whether the diff view control is enabled or not.
		/// </summary>
		public bool IsEnabled { get { return true; } }

		/// <summary>
		/// Gets whether line numbers should be shown in the diff view control or not.
		/// </summary>
		public bool ShowLineNumbers { get { return true; } }

		/// <summary>
		/// Gets whether the text displayed in the diff should be editable or not.
		/// </summary>
		public bool IsReadOnly { get { return true; } }

		/// <summary>
		/// Gets Text/binary specific diff options (eg. ignore white space) which are applied
		/// to compute the text differences shown in the view.
		/// </summary>
		public ChangeDiffOptions ChangeDiffOptions
		{
			get
			{
				return _ChangeDiffOptions;
			}

			internal set
			{
				if (_ChangeDiffOptions != value)
				{
					_ChangeDiffOptions = value;
					NotifyPropertyChanged(() => ChangeDiffOptions);
				}
			}
		}

		/// <summary>
		/// Gets the name of the file from which the content in this viewmodel was red.
		/// </summary>
		public string FileName
		{
			get { return _FileName; }
			protected set
			{
				if (_FileName != value)
				{
					_FileName = value;
					NotifyPropertyChanged(() => FileName);
				}
			}
		}

		#region DiffLines
		/// <summary>
		/// Gets a list of line information towards their difference when
		/// compared to the other document.
		/// </summary>
		public IReadOnlyList<IDiffLineViewModel> DocLineDiffs
		{
			get
			{
				return _DocLineDiffs;
			}
		}

		/// <summary>
		/// Gets the number of line items available in the <see cref="DocLineDiffs"/> property.
		/// </summary>
		public int LineCount { get { return DocLineDiffs.Count; } }

		/// <summary>
		/// Gets the line where a diff identified by an index i starts.
		/// (eg. the diff starts i=0 start at line 4 then we have DiffStartLines[0] == 4 )
		/// see also <see cref="DiffEndLines"/>
		/// </summary>
		public int[] DiffStartLines { get { return _diffStartLines; } }

		/// <summary>
		/// Gets the line where a diff identified by an index i ends.
		/// (eg. the diff starts i=0 start at line 4 then we have DiffEndLines[0] == 4 )
		/// see also <see cref="DiffStartLines"/>
		/// </summary>
		public int[] DiffEndLines { get { return _diffEndLines; } }
		#endregion DiffLines

		/// <summary>
		/// Gets a maximum imaginary line number which incorporates not only real text lines
		/// but also imaginary line that where inserted on either side of the comparison
		/// view to sync both sides into a consistent display.
		/// </summary>
		public int MaxLineNumber { get { return _maxImaginaryLineNumber; } }
		#endregion properties

		#region methods
		/// <summary>
		/// Implements a method that is invoked by the view to request
		/// the matching (edit script computation) of the indicated text lines.
		/// 
		/// This method should be called on the UI thread since
		/// the resulting event <see cref="ILineDiffProvider.DiffLineInfoChanged"/>
		/// will be raised on the calling thread.
		/// </summary>
		/// <returns>Number of lines matched (may not be as requested if line appears to have been matched already).</returns>
		void ILineDiffProvider.RequestLineDiff(IEnumerable<int> linenumbers)
		{
			// Capture the current context and make sure resulting event is raised on that context
			var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

			Task.Factory.StartNew<List<int>>(() =>
			{
				var linesChanged = new List<int>();

				foreach (var i in linenumbers)
				{
					if (_DocLineDiffs.Count <= i)
						continue;

					// We've previously seen and computed this?
					if (DocLineDiffs[i].LineEditScriptSegmentsIsDirty == false)
						continue;

					DocLineDiffs[i].GetChangeEditScript(this.ChangeDiffOptions, _spacesPerTab);

					// Its possible that we have to match this even though there is no result
					// So, we optimize empty results away like this
					if (DocLineDiffs[i].LineEditScriptSegments != null)
						linesChanged.Add(i);
				}

				return linesChanged;
			}
			, CancellationToken.None, TaskCreationOptions.PreferFairness, _oneTaskScheduler
			).ContinueWith((r) =>
			{
				// Tell the view we are done please redraw these if you like
				if (r.Result.Count > 0)
				{
					this.DiffLineInfoChanged?.Invoke(this,
						new DiffLineInfoChangedEvent(DiffLineInfoChange.LineEditScriptSegments, r.Result));
				}
			},
			uiScheduler);
		}

		#region FirstDiff NextDiff PrevDiff LastDiff
		/// <summary>
		/// Determine whether or not we can goto the first difference in the model.
		/// 
		/// The function returns false:
		/// - if there is no difference or
		/// - if current positioning already indicates positioning on the 1st difference
		/// </summary>
		public bool CanGoToFirstDiff()
		{
			if (DiffStartLines == null || DiffEndLines == null)
				return false;

			bool result = false;

			int[] starts = DiffStartLines;
			int[] ends = DiffEndLines;

			result = starts.Length > 0 &&
						ends.Length > 0 &&
						(_position.Line < starts[0] || _position.Line > ends[0]);

			return result;
		}

		public bool CanGoToNextDiff()
		{
			if (DiffStartLines == null)
				return false;

			bool result = false;

			int[] starts = DiffStartLines;
			result = starts.Length > 0 && _position.Line < starts[starts.Length - 1];

			return result;
		}

		public bool CanGoToPreviousDiff()
		{
			if (DiffEndLines == null)
				return false;

			bool result = false;

			int[] ends = DiffEndLines;
			result = ends.Length > 0 && _position.Line > ends[0];

			return result;
		}

		public bool CanGoToLastDiff()
		{
			if (DiffStartLines == null || DiffEndLines == null)
				return false;

			bool result = false;

			int[] starts = DiffStartLines;
			int[] ends = DiffEndLines;
			result = starts.Length > 0 && ends.Length > 0 &&
				(_position.Line < starts[starts.Length - 1] || _position.Line > ends[ends.Length - 1]);

			return result;
		}

		/// <summary>
		/// (re)Sets the current caret position (column and line) in the text editor view.
		/// </summary>
		/// <param name="gotoPos"></param>
		public void SetPosition(IDiffViewPosition gotoPos)
		{
			_position.SetPosition(gotoPos.Line, gotoPos.Column);
		}

		/// <summary>
		/// Gets the position of the first difference in the text.
		/// </summary>
		/// <returns></returns>
		public IDiffViewPosition GetFirstDiffPosition()
		{
			return new DiffViewPosition(DiffStartLines[0], _position.Column);
		}

		/// <summary>
		/// Gets the position of the next difference in the text.
		/// </summary>
		/// <returns></returns>
		public IDiffViewPosition GetNextDiffPosition()
		{
			int[] starts = DiffStartLines;
			int numStarts = starts.Length;
			for (int i = 0; i < numStarts; i++)
			{
				if (_position.Line < starts[i])
				{
					return new DiffViewPosition(starts[i], _position.Column);
				}
			}

			// We should never get here.
			Debug.Assert(false, "CanGoToPreviousDiff was wrong.");
			return default(DiffViewPosition);
		}

		/// <summary>
		/// Gets the position of the previous difference in the text.
		/// </summary>
		/// <returns></returns>
		public IDiffViewPosition GetPrevDiffPosition()
		{
			int[] ends = DiffEndLines;
			int numEnds = ends.Length;
			for (int i = numEnds - 1; i >= 0; i--)
			{
				if (_position.Line > ends[i])
				{
					// I'm intentionally setting the line to Starts[i] here instead of Ends[i].
					return new DiffViewPosition(DiffStartLines[i], _position.Column);
				}
			}

			// We should never get here.
			Debug.Assert(false, "CanGoToPreviousDiff was wrong.");
			return default(DiffViewPosition);
		}

		/// <summary>
		/// Gets the position of the last difference in the text.
		/// </summary>
		/// <returns></returns>
		public IDiffViewPosition GetLastDiffPosition()
		{
			int[] starts = DiffStartLines;

			return new DiffViewPosition(starts[starts.Length - 1], _position.Column);
		}
		#endregion FirstDiff NextDiff PrevDiff LastDiff

		/// <summary>
		/// Scrolls the attached view to line <paramref name="n"/>
		/// where n should in the range of [1 ... max lines].
		/// </summary>
		/// <param name="n"></param>
		/// <param name="positionCursor"></param>
		public void ScrollToLine(int n, bool positionCursor)
		{
			DocumentLine line = Document.GetLineByNumber(n);

			if (positionCursor == true)                  // Position caret with
				TxtControl.SelectText(line.Offset, 0);  // Text Selection length 0 and scroll to where

			TxtControl.ScrollToLine(n);               // we are supposed to be at
		}

		/// <summary>
		/// Implements a user option to switches the highlighting in text documents off.
		/// </summary>
		internal void SwitchHighlightingDefinitionOff()
		{
			IsHighlightingDefinitionOff = true;
			HighlightingDefinition = null;
		}

		/// <summary>
		/// Used to setup the ViewA/ViewB view that shows the left and right text views
		/// with the textual content and imaginary lines.
		/// each other.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="lines"></param>
		/// <param name="text"></param>
		/// <param name="spacesPerTab"></param>
		internal void SetData(string filename,
							  IDiffLines lines, string text, int spacesPerTab)
		{
			this.FileName = filename;

			try
			{
				string ext = System.IO.Path.GetExtension(filename);

				var hlManager = GetService<IThemedHighlightingManager>();

				// Use fallback to actual service implementation if injector is not used here...
				if (hlManager == null)
					hlManager = HL.Manager.ThemedHighlightingManager.Instance;

				if (IsHighlightingDefinitionOff == false)
					HighlightingDefinition = hlManager.GetDefinitionByExtension(ext);
			}
			catch
			{
				// We go without highlighting in case System.IO throws an exception here
				HighlightingDefinition = null;
			}

			_position.SetPosition(0, 0);
			_spacesPerTab = spacesPerTab;
			Line = 0;
			Column = 0;

			if (lines != null)
			{
				_diffEndLines = lines.DiffEndLines;
				_diffStartLines = lines.DiffStartLines;
				_maxImaginaryLineNumber = lines.MaxImaginaryLineNumber;

				_DocLineDiffs.ReplaceRange(lines.DocLineDiffs);
			}
			else
			{
				_diffEndLines = null;
				_diffStartLines = null;
				_maxImaginaryLineNumber = 1;

				_DocLineDiffs.Clear();
			}

			Document = new TextDocument(text);

			NotifyPropertyChanged(() => Document);
		}

		/// <summary>
		/// Used to setup the ViewLineDiff view that shows only 2 lines over each other
		/// representing the currently active line from the left/right side views under
		/// each other.
		/// </summary>
		/// <param name="lineOneVM"></param>
		/// <param name="lineTwoVM"></param>
		/// <param name="spacesPerTab"></param>
		internal void SetData(IDiffLineViewModel lineOneVM,
							  IDiffLineViewModel lineTwoVM,
							  int spacesPerTab)
		{
			_spacesPerTab = spacesPerTab;
			var documentLineDiffs = new List<IDiffLineViewModel>();

			string text = string.Empty;

			if (lineOneVM != null && lineOneVM.LineEditScriptSegmentsIsDirty == true)
				lineOneVM.GetChangeEditScript(this.ChangeDiffOptions, spacesPerTab);

			if (lineTwoVM != null && lineTwoVM.LineEditScriptSegmentsIsDirty == true)
				lineTwoVM.GetChangeEditScript(this.ChangeDiffOptions, spacesPerTab);

			if (lineOneVM != null && lineTwoVM != null)
			{
				documentLineDiffs.Add(lineOneVM);
				text += lineOneVM.Text + '\n';

				documentLineDiffs.Add(lineTwoVM);
				text += lineTwoVM.Text + "\n";
			}

			text = text.Replace("\t", "    ");

			// Update LineInfo viewmodels
			_DocLineDiffs.Clear();
			_DocLineDiffs.AddRange(documentLineDiffs, NotifyCollectionChangedAction.Reset);

			// Update text document
			Document = new TextDocument(text);

			NotifyPropertyChanged(() => DocLineDiffs);
			NotifyPropertyChanged(() => Document);
		}

		/// <summary>
		/// Gets the n-th line of the diff stored in this viewmodel and returns it.
		/// </summary>
		/// <param name="lineN"></param>
		/// <returns></returns>
		internal IDiffLineViewModel GetLine(int lineN)
		{
			if (lineN >= LineCount || LineCount == 0)
				return null;

			return DocLineDiffs[lineN];
		}

		internal IDiffLineViewModel GotoTextLine(int thisLine)
		{
			DocumentLine line = Document.GetLineByNumber(thisLine);

			TxtControl.SelectText(line.Offset, 0);  // Select text with length 0 and scroll to where
			TxtControl.ScrollToLine(thisLine);     // we are supposed to be at

			return _DocLineDiffs[thisLine - 1];
		}

		internal int FindThisTextLine(int thisLine)
		{
			// Translate given line number into real line number (adding virtual lines if any)
			int idx = Math.Min(thisLine, _DocLineDiffs.Count - 1);
			if (idx < 0)
				idx = 0;

			var model = _DocLineDiffs[idx];

			int iCurrLineNumber = (model.ImaginaryLineNumber == null ? 0 : (int)model.ImaginaryLineNumber);

			// TODO: Naive search should be binary later on
			if (iCurrLineNumber < thisLine)
			{
				for (; idx < _DocLineDiffs.Count; idx++)
				{
					model = _DocLineDiffs[idx];
					iCurrLineNumber = (model.ImaginaryLineNumber == null ? 0 : (int)model.ImaginaryLineNumber);

					if (iCurrLineNumber >= thisLine)
						break;
				}
			}

			return idx;
		}

		/// <summary>
		/// Invoke this method to apply a change of theme to the content of the document
		/// (eg: Adjust the highlighting colors when changing from "Dark" to "Light"
		///      WITH current text document loaded.)
		/// </summary>
		internal void OnAppThemeChanged(IThemedHighlightingManager hlManager)
		{
			if (hlManager == null)
				return;

			// Does this highlighting definition have an associated highlighting theme?
			if (hlManager.CurrentTheme.HlTheme != null)
			{
				// A highlighting theme with GlobalStyles?
				// Apply these styles to the resource keys of the editor
				foreach (var item in hlManager.CurrentTheme.HlTheme.GlobalStyles)
				{
					switch (item.TypeName)
					{
						case "DefaultStyle":
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorBackground, item.backgroundcolor);
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorForeground, item.foregroundcolor);
							break;

						case "CurrentLineBackground":
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorCurrentLineBackgroundBrushKey, item.backgroundcolor);
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorCurrentLineBorderBrushKey, item.bordercolor);
							break;

						case "LineNumbersForeground":
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorLineNumbersForeground, item.foregroundcolor);
							break;

						case "Selection":
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorSelectionBrush, item.backgroundcolor);
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorSelectionBorder, item.bordercolor);
							break;

						case "Hyperlink":
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorLinkTextBackgroundBrush, item.backgroundcolor);
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorLinkTextForegroundBrush, item.foregroundcolor);
							break;

						case "NonPrintableCharacter":
							ApplyToDynamicResource(AehnlichViewLib.Themes.ResourceKeys.EditorNonPrintableCharacterBrush, item.foregroundcolor);
							break;

						default:
							throw new System.ArgumentOutOfRangeException("GlobalStyle named '{0}' is not supported.", item.TypeName);
					}
				}
			}

			if (IsHighlightingDefinitionOff == true)
				return;

			// 1st try: Find highlighting based on currently selected highlighting
			// The highlighting name may be the same as before, but the highlighting theme has just changed
			if (HighlightingDefinition != null)
			{
				// Reset property for currently select highlighting definition
				HighlightingDefinition = hlManager.GetDefinition(HighlightingDefinition.Name);

				if (HighlightingDefinition != null)
					return;
			}

			// 2nd try: Find highlighting based on extension of file currenlty being viewed
			if (string.IsNullOrEmpty(this.FileName))
				return;

			string extension = System.IO.Path.GetExtension(this.FileName);

			if (string.IsNullOrEmpty(extension))
				return;

			// Reset property for currently select highlighting definition
			HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);
		}

		/// <summary>
		/// Re-define an existing <seealso cref="SolidColorBrush"/> and backup the originial color
		/// as it was before the application of the custom coloring.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="newColor"></param>
		private void ApplyToDynamicResource(ComponentResourceKey key, Color? newColor)
		{
			if (Application.Current.Resources[key] == null || newColor == null)
				return;

			// Re-coloring works with SolidColorBrushs linked as DynamicResource
			if (Application.Current.Resources[key] is SolidColorBrush)
			{
				//backupDynResources.Add(resourceName);

				var newColorBrush = new SolidColorBrush((Color)newColor);
				newColorBrush.Freeze();

				Application.Current.Resources[key] = newColorBrush;
			}
		}

		#region IDisposable
		/// <summary>
		/// Standard dispose method of the <seealso cref="IDisposable" /> interface.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed == false)
			{
				if (disposing == true)
				{
					// Dispose of the currently used inner disposables
					_oneTaskScheduler.Dispose();
					_oneTaskScheduler = null;
				}

				// There are no unmanaged resources to release, but
				// if we add them, they need to be released here.
			}

			_disposed = true;

			//// If it is available, make the call to the
			//// base class's Dispose(Boolean) method
			////base.Dispose(disposing);
		}
		#endregion IDisposable
		#endregion methods
	}
}