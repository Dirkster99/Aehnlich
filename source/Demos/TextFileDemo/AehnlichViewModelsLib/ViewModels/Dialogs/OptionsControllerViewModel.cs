namespace AehnlichViewModelsLib.ViewModels.Dialogs
{
    using AehnlichViewModelsLib.Enums;
    using AehnlichViewModelsLib.ViewModels.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    internal class OptionsControllerViewModel : Base.ViewModelBase, IOptionsControllerViewModel
    {
        #region fields
        readonly private Func<InlineDialogMode, InlineDialogMode> _closeFunc;
        private RelayCommand<object> _CloseDialogCommand;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="closeFunc"></param>
        public OptionsControllerViewModel(Func<InlineDialogMode, InlineDialogMode> closeFunc)
            : this()
        {
            _closeFunc = closeFunc;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected OptionsControllerViewModel()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a command that closes the dialog via the delegate function
        /// that was supplied at constructor time of this object.
        /// </summary>
        public ICommand CloseDialogCommand
        {
            get
            {
                if (_CloseDialogCommand == null)
                {
                    _CloseDialogCommand = new RelayCommand<object>((p) =>
                    {
                        _closeFunc(InlineDialogMode.None);
                    },
                    (p) =>
                    {
                        return (_closeFunc == null ? false : true);
                    });
                }

                return _CloseDialogCommand;
            }
        }
        #endregion properties

        #region methods

        #endregion methods
    }
}
