namespace AehnlichLibViewModels.ViewModels
{
    using System;
    using System.Windows.Input;
    using AehnlichLibViewModels.Enums;
    using Base;

    public class GotoLineControllerViewModel : Base.ViewModelBase
    {
        #region fields
        private uint _MaxLineValue;
        private ICommand _GotoThisLineCommand;
        private Action<uint> _gotoLineAction;
        private uint _MinLineValue;
        private uint _Value;
        private ICommand _CloseGotoThisLineCommand;
        private Func<InlineDialogMode, InlineDialogMode> _closeFunc;
        #endregion fields

        #region ctor

        public GotoLineControllerViewModel(Action<uint> gotoLine)
            : this()
        {
            _gotoLineAction = gotoLine;
        }

        public GotoLineControllerViewModel(Action<uint> gotoLine,
                                           Func<InlineDialogMode, InlineDialogMode> closeFunc) : this(gotoLine)                      
        {
            _closeFunc = closeFunc;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected GotoLineControllerViewModel()
        {
            _MinLineValue = 1;
            Value = 1;
            MaxLineValue = 1;
        }
        #endregion ctor

        #region properties
        
        public uint MinLineValue
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

        public uint MaxLineValue
        {
            get { return _MaxLineValue; }
            set
            {
                if (_MaxLineValue != value)
                {
                    _MaxLineValue = value;
                    NotifyPropertyChanged(() => MaxLineValue);
                    NotifyPropertyChanged(() => MinLineValue);
                    NotifyPropertyChanged(() => GotoLineToolTip);
                }
            }
        }


        public uint Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    NotifyPropertyChanged(() => Value);
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
                    },(p) =>
                    {
                        if (MinLineValue < MaxLineValue)
                            return true;

                        return false;
                    });
                }

                return _GotoThisLineCommand;
            }
        }

        public ICommand CloseGotoThisLineCommand
        {
            get
            {
                if (_CloseGotoThisLineCommand == null)
                {
                    _CloseGotoThisLineCommand = new RelayCommand<object>((p) =>
                    {
                        _closeFunc(InlineDialogMode.None);
                    },
                    (p) =>
                    {
                        return (_closeFunc == null ? false : true);
                    });
                }

                return _CloseGotoThisLineCommand;
            }
        }

        
        #endregion properties
    }
}