namespace DiffLibViewModels.ViewModels
{
    using DiffLib.Binaries;
    using DiffLib.Dir;
    using DiffLib.Text;
    using DiffLibViewModels.Enums;
    using DiffLibViewModels.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    public class FileDiffFormViewModel : Base.ViewModelBase
    {
        #region fields
        private ShowDiffArgs currentDiffArgs;
        private bool _ShowWhiteSpaceInMainDiff;
        private bool _ShowWhiteSpaceInLineDiff;
        private string _StatusText;
        private EditScript _Script;
        private IList<string> _ListA;
        private IList<string> _ListB;
        private string _FileNameA;
        private string _FileNameB;
        private bool _IgnoreCase;
        private bool _IgnoreTextWhitespace;
        private bool _IsBinaryCompare;
        private int _LineDiffHeight;
        private int _NumberOfLines;

        private readonly DiffControlViewModel _DiffCtrl;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public FileDiffFormViewModel()
        {
            Options.OptionsChanged += this.OptionsChanged;
            _DiffCtrl = new DiffControlViewModel();
        }
        #endregion ctors

        #region properties
        public DiffControlViewModel DiffCtrl
        {
            get { return _DiffCtrl; }
        }

        public bool ShowWhiteSpaceInMainDiff
        {
            get
            {
                return _ShowWhiteSpaceInMainDiff;
            }

            protected set
            {
                if (_ShowWhiteSpaceInMainDiff != value)
                {
                    _ShowWhiteSpaceInMainDiff = value;
                    NotifyPropertyChanged(() => ShowWhiteSpaceInMainDiff);
                }
            }
        }

        public bool ShowWhiteSpaceInLineDiff
        {
            get
            {
                return _ShowWhiteSpaceInLineDiff;
            }

            protected set
            {
                if (_ShowWhiteSpaceInLineDiff != value)
                {
                    _ShowWhiteSpaceInLineDiff = value;
                    NotifyPropertyChanged(() => ShowWhiteSpaceInLineDiff);
                }
            }
        }

        public string ToolTipText
        {
            get
            {
                string result = null;

                if (this.currentDiffArgs != null)
                {
                    result = this.currentDiffArgs.A + Environment.NewLine + this.currentDiffArgs.B;
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

        public IList<string> ListA
        {
            get
            {
                return _ListA;
            }

            protected set
            {
                if (_ListA != value)
                {
                    _ListA = value;
                    NotifyPropertyChanged(() => ListA);
                }
            }
        }

        public IList<string> ListB
        {
            get
            {
                return _ListB;
            }

            protected set
            {
                if (_ListB != value)
                {
                    _ListB = value;
                    NotifyPropertyChanged(() => ListB);
                }
            }
        }

        public EditScript Script
        {
            get
            {
                return _Script;
            }

            protected set
            {
                if (_Script != value)
                {
                    _Script = value;
                    NotifyPropertyChanged(() => Script);
                }
            }
        }

        public string FileNameA
        {
            get
            {
                return _FileNameA;
            }

            protected set
            {
                if (_FileNameA != value)
                {
                    _FileNameA = value;
                    NotifyPropertyChanged(() => FileNameA);
                }
            }
        }

        public string FileNameB
        {
            get
            {
                return _FileNameB;
            }

            protected set
            {
                if (_FileNameB != value)
                {
                    _FileNameB = value;
                    NotifyPropertyChanged(() => FileNameB);
                }
            }
        }

        public bool IgnoreCase
        {
            get
            {
                return _IgnoreCase;
            }

            protected set
            {
                if (_IgnoreCase != value)
                {
                    _IgnoreCase = value;
                    NotifyPropertyChanged(() => IgnoreCase);
                }
            }
        }

        public bool IgnoreTextWhitespace
        {
            get
            {
                return _IgnoreTextWhitespace;
            }

            protected set
            {
                if (_IgnoreTextWhitespace != value)
                {
                    _IgnoreTextWhitespace = value;
                    NotifyPropertyChanged(() => IgnoreTextWhitespace);
                }
            }
        }

        public bool IsBinaryCompare
        {
            get
            {
                return _IsBinaryCompare;
            }

            protected set
            {
                if (_IsBinaryCompare != value)
                {
                    _IsBinaryCompare = value;
                    NotifyPropertyChanged(() => IsBinaryCompare);
                }
            }
        }

        public int LineDiffHeight
        {
            get
            {
                return _LineDiffHeight;
            }

            protected set
            {
                if (_LineDiffHeight != value)
                {
                    _LineDiffHeight = value;
                    NotifyPropertyChanged(() => LineDiffHeight);
                }
            }
        }

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
        #endregion properties

        #region methods
        public void ShowDifferences(ShowDiffArgs e)
        {
            string textA = e.A;
            string textB = e.B;
            DiffType diffType = e.DiffType;

            IList<string> a, b;
            int leadingCharactersToIgnore = 0;
            bool fileNames = diffType == DiffType.File;
            if (fileNames)
            {
                GetFileLines(textA, textB, out a, out b, out leadingCharactersToIgnore);
            }
            else
            {
                GetTextLines(textA, textB, out a, out b);
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
                captionA = textA;
                captionB = textB;
                this.StatusText = string.Format("{0} : {1}", Path.GetFileName(textA), Path.GetFileName(textB));
            }
            else
            {
                this.StatusText = "Text Comparison";
            }

            // Apply options first since SetData needs to know things
            // like SpacesPerTab and ShowWhitespace up front, so it
            // can build display lines, determine scroll bounds, etc.
            this.ApplyOptions();

            this.ListA = a;
            this.ListB = b;
            this.Script = script;
            this.FileNameA = captionA;
            this.FileNameB = captionB;

            this.IgnoreCase = ignoreCase;
            this.IgnoreTextWhitespace = ignoreTextWhitespace;
            this.IsBinaryCompare = isBinaryCompare;

            this._DiffCtrl.SetData(a, b, script, captionA, captionB,
                                  ignoreCase, ignoreTextWhitespace, isBinaryCompare);

            if (Options.LineDiffHeight != 0)
            {
                this.LineDiffHeight = Options.LineDiffHeight;
                this._DiffCtrl.LineDiffHeight = Options.LineDiffHeight;
            }

            NumberOfLines = ListA.Count;
            NotifyPropertyChanged(() => DiffCtrl);

            this.currentDiffArgs = e;
        }

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

        private void OptionsChanged(object sender, EventArgs e)
        {
            this.ApplyOptions();
        }

        private void ApplyOptions()
        {
            this.ShowWhiteSpaceInMainDiff = Options.ShowWSInMainDiff;
            this.ShowWhiteSpaceInLineDiff = Options.ShowWSInLineDiff;
////            this.DiffCtrl.ViewFont = Options.ViewFont;
        }
        #endregion methods
    }
}
