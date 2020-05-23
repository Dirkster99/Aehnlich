namespace AehnlichViewModelsLib.ViewModels.Dialogs
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// This <see cref="DataTemplateSelector"/> determines the view to be shown for each viewmodel
	/// that is associated with the different inline dialogs.
	/// </summary>
	internal class DialogtTemplateSelector : DataTemplateSelector
	{
		/// <summary>
		/// Gets the view that is used to adjust options
		/// and settings for a particular directory comparison.
		/// </summary>
		public DataTemplate GotoLineDialog { get; set; }

		/// <summary>
		/// Gets the view that is used to display directory diff results and browse the
		/// directory structure.
		/// </summary>
		public DataTemplate OptionsDialog { get; set; }

		/// <summary>
		/// Returns a <see cref="DataTemplate"/> based on custom logic.
		/// </summary>
		/// <param name="item">The data object for which to select the template.</param>
		/// <param name="container">The data-bound object.</param>
		/// <returns>Returns a <see cref="DataTemplate"/> or null. The default value is null.</returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is GotoLineControllerViewModel)
				return GotoLineDialog;

			if (item is OptionsControllerViewModel)
				return OptionsDialog;

			return base.SelectTemplate(item, container);
		}
	}
}
