namespace AehnlichLibViewModels.ViewModels
{
    using System.Collections.Generic;
    using AehnlichViewLib.Enums;
    using AehnlichViewLib.Interfaces;
    using ICSharpCode.AvalonEdit.Document;

    public class DiffLineInfoViewModel : Base.ViewModelBase, IDiffLineInfo
    {
        #region fields
        private DiffContext _Context;
        private int? _ImaginearyLineNumber;

        private ObservableRangeCollection<ISegment> _LineEditScriptSegments = null;
        private bool _FromA;
        private bool _LineEditScriptSegmentsIsDirty;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="contextOfDiff"></param>
        /// <param name="imaginaryLineNumber"></param>
        /// <param name="fromA"></param>
        public DiffLineInfoViewModel(DiffContext contextOfDiff
                                   , int? imaginaryLineNumber, bool fromA)
            : this()
        {
            Context = contextOfDiff;
            ImaginaryLineNumber = imaginaryLineNumber;
            _FromA = fromA;
        }

        /// <summary>
        /// Hidden standard class constructor
        /// </summary>
        protected DiffLineInfoViewModel()
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
            get { return _ImaginearyLineNumber; }
            protected set
            {
                if (_ImaginearyLineNumber != value)
                {
                    _ImaginearyLineNumber = value;
                    NotifyPropertyChanged(() => ImaginaryLineNumber);
                }
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
            get { return _FromA; }
            protected set
            {
                if (_FromA != value)
                {
                    _FromA = value;
                    NotifyPropertyChanged(() => FromA);
                }
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
        #endregion methods
    }
}
