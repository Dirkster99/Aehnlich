namespace AehnlichLibViewModels.ViewModels
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
    using AehnlichLibViewModels.Enums;
    using AehnlichLibViewModels.Events;
    using AehnlichLibViewModels.Models;
    using AehnlichLibViewModels.ViewModels.Base;
    using AehnlichLibViewModels.ViewModels.LineInfo;
    using ICSharpCode.AvalonEdit;

    /// <summary>
    /// DiffControl
    /// </summary>
    public class DiffDocViewModel : Base.ViewModelBase
    {
        #region fields
        private ShowDiffArgs _currentDiffArgs;
        private int _NumberOfLines;
        private DiffType _DiffType;

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

        internal DiffSideViewModel GetActiveView(out object nonActView)
        {
            throw new NotImplementedException();
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
                        ScrollToLine(gotoPos, nonActView, activeView);
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
                        ScrollToLine(gotoPos, nonActView, activeView);
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
                        ScrollToLine(gotoPos, nonActView, activeView);
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
                        ScrollToLine(gotoPos, nonActView, activeView);
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

        public int NumberOfLines
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
        /// Gets the type of the items being compared
        /// <see cref="DiffType.File"/>, <see cref="DiffType.Text"/> or <see cref="DiffType.Directory"/>
        /// </summary>
        public DiffType DiffType
        {
            get
            {
                return _DiffType;
            }

            protected set
            {
                if (_DiffType != value)
                {
                    _DiffType = value;
                    NotifyPropertyChanged(() => DiffType);
                    NotifyPropertyChanged(() => ToolTipText);
                }
            }
        }

        public string ToolTipText
        {
            get
            {
                string result = null;

                if (_currentDiffArgs != null)
                {
                    result = _currentDiffArgs.A + Environment.NewLine + _currentDiffArgs.B;
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
            this.DiffType = args.DiffType;


            IList<string> a, b;
            int leadingCharactersToIgnore = 0;
            bool fileNames = (this.DiffType == DiffType.File);
            if (fileNames)
            {
                GetFileLines(args.A, args.B, out a, out b, out leadingCharactersToIgnore);
            }
            else
            {
                GetTextLines(args.A, args.B, out a, out b);
            }

            bool isBinaryCompare = leadingCharactersToIgnore > 0;
            bool ignoreCase = isBinaryCompare ? false : Options.IgnoreCase;
            bool ignoreTextWhitespace = isBinaryCompare ? false : Options.IgnoreTextWhitespace;
            TextDiff diff = new TextDiff(Options.HashType, ignoreCase, ignoreTextWhitespace, leadingCharactersToIgnore, !Options.ShowChangeAsDeleteInsert);
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

            // Apply options first since SetData needs to know things
            // like SpacesPerTab and ShowWhitespace up front, so it
            // can build display lines, determine scroll bounds, etc.
////            this.ApplyOptions();

////            this.FileNameA = args.A;
////            this.FileNameB = args.B;

////            this.IgnoreCase = ignoreCase;
////            this.IgnoreTextWhitespace = ignoreTextWhitespace;
////            this.IsBinaryCompare = isBinaryCompare;

            SetData(a, b, script, args, ignoreCase, ignoreTextWhitespace, isBinaryCompare);

            NumberOfLines = a.Count;
            this._currentDiffArgs = args;
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
            _currentDiffArgs = args;

            const int spacesPerTab = 4;
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

            _ViewA.SetData(args.A, factory.LinesA, factory.TextA);
            _ViewB.SetData(args.B, factory.LinesB, factory.TextB);

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
            this.UpdateViewLineDiff(spacesPerTab);    // Update 2 line diff ViewLineDiff
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
        internal void ScrollToLine(DiffViewPosition gotoPos,
                                   DiffSideViewModel viewA,
                                   DiffSideViewModel viewB)
        {
            if (viewA != null)
            {
                viewA.ScrollToLine(gotoPos.Line + 1);
                viewA.SetPosition(gotoPos);
            }

            if (viewB != null)
            {
                viewB.ScrollToLine(gotoPos.Line + 1);
                viewB.SetPosition(gotoPos);
            }
        }

        /// <summary>
        /// Is invoked whenever the view port of the document changes to ensure all change
        /// script segments per line are available when they are scrolled into the view.
        /// </summary>
        /// <param name="firstLine"></param>
        /// <param name="lastLine"></param>
        /// <param name="spacesPerTab"></param>
        internal int GetChangeEditScript(int firstLine, int lastLine, int spacesPerTab)
        {
            int count = 0;

            count += _ViewA.GetChangeEditScript(firstLine, lastLine, spacesPerTab);
            count += _ViewB.GetChangeEditScript(firstLine, lastLine, spacesPerTab);

            return count;
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
        private static void GetFileLines(string fileNameA, string fileNameB, out IList<string> a, out IList<string> b, out int leadingCharactersToIgnore)
        {
            a = null;
            b = null;
            leadingCharactersToIgnore = 0;
            CompareType compareType = Options.CompareType;
            bool isAuto = compareType == CompareType.Auto;

            if (compareType == CompareType.Binary ||
                (isAuto && (DiffUtility.IsBinaryFile(fileNameA) || DiffUtility.IsBinaryFile(fileNameB))))
            {
                using (FileStream fileA = File.OpenRead(fileNameA))
                using (FileStream fileB = File.OpenRead(fileNameB))
                {
                    BinaryDiff diff = new BinaryDiff
                    {
                        FootprintLength = Options.BinaryFootprintLength
                    };

                    AddCopyCollection addCopy = diff.Execute(fileA, fileB);

                    BinaryDiffLines lines = new BinaryDiffLines(fileA, addCopy, Options.BinaryFootprintLength);
                    a = lines.BaseLines;
                    b = lines.VersionLines;
                    leadingCharactersToIgnore = BinaryDiffLines.PrefixLength;
                }
            }

            if (compareType == CompareType.Xml || (isAuto && (a == null || b == null)))
            {
                a = TryGetXmlLines(DiffUtility.GetXmlTextLines, fileNameA, fileNameA, !isAuto);

                // If A failed to parse with Auto, then there's no reason to try B.
                if (a != null)
                {
                    b = TryGetXmlLines(DiffUtility.GetXmlTextLines, fileNameB, fileNameB, !isAuto);
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

        private static void GetTextLines(string textA, string textB, out IList<string> a, out IList<string> b)
        {
            a = null;
            b = null;
            CompareType compareType = Options.CompareType;
            bool isAuto = compareType == CompareType.Auto;

            if (compareType == CompareType.Xml || isAuto)
            {
                a = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the left side text", textA, !isAuto);

                // If A failed to parse with Auto, then there's no reason to try B.
                if (a != null)
                {
                    b = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the right side text", textB, !isAuto);
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
            bool throwOnError)
        {
            IList<string> result = null;
            try
            {
                result = converter(input, Options.IgnoreXmlWhitespace);
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