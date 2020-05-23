namespace AehnlichViewModelsLib.ViewModels.Suggest
{
	using System.Collections.Generic;

	/// <summary>
	/// Implements a simple model to capture the result a query towards an entered text.
	/// </summary>
	public class SuggestQueryResultModel
	{
		/// <summary>
		/// Class constructor
		/// </summary>
		public SuggestQueryResultModel(List<object> lst, bool validInput = true)
		{
			ResultList = lst;
			ValidInput = validInput;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public SuggestQueryResultModel()
		{
			ResultList = new List<object>();
			ValidInput = true;
		}

		/// <summary>
		/// Gets the list of objects that represents a likely suggestion towards an entered text.
		/// </summary>
		public List<object> ResultList { get; }

		/// <summary>
		/// specifies whether the entered text was considered valid or not.
		/// </summary>
		public bool ValidInput { get; }
	}
}
