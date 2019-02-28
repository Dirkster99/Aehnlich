namespace AehnlichLibViewModels.ViewModels
{
    using AehnlichViewLib.Enums;
    using AehnlichViewLib.Interfaces;

    public class DiffLineInfoViewModel : Base.ViewModelBase, IDiffLineInfo
    {
        #region fields
        private DiffContext _Context;
        private int? _ImaginearyLineNumber;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="contextOfDiff"></param>
        /// <param name="imaginaryLineNumber"></param>
        public DiffLineInfoViewModel(DiffContext contextOfDiff
                                   , int? imaginaryLineNumber)
            : this()
        {
            Context = contextOfDiff;
            ImaginaryLineNumber = imaginaryLineNumber;
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
        #endregion properties

        #region methods

        #endregion methods
    }
}
