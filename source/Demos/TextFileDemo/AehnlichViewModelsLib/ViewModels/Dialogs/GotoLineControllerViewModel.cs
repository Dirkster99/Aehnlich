namespace AehnlichViewModelsLib.ViewModels.Dialogs
{
	using AehnlichViewModelsLib.Enums;
	using Base;
	using System;
	using System.Windows;
	using System.Windows.Input;

	/// <summary>
	/// Implements a viewmodel that drives the functionality for a Goto Line dialog.
	/// </summary>
	internal class GotoLineControllerViewModel : Base.ViewModelBase, IGotoLineControllerViewModel
	{
		#region fields
		private uint _MaxLineValue;
		private ICommand _GotoThisLineCommand;
		private uint _MinLineValue;
		private uint _Value;
		private ICommand _CloseDialogCommand;

		readonly private Action<uint> _gotoLineAction;
		readonly private Func<InlineDialogMode, InlineDialogMode> _closeFunc;
		#endregion fields

		#region ctor
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="gotoLine"></param>
		/// <param name="closeFunc"></param>
		public GotoLineControllerViewModel(Action<uint> gotoLine,
										   Func<InlineDialogMode, InlineDialogMode> closeFunc)
			: this(gotoLine)
		{
			_closeFunc = closeFunc;
		}

		/// <summary>
		/// <summary>
		/// Class constructor
		/// </summary>
		/// </summary>
		/// <param name="gotoLine"></param>
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
			_MinLineValue = 1;
			Value = 1;
			MaxLineValue = 1;
		}
		#endregion ctor

		#region properties
		/// <summary>
		/// Gets the minimum linenumber value.
		/// </summary>
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

		/// <summary>
		/// Gets the maximum linenumber value.
		/// </summary>
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

		/// <summary>
		/// Gets the actual linenumber value.
		/// </summary>
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

		/// <summary>
		/// Gets a tool tip that hints legal values using minium and maximum as bounds.
		/// </summary>
		public string GotoLineToolTip
		{
			get
			{
				return string.Format("Choose a line between {0} and {1}", MinLineValue, MaxLineValue);
			}
		}

		/// <summary>
		/// Gets a command to scroll a view to the requested line number
		/// via the <see cref="Action"/> that was supplied at constructor
		/// time of this object.
		/// </summary>
		public ICommand GotoThisLineCommand
		{
			get
			{
				if (_GotoThisLineCommand == null)
				{
					_GotoThisLineCommand = new RelayCommand<object>((p) =>
					{
						if ((p is uint) == true)
						{
							_gotoLineAction.Invoke((uint)p);
							return;
						}
						else
						{
							if (p is object[])
							{
								var param = p as object[];

								if (param[0] is uint && param[1] is UIElement)
								{
									_gotoLineAction.Invoke((uint)param[0]);

									(param[1] as UIElement).Focus();     // (Re)Focus this after execution
									Keyboard.Focus(param[1] as UIElement);

									return;
								}
							}
						}

					}, (p) =>
					 {
						 if (MinLineValue < MaxLineValue)
							 return true;

						 return false;
					 });
				}

				return _GotoThisLineCommand;
			}
		}

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
	}
}