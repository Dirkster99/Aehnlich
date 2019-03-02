namespace AehnlichLibViewModels.ViewModels
{
    using System.Collections.Generic;
    using AehnlichLib.Enums;
    using AehnlichLib.Text;
    using AehnlichLibViewModels.Enums;
    using AehnlichLibViewModels.Models;
    using AehnlichViewLib.Enums;
    using AehnlichViewLib.Interfaces;
    using ICSharpCode.AvalonEdit.Document;

    public class DiffLineViewModel : Base.ViewModelBase, IDiffLineInfo
    {
        #region fields
        private DiffContext _Context;

        private ObservableRangeCollection<ISegment> _LineEditScriptSegments = null;
        private bool _LineEditScriptSegmentsIsDirty;
        private DiffLineViewModel _counterPart;
        private readonly DiffViewLine _Model;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="lineContext"></param>
        /// <param name="item"></param>
        internal DiffLineViewModel(DiffContext lineContext, DiffViewLine item)
            : this()
        {
            _Context = lineContext;
            _Model = item;
        }

        /// <summary>
        /// Hidden standard class constructor
        /// </summary>
        protected DiffLineViewModel()
        {
            _Context = DiffContext.Blank;
            _LineEditScriptSegmentsIsDirty = true;
        }
        #endregion ctors

        #region properties
        public DiffContext Context
        {
            get { return _Context; }
            protected set
            {
                if (_Context != value)
                {
                    _Context = value;
                    NotifyPropertyChanged(() => Context);
                }
            }
        }

        public int? ImaginaryLineNumber
        {
            get
            {
                if (_Model != null)
                    return _Model.Number;

                return null;
            }
        }

        public IReadOnlyCollection<ISegment> LineEditScriptSegments
        {
            get
            {
                return _LineEditScriptSegments;
            }
        }

        public bool FromA
        {
            get
            {
                if (_Model != null)
                    return _Model.FromA;

                return false;
            }
        }

        public bool LineEditScriptSegmentsIsDirty
        {
            get { return _LineEditScriptSegmentsIsDirty; }
            set
            {
                if (_LineEditScriptSegmentsIsDirty != value)
                {
                    _LineEditScriptSegmentsIsDirty = value;
                    NotifyPropertyChanged(() => LineEditScriptSegmentsIsDirty);
                }
            }
        }

        public DiffLineViewModel Counterpart
        {
            get { return _counterPart;  }
            internal set
            {
                if (_counterPart != value)
                {
                    _counterPart = value;
                    NotifyPropertyChanged(() => Counterpart);

                    if (_Model != null)
                        _Model.Counterpart = value._Model;
                }
            }
        }

        public string Text
        {
            get
            {
                if (_Model == null)
                    return null;

                return _Model.Text;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Re(sets) the list of text line <see cref="ISegment"/>s on demand when the
        /// user scrolls to a certain location and brings the line into view.
        /// 
        /// Sets <see cref="LineEditScriptSegmentsIsDirty"/> to true to signify that the
        /// script is already cached away.
        /// </summary>
        /// <param name="segments"></param>
        internal void SetEditScript(IList<ISegment> segments)
        {
            if (_LineEditScriptSegments == null)
                _LineEditScriptSegments = new ObservableRangeCollection<ISegment>();
            else
                _LineEditScriptSegments.Clear();

            if (segments != null)
                _LineEditScriptSegments.AddRange(segments);

            LineEditScriptSegmentsIsDirty = false;
        }

        internal EditScript GetChangeEditScript(ChangeDiffOptions changeDiffOptions, int spacesPerTab)
        {
            var editScript = _Model.GetChangeEditScript(changeDiffOptions);

            if (editScript != null)     // Cache the line segments for display
            {
                var segments = GetChangeSegments(editScript, _Model.Text, _Model.FromA, spacesPerTab);
                SetEditScript(segments);
            }
            else
                SetEditScript(null);  // Make sure its been set even if empty

            return editScript;
        }

        private IList<ISegment>
                GetChangeSegments(EditScript changeEditScript,
                                  string originalLineText,
                                  bool useA,
                                  int spacesPerTab)
        {
            var result = new List<ISegment>();

            int previousOriginalTextIndex = 0;
            int previousDisplayTextIndex = 0;

            foreach (Edit edit in changeEditScript)
            {
                if ((useA && edit.EditType == EditType.Delete) ||
                    (!useA && edit.EditType == EditType.Insert))
                {
                    int startOriginalTextIndex = useA ? edit.StartA : edit.StartB;
                    int startDisplayTextIndex = this.GetDisplayTextIndex(startOriginalTextIndex, previousOriginalTextIndex, previousDisplayTextIndex, originalLineText, spacesPerTab);

                    int endOriginalTextIndex = startOriginalTextIndex + edit.Length;
                    int endDisplayTextIndex = this.GetDisplayTextIndex(endOriginalTextIndex, startOriginalTextIndex, startDisplayTextIndex, originalLineText, spacesPerTab);

                    previousOriginalTextIndex = endOriginalTextIndex;
                    previousDisplayTextIndex = endDisplayTextIndex;

                    result.Add(new Segment(startDisplayTextIndex, endDisplayTextIndex - startDisplayTextIndex, endDisplayTextIndex));
                }
            }

            return result;
        }

        private int GetDisplayTextIndex(int originalTextIndex,
                                        int previousOriginalTextIndex,
                                        int previousDisplayTextIndex,
                                        string originalLineText,
                                        int spacesPerTab)
        {
            int result = previousDisplayTextIndex;

            string originalText = originalLineText;
            int maxLength = originalText.Length;

            for (int i = previousOriginalTextIndex; i < maxLength && i < originalTextIndex; i++)
            {
                result += this.GetCharWidth(originalText[i], result, spacesPerTab);
            }

            return result;
        }

        private int GetCharWidth(char ch, int columnStart, int spacesPerTab)
        {
            int result = 1;
            int position = columnStart + result;

            if (ch == '\t')
            {
                // We've already counted the tab as one character, but now we need to add on spaces
                // to expand to the next multiple of this.spacesPerTab.  The mod remainder tells us how
                // many spaces of the current tab that we've used so far.
                // Examples (with SpacesPerTab == 4):
                //  position = 5 --> mod == 1 --> need 3 spaces
                //  position = 16 --> mod == 0 --> need 0 spaces
                //  position = 18 --> mod == 2 --> need 2 spaces
                //  position = 23 --> mod == 1 --> need 1 space
                int tabSpacesUsed = position % spacesPerTab;
                if (tabSpacesUsed > 0)
                {
                    int extraSpacesNeeded = spacesPerTab - tabSpacesUsed;
                    result += extraSpacesNeeded;
                }
            }

            return result;
        }
        #endregion methods
    }
}
