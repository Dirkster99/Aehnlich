namespace AehnlichViewModelsLib.ViewModels.Suggest
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Threading.Tasks;
	using System.Windows.Input;

	/// <summary>
	/// Defines a suggestion object to generate suggestions
	/// based on sub entries of specified string.
	/// </summary>
	public interface ISuggestSourceViewModel : IDisposable, INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the path of file A in the comparison.
		/// </summary>
		string FilePath { get; set; }

		/// <summary>
		/// Gets whether the last query text was valid or invalid.
		/// </summary>
		bool IsTextValid { get; }

		/// <summary>
		/// Gets a collection of items that represent likely suggestions towards
		/// a previously entered text.
		/// </summary>
		IEnumerable<object> ListQueryResult { get; }

		/// <summary>
		/// Gets a command that queries a sub-system in order to resolve a query
		/// based on a previously entered text. The entered text is expected as
		/// parameter of this command.
		/// </summary>
		ICommand SuggestTextChangedCommand { get; }

		/// <summary>
		/// Method returns a task that returns a list of suggestion objects
		/// that are associated to the <paramref name="input"/> string.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		Task<SuggestQueryResultModel> SuggestAsync(string input);
	}
}