namespace AehnlichViewModelsLib.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Windows.Input;
    using System.Xml;
    using AehnlichLib.Binaries;
    using AehnlichLib.Dir;
    using AehnlichLib.Text;
    using AehnlichViewModelsLib.Enums;
    using AehnlichViewModelsLib.Events;
    using AehnlichViewModelsLib.Models;
    using AehnlichViewModelsLib.ViewModels.Base;
    using AehnlichViewModelsLib.ViewModels.LineInfo;
    using ICSharpCode.AvalonEdit;

    /// <summary>
    /// Implements a viewmodel that services both views viewA and viewB (left and right).
    /// </summary>
    public class DiffDocViewModel : Base.ViewModelBase, IDisposable
    {
        #region fields
        private ShowDiffArgs _Args;
        private uint _NumberOfLines, _MaxNumberOfLines;

        private readonly DiffSideViewModel _ViewA;
        private readonly DiffSideViewModel _ViewB;
        private readonly DiffSideViewModel _ViewLineDiff;

        private int _LineDiffHeight = 38;
        private string _Similarity_Text;
        private bool _edtLeft_Right_Visible;
        private string _edtRight_Text;
        private string _edtLeft_Text;
        private int currentDiffLine = -1;
        private int _SynchronizedLine = 1;
        private int _SynchronizedColumn = 0;
        private ICommand _GoToLastDifferenceCommand;
        private ICommand _GoToPrevDifferenceCommand;
        private ICommand _GoToNextDifferenceCommand;
        private ICommand _GoToFirstDifferenceCommand;
        private string _StatusText;
        private bool _disposed;
        private readonly TextEditorOptions _DiffViewOptions;
        #endregion fields

        #region ctors
        /// <summary>
        /// class constructor
        /// </summary>
        public DiffDocViewModel()
        {
            _DiffViewOptions = new TextEditorOptions() { ShowTabs = false, ConvertTabsToSpaces = true, IndentationSize = 4 };
            _ViewA = new DiffSideViewModel();
            _ViewB = new DiffSideViewModel();
            _ViewLineDiff = new DiffSideViewModel();

            _ViewA.CaretPositionChanged += OnViewACaretPositionChanged;
            _ViewB.CaretPositionChanged += OnViewBCaretPositionChanged;
        }
        #endregion ctors

        #region properties
        public TextEditorOptions DiffViewOptions
        {
            get
            {
                return _DiffViewOptions;
            }
        }

        public DiffSideViewModel ViewA
        {
            get { return _ViewA; }
        }

        public DiffSideViewModel ViewB
        {
            get { return _ViewB; }
        }

        /// <summary>
        /// Gets whether both viewmodels ViewA or ViewB hold more than
        /// no line to compare (enabling comparison functions makes no sense if this is false).
        /// </summary>
        public bool IsDiffDataAvailable
        {
            get
            {
                return _ViewA.LineCount > 0 || _ViewB.LineCount > 0;
            }
        }

        #region Caret Position
        public int SynchronizedColumn
        {
            get
            {
                return _SynchronizedColumn;
            }

            set
            {
                if (_SynchronizedColumn != value)
                {
                    _SynchronizedColumn = value;
                    NotifyPropertyChanged(() => SynchronizedColumn);
                }
            }
        }

        public int SynchronizedLine
        {
            get
            {
                return _SynchronizedLine;
            }

            set
            {
                if (_SynchronizedLine != value)
                {
                    _SynchronizedLine = value;
                    NotifyPropertyChanged(() => SynchronizedLine);
                }
            }
        }
        #endregion Caret Position

        /// <summary>
        /// Gets the similarity value (0% - 100%) between 2 things shown in toolbar
        /// </summary>
        public string Similarity_Text
        {
            get
            {
                return _Similarity_Text;
            }

            internal set
            {
                if (_Similarity_Text != value)
                {
                    _Similarity_Text = value;
                    NotifyPropertyChanged(() => Similarity_Text);
                }
            }
        }

        #region Left and Right File Name Labels
        /// <summary>
        /// Gets whether left and right file name labels over each ViewA and ViewB
        /// are visible or not.
        /// </summary>
        public bool edtLeft_Right_Visible
        {
            get
            {
                return _edtLeft_Right_Visible;
            }

            internal set
            {
                if (_edtLeft_Right_Visible != value)
                {
                    _edtLeft_Right_Visible = value;
                    NotifyPropertyChanged(() => edtLeft_Right_Visible);
                }
            }
        }

        /// <summary>
        /// Gets the left text label (file name) displayed over the left diff view (ViewA).
        /// </summary>
        public string edtLeft_Text
        {
            get
            {
                return _edtLeft_Text;
            }

            internal set
            {
                if (_edtLeft_Text != value)
                {
                    _edtLeft_Text = value;
                    NotifyPropertyChanged(() => edtLeft_Text);
                }
            }
        }

        /// <summary>
        /// Gets the right text label (file name) displayed over the right diff view (ViewA).
        /// </summary>
        public string edtRight_Text
        {
            get
            {
                return _edtRight_Text;
            }

            internal set
            {
                if (_edtRight_Text != value)
                {
                    _edtRight_Text = value;
                    NotifyPropertyChanged(() => edtRight_Text);
                }
            }
        }
        #endregion Left and Right File Name Labels

        public DiffSideViewModel ViewLineDiff
        {
            get { return _ViewLineDiff; }
        }

        /// <summary>
        /// Gets/sets the height of the bottom panel view that shows diff
        /// of the currently selected line with a 2 line view.
        /// </summary>
        public int LineDiffHeight
        {
            get
            {
                return _LineDiffHeight;
            }

            internal set
            {
                if (_LineDiffHeight != value)
                {
                    _LineDiffHeight = value;
                    NotifyPropertyChanged(() => LineDiffHeight);
                }
            }
        }

        #region Goto Diff Commands
        /// <summary>
        /// Gets a command that positions the diff viewer at the first detected difference.
        /// </summary>
        public ICommand GoToFirstDifferenceCommand
        {
            get
            {
                if (_GoToFirstDifferenceCommand == null)
                {
                    _GoToFirstDifferenceCommand = new RelayCommand<object>((p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = GetActiveView(out nonActView);
                        DiffViewPosition gotoPos = activeView.GetFirstDiffPosition();
                        ScrollToLine(gotoPos, nonActView, activeView, true);
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        bool isEnabled = activeView.CanGoToFirstDiff();

                        return isEnabled;
                    });
                }

                return _GoToFirstDifferenceCommand;
            }
        }

        /// <summary>
        /// Gets a command that positions the diff viewer at the next detected difference.
        /// </summary>
        public ICommand GoToNextDifferenceCommand
        {
            get
            {
                if (_GoToNextDifferenceCommand == null)
                {
                    _GoToNextDifferenceCommand = new RelayCommand<object>((p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = GetActiveView(out nonActView);
                        DiffViewPosition gotoPos = activeView.GetNextDiffPosition();
                        ScrollToLine(gotoPos, nonActView, activeView, true);
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        bool isEnabled = activeView.CanGoToNextDiff();

                        return isEnabled;
                    });
                }

                return _GoToNextDifferenceCommand;
            }
        }

        /// <summary>
        /// Gets a command that positions the diff viewer at a previously detected difference.
        /// </summary>
        public ICommand GoToPrevDifferenceCommand
        {
            get
            {
                if (_GoToPrevDifferenceCommand == null)
                {
                    _GoToPrevDifferenceCommand = new RelayCommand<object>((p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = GetActiveView(out nonActView);
                        DiffViewPosition gotoPos = activeView.GetPrevDiffPosition();
                        ScrollToLine(gotoPos, nonActView, activeView, true);
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        bool isEnabled = activeView.CanGoToPreviousDiff();

                        return isEnabled;
                    });
                }

                return _GoToPrevDifferenceCommand;
            }
        }

        /// <summary>
        /// Gets a command that positions the diff viewer at the last detected difference.
        /// </summary>
        public ICommand GoToLastDifferenceCommand
        {
            get
            {
                if (_GoToLastDifferenceCommand == null)
                {
                    _GoToLastDifferenceCommand = new RelayCommand<object>((p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = GetActiveView(out nonActView);
                        DiffViewPosition gotoPos = activeView.GetLastDiffPosition();
                        ScrollToLine(gotoPos, nonActView, activeView, true);
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        bool isEnabled = activeView.CanGoToLastDiff();

                        return isEnabled;
                    });
                }

                return _GoToLastDifferenceCommand;
            }
        }
        #endregion Goto Diff Commands

        /// <summary>
        /// Gets the total number of labeled lines that are visible in the left view.
        /// This count DOES NOT include imaginary lines that may have
        /// been inserted to bring both texts into a synchronized
        /// view.
        /// 
        /// This count applies therfore, to the maximum number of LABELED
        /// lines in the left side view and is equal to the number of lines
        /// number of lines in the original text.
        /// 
        /// This number is used to instruct the Goto Line dialog to jump to
        /// a particular line and left and right side the view synchronization
        /// ensures scrolling to the correct line even if that line label is different
        /// in the right text view.
        /// </summary>
        public uint NumberOfLines
        {
            get
            {
                return _NumberOfLines;
            }

            protected set
            {
                if (_NumberOfLines != value)
                {
                    _NumberOfLines = value;
                    NotifyPropertyChanged(() => NumberOfLines);
                }
            }
        }

        /// <summary>
        /// Gets the total number of lines available in left and right text view.
        /// 
        /// This number includes Imaginary lines and is always applicable to the
        /// left and right view since both view displays are synchronized via Meyers Diff.
        /// </summary>
        public uint MaxNumberOfLines
        {
            get
            {
                return _MaxNumberOfLines;
            }

            protected set
            {
                if (_MaxNumberOfLines != value)
                {
                    _MaxNumberOfLines = value;
                    NotifyPropertyChanged(() => MaxNumberOfLines);
                }
            }
        }

        /// <summary>
        /// Gets the type of the items being compared
        /// <see cref="DiffType.File"/>, <see cref="DiffType.Text"/> or <see cref="DiffType.Directory"/>
        /// </summary>
        public DiffType DiffType
        {
            get
            {
                return _Args.DiffType;
            }
        }

        public string ToolTipText
        {
            get
            {
                string result = null;

                if (_Args != null)
                {
                    result = _Args.A + Environment.NewLine + _Args.B;
                }

                return result;
            }
        }

        public string StatusText
        {
            get
            {
                return _StatusText;
            }

            protected set
            {
                if (_StatusText != value)
                {
                    _StatusText = value;
                    NotifyPropertyChanged(() => StatusText);
                }
            }
        }
        #endregion properties

        #region methods
        public void ShowDifferences(ShowDiffArgs args)
        {
            IList<string> a, b;
            int leadingCharactersToIgnore = 0;
            bool fileNames = (args.DiffType == DiffType.File);
            if (fileNames)
            {
                GetFileLines(args.A, args.B, out a, out b, out leadingCharactersToIgnore, args);
            }
            else
            {
                GetTextLines(args.A, args.B, args, out a, out b);
            }

            bool isBinaryCompare = leadingCharactersToIgnore > 0;
            bool ignoreCase = isBinaryCompare ? false : args.IgnoreCase;
            bool ignoreTextWhitespace = isBinaryCompare ? false : args.IgnoreTextWhitespace;
            TextDiff diff = new TextDiff(args.HashType, ignoreCase, ignoreTextWhitespace, leadingCharactersToIgnore, !args.ShowChangeAsDeleteInsert);
            EditScript script = diff.Execute(a, b);

            string captionA = string.Empty;
            string captionB = string.Empty;
            if (fileNames)
            {
                try
                {
                    this.StatusText = string.Format("{0} : {1}", Path.GetFileName(args.A), Path.GetFileName(args.B));
                }
                catch
                {
                    // System.IO throws exception on invalid file name
                }
            }
            else
            {
                this.StatusText = "Text Comparison";
            }

            SetData(a, b, script, args, ignoreCase, ignoreTextWhitespace, isBinaryCompare);

            this.NumberOfLines = (uint)a.Count;
            this.MaxNumberOfLines = (uint)_ViewA.LineCount;
            _Args = args;
        }

        internal void GotoTextLine(uint thisLine)
        {
            if (ViewA != null && ViewB != null)
            {
                int realLineA = ViewA.FindThisTextLine((int)thisLine);
                var modelLineA = ViewA.GotoTextLine(realLineA);

                if (modelLineA.Counterpart.Number.HasValue)
                {
                    int counterPartLine = Math.Min((int)modelLineA.Counterpart.Number+1, ViewB.DocLineDiffs.Count);
                    int realLineB = ViewB.FindThisTextLine(counterPartLine);
                    ViewB.GotoTextLine((int)realLineB);
                }
                else
                {
                    int realLineB = ViewB.FindThisTextLine((int)thisLine);
                    ViewB.GotoTextLine((int)realLineB);
                }
            }
        }

        /// <summary>
        /// Sets up the left and right diff viewmodels which contain line by line information
        /// with reference to textual contents and whether it should be handled as insertion,
        /// deletion, change, or no change when comparing left side (ViewA) with right side (ViewB).
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <param name="script"></param>
        /// <param name="args"></param>
        /// <param name="changeDiffIgnoreCase"></param>
        /// <param name="changeDiffIgnoreWhiteSpace"></param>
        /// <param name="changeDiffTreatAsBinaryLines"></param>
        private void SetData(IList<string> listA, IList<string> listB, EditScript script,
                             ShowDiffArgs args,
                             bool changeDiffIgnoreCase,
                             bool changeDiffIgnoreWhiteSpace,
                             bool changeDiffTreatAsBinaryLines)
        {
            _Args = args;

            ChangeDiffOptions changeDiffOptions = ChangeDiffOptions.None;
            if (changeDiffTreatAsBinaryLines)
            {
                changeDiffOptions |= ChangeDiffOptions.IgnoreBinaryPrefix;
            }
            else
            {
                if (changeDiffIgnoreCase)
                {
                    changeDiffOptions |= ChangeDiffOptions.IgnoreCase;
                }

                if (changeDiffIgnoreWhiteSpace)
                {
                    changeDiffOptions |= ChangeDiffOptions.IgnoreWhitespace;
                }
            }

            _ViewA.ChangeDiffOptions = changeDiffOptions;
            _ViewB.ChangeDiffOptions = changeDiffOptions;
            _ViewLineDiff.ChangeDiffOptions = changeDiffOptions;

            var factory = new LinesFactory();
            factory.SetData(listA, listB, script);

            _ViewA.SetData(args.A, factory.LinesA, factory.TextA, args.SpacesPerTab);
            _ViewB.SetData(args.B, factory.LinesB, factory.TextB, args.SpacesPerTab);

            NotifyPropertyChanged(() => this.IsDiffDataAvailable);

            Debug.Assert(this._ViewA.LineCount == this._ViewB.LineCount, "Both DiffView's LineCounts must be the same");

            // Sets the similarity value (0% - 100%) between 2 things shown in toolbar
            this.Similarity_Text = string.Format("{0:P}", script.Similarity);

            // Show left and right file name labels over each ViewA and ViewB
            bool showNames = !string.IsNullOrEmpty(args.A) || !string.IsNullOrEmpty(args.B);
            this.edtLeft_Right_Visible = showNames;

            if (showNames)
            {
                this.edtLeft_Text = args.A;
                this.edtRight_Text = args.B;
            }

            this.currentDiffLine = -1;
            this.UpdateViewLineDiff(args.SpacesPerTab);    // Update 2 line diff ViewLineDiff
        }

        /// <summary>
        /// Update the 2 line display (ViewLineDiff) that contains the display
        /// of the currently selected line in 2 rows rather than side by side.
        /// </summary>
        private void UpdateViewLineDiff(int spacesPerTab)
        {
            // Determine zero based current cursor position in line n
            int line = Math.Max(0, SynchronizedLine - 1);
            if (line == this.currentDiffLine)
            {
                return;
            }

            this.currentDiffLine = line;

            DiffLineViewModel lineOneVM = null;
            DiffLineViewModel lineTwoVM = null;

            lineOneVM = this.ViewA.GetLine(line);
            lineTwoVM = this.ViewB.GetLine(line);

            if (lineOneVM != null && lineTwoVM != null)
                this._ViewLineDiff.SetData(lineOneVM, lineTwoVM, spacesPerTab);
        }

        /// <summary>
        /// Gets the view (of the 2 side by side views) that was activated last
        /// (had the focus the last time)
        /// </summary>
        /// <returns></returns>
        internal DiffSideViewModel GetActiveView(out DiffSideViewModel nonActiveView)
        {
            nonActiveView = null;

            if (ViewA == null && ViewB == null)
                return null;

            if (ViewA == null)
                return ViewB;

            if (ViewB == null)
                return ViewA;

            if (ViewA.ViewActivation < ViewB.ViewActivation)
            {
                nonActiveView = ViewA;
                return ViewB;
            }

            nonActiveView = ViewB;
            return ViewA;
        }

        /// <summary>
        /// Moves both views to the requested line position.
        /// </summary>
        /// <param name="gotoPos"></param>
        /// <param name="viewA"></param>
        /// <param name="viewB"></param>
        /// <param name="positionCursor"></param>
        internal void ScrollToLine(DiffViewPosition gotoPos,
                                   DiffSideViewModel viewA,
                                   DiffSideViewModel viewB,
                                   bool positionCursor)
        {
            if (viewA != null)
            {
                viewA.ScrollToLine(gotoPos.Line + 1, positionCursor);
                viewA.SetPosition(gotoPos);
            }

            if (viewB != null)
            {
                viewB.ScrollToLine(gotoPos.Line + 1, positionCursor);
                viewB.SetPosition(gotoPos);
            }
        }

        /// <summary>
        /// Is invoked when the cursor position in view B has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewACaretPositionChanged(object sender, CaretPositionChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case CaretChangeType.ColumnAndLine:
                    SynchronizedLine = e.Line;
                    SynchronizedColumn = e.Column;
                    break;
                case CaretChangeType.Column:
                    SynchronizedColumn = e.Column;
                    break;
                case CaretChangeType.Line:
                    SynchronizedLine = e.Line;
                    break;
                default:
                    throw new NotImplementedException(e.ChangeType.ToString());
            }

            if (e.ChangeType == CaretChangeType.Line || e.ChangeType == CaretChangeType.ColumnAndLine)
            {
                UpdateViewLineDiff(4);
                UpdateOtherView(e, ViewB);
            }
        }

        /// <summary>
        /// Is invoked when the cursor position in view A has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewBCaretPositionChanged(object sender, CaretPositionChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case CaretChangeType.ColumnAndLine:
                    SynchronizedLine = e.Line;
                    SynchronizedColumn = e.Column;
                    break;
                case CaretChangeType.Column:
                    SynchronizedColumn = e.Column;
                    break;
                case CaretChangeType.Line:
                    SynchronizedLine = e.Line;
                    break;
                default:
                    throw new NotImplementedException(e.ChangeType.ToString());
            }

            if (e.ChangeType == CaretChangeType.Line || e.ChangeType == CaretChangeType.ColumnAndLine)
            {
                UpdateViewLineDiff(4);
                UpdateOtherView(e, ViewA);
            }
        }

        private void UpdateOtherView(CaretPositionChangedEvent e,
                                     DiffSideViewModel otherView)
        {
            if (otherView != null)
            {
                switch (e.ChangeType)
                {
                    case CaretChangeType.ColumnAndLine:
                        otherView.Line = e.Line;
                        otherView.Column = e.Column;
                        break;
                    case CaretChangeType.Column:
                        otherView.Column = e.Column;
                        break;
                    case CaretChangeType.Line:
                        otherView.Line = e.Line;
                        break;
                    default:
                        throw new NotImplementedException(e.ChangeType.ToString());
                }
            }
        }

        #region TextLineConverter
        private static void GetFileLines(string fileNameA, string fileNameB,
                                         out IList<string> a, out IList<string> b,
                                         out int leadingCharactersToIgnore, ShowDiffArgs args)
        {
            a = null;
            b = null;
            leadingCharactersToIgnore = 0;
            CompareType compareType = args.CompareType;
            bool isAuto = compareType == CompareType.Auto;

            if (compareType == CompareType.Binary ||
                (isAuto && (DiffUtility.IsBinaryFile(fileNameA) || DiffUtility.IsBinaryFile(fileNameB))))
            {
                using (FileStream fileA = File.OpenRead(fileNameA))
                using (FileStream fileB = File.OpenRead(fileNameB))
                {
                    BinaryDiff diff = new BinaryDiff
                    {
                        FootprintLength = args.BinaryFootprintLength
                    };

                    AddCopyCollection addCopy = diff.Execute(fileA, fileB);

                    BinaryDiffLines lines = new BinaryDiffLines(fileA, addCopy, args.BinaryFootprintLength);
                    a = lines.BaseLines;
                    b = lines.VersionLines;
                    leadingCharactersToIgnore = BinaryDiffLines.PrefixLength;
                }
            }

            if (compareType == CompareType.Xml || (isAuto && (a == null || b == null)))
            {
                a = TryGetXmlLines(DiffUtility.GetXmlTextLines, fileNameA, fileNameA, !isAuto, args);

                // If A failed to parse with Auto, then there's no reason to try B.
                if (a != null)
                {
                    b = TryGetXmlLines(DiffUtility.GetXmlTextLines, fileNameB, fileNameB, !isAuto, args);
                }

                // If we get here and the compare type was XML, then both
                // inputs parsed correctly, and both lists should be non-null.
                // If we get here and the compare type was Auto, then one
                // or both lists may be null, so we'll fallthrough to the text
                // handling logic.
            }

            if (a == null || b == null)
            {
                a = DiffUtility.GetFileTextLines(fileNameA);
                b = DiffUtility.GetFileTextLines(fileNameB);
            }
        }

        private static void GetTextLines(string textA, string textB, ShowDiffArgs args
                                       , out IList<string> a, out IList<string> b)
        {
            a = null;
            b = null;
            CompareType compareType = args.CompareType;
            bool isAuto = compareType == CompareType.Auto;

            if (compareType == CompareType.Xml || isAuto)
            {
                a = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the left side text", textA, !isAuto, args);

                // If A failed to parse with Auto, then there's no reason to try B.
                if (a != null)
                {
                    b = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the right side text", textB, !isAuto, args);
                }

                // If we get here and the compare type was XML, then both
                // inputs parsed correctly, and both lists should be non-null.
                // If we get here and the compare type was Auto, then one
                // or both lists may be null, so we'll fallthrough to the text
                // handling logic.
            }

            if (a == null || b == null)
            {
                a = DiffUtility.GetStringTextLines(textA);
                b = DiffUtility.GetStringTextLines(textB);
            }
        }

        private static IList<string> TryGetXmlLines(
            Func<string, bool, IList<string>> converter,
            string name,
            string input,
            bool throwOnError,
            ShowDiffArgs args)
        {
            IList<string> result = null;
            try
            {
                result = converter(input, args.IgnoreXmlWhitespace);
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
                    _ViewA.Dispose();
                    _ViewB.Dispose();
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