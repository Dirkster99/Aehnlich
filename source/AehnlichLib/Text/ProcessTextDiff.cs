namespace AehnlichLib.Text
{
    using AehnlichLib.Binaries;
    using AehnlichLib.Dir;
    using AehnlichLib.Enums;
    using AehnlichLib.Interfaces;
    using AehnlichLib.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    public class ProcessTextDiff
    {
        #region fields
        private readonly ShowDiffArgs _Args;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public ProcessTextDiff(ShowDiffArgs args)
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
                if (System.IO.File.Exists(_Args.A) == false ||
                    System.IO.File.Exists(_Args.B) == false)
                    return progress;

                IList<string> a, b;
                int leadingCharactersToIgnore = 0;

                if (_Args.DiffType == DiffType.File)
                {
                    GetFileLines(_Args.A, _Args.B, out a, out b, out leadingCharactersToIgnore, _Args, progress);
                }
                else
                {
                    GetTextLines(_Args.A, _Args.B, _Args, out a, out b, progress);
                }

                IsBinaryCompare = leadingCharactersToIgnore > 0;
                IgnoreCase = IsBinaryCompare ? false : _Args.IgnoreCase;
                IgnoreTextWhitespace = IsBinaryCompare ? false : _Args.IgnoreTextWhitespace;
                TextDiff diff = new TextDiff(_Args.HashType, IgnoreCase, IgnoreTextWhitespace, leadingCharactersToIgnore, !_Args.ShowChangeAsDeleteInsert);

                ListA = a;
                ListB = b;
                Script = diff.Execute(a, b);

                progress.ResultData = this;

                return progress;
            }
            finally
            {
                progress.ProgressDisplayOff();
            }
        }

        #region TextLineConverter
        private static void GetFileLines(string fileNameA, string fileNameB,
                                         out IList<string> a, out IList<string> b,
                                         out int leadingCharactersToIgnore,
                                         ShowDiffArgs args,
                                         IDiffProgress progress)
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
                a = TryGetXmlLines(DiffUtility.GetXmlTextLines, fileNameA, fileNameA, !isAuto, args, progress);

                // If A failed to parse with Auto, then there's no reason to try B.
                if (a != null)
                {
                    b = TryGetXmlLines(DiffUtility.GetXmlTextLines, fileNameB, fileNameB, !isAuto, args, progress);
                }

                // If we get here and the compare type was XML, then both
                // inputs parsed correctly, and both lists should be non-null.
                // If we get here and the compare type was Auto, then one
                // or both lists may be null, so we'll fallthrough to the text
                // handling logic.
            }

            if (a == null || b == null)
            {
                a = DiffUtility.GetFileTextLines(fileNameA, progress);
                b = DiffUtility.GetFileTextLines(fileNameB, progress);
            }
        }

        private static void GetTextLines(string textA, string textB, ShowDiffArgs args,
                                         out IList<string> a, out IList<string> b,
                                         IDiffProgress progress)
        {
            a = null;
            b = null;
            CompareType compareType = args.CompareType;
            bool isAuto = compareType == CompareType.Auto;

            if (compareType == CompareType.Xml || isAuto)
            {
                a = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the left side text", textA, !isAuto, args, progress);

                // If A failed to parse with Auto, then there's no reason to try B.
                if (a != null)
                {
                    b = TryGetXmlLines(DiffUtility.GetXmlTextLinesFromXml, "the right side text", textB, !isAuto, args, progress);
                }

                // If we get here and the compare type was XML, then both
                // inputs parsed correctly, and both lists should be non-null.
                // If we get here and the compare type was Auto, then one
                // or both lists may be null, so we'll fallthrough to the text
                // handling logic.
            }

            if (a == null || b == null)
            {
                a = DiffUtility.GetStringTextLines(textA, progress);
                b = DiffUtility.GetStringTextLines(textB, progress);
            }
        }

        private static IList<string> TryGetXmlLines(
            Func<string, bool, IDiffProgress, IList<string>> converter,
            string name,
            string input,
            bool throwOnError,
            ShowDiffArgs args,
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
