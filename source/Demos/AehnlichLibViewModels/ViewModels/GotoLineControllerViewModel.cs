namespace AehnlichLibViewModels.ViewModels
{
    using System;
    using System.Windows.Input;
    using Base;

    public class GotoLineControllerViewModel : Base.ViewModelBase
    {
        #region fields
        private int _MaxLineValue;
        private ICommand _GotoThisLineCommand;
        private Action<uint> _gotoLineAction;
        private int _MinLineValue;
        #endregion fields

        #region ctor

        public GotoLineControllerViewModel(Action<uint> gotoLine)
            : this()
        {
            _gotoLineAction = gotoLine;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected GotoLineControllerViewModel()
        {
            MinLineValue = 1;
            MaxLineValue = 1;
        }
        #endregion ctor

        #region properties
        
        public int MinLineValue
        {
            get { return _MinLineValue; }
            set
            {
                if (_MinLineValue != value)
                {
                    _MinLineValue = value;
                    NotifyPropertyChanged(() => MinLineValue);
                    NotifyPropertyChanged(() => GotoLineToolTip);
                }
            }
        }

        public int MaxLineValue
        {
            get { return _MaxLineValue; }
            set
            {
                if (_MaxLineValue != value)
                {
                    _MaxLineValue = value;
                    NotifyPropertyChanged(() => MaxLineValue);
                    NotifyPropertyChanged(() => GotoLineToolTip);
                }
            }
        }

        public string GotoLineToolTip
        {
            get
            {
                return string.Format("Choose a line between {0} and {1}", MinLineValue, MaxLineValue);
            }
        }

        public ICommand GotoThisLineCommand
        {
            get
            {
                if (_GotoThisLineCommand == null)
                {
                    _GotoThisLineCommand = new RelayCommand<object>((p) =>
                    {
                        if ((p is uint) == false)
                            return;

                        var param = (uint)p;

                        _gotoLineAction.Invoke(param);
                    });
                }

                return _GotoThisLineCommand;
            }
        }
        #endregion properties
    }
}