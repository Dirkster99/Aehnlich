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

        #endregion properties

        #region methods

        internal void SetEditScript(IList<ISegment> segments)
        {
            if (_LineEditScriptSegments == null)
                _LineEditScriptSegments = new ObservableRangeCollection<ISegment>();
            else
                _LineEditScriptSegments.Clear();

            _LineEditScriptSegments.AddRange(segments);
        }
        #endregion methods
    }
}
