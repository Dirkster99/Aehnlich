namespace AehnlichViewModelsLib.ViewModels
{
	using System.ComponentModel;
	using System.Windows.Input;

	/// <summary>
	/// Defines base properties and methods that should be implemented by all inline dialogs.
	/// </summary>
	public interface IControllerBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets a command that closes the dialog via the delegate function
		/// that was supplied at constructor time of this object.
		/// </summary>
		ICommand CloseDialogCommand { get; }
	}


	/// <summary>
	/// Defines public properties and methods of the Goto Line Dialog Controller ViewModel.
	/// </summary>
	public interface IOptionsControllerViewModel : IControllerBase, INotifyPropertyChanged
	{

	}

	/// <summary>
	/// Defines public properties and methods of the Goto Line Dialog Controller ViewModel.
	/// </summary>
	public interface IGotoLineControllerViewModel : IControllerBase, INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the minimum linenumber value.
		/// </summary>
		uint MinLineValue { get; set; }

		/// <summary>
		/// Gets the maximum linenumber value.
		/// </summary>
		uint MaxLineValue { get; set; }

		/// <summary>
		/// Gets the actual linenumber value.
		/// </summary>
		uint Value { get; set; }

		/// <summary>
		/// Gets a tool tip that hints legal values using minium and maximum as bounds.
		/// </summary>
		string GotoLineToolTip { get; }

		/// <summary>
		/// Gets a command to scroll a view to the requested line number
		/// via the Action that was supplied at constructor time of of the object.
		/// </summary>
		ICommand GotoThisLineCommand { get; }
	}
}