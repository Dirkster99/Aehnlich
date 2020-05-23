namespace Aehnlich.ViewModels.DirectoryDiff
{
	using Aehnlich.ViewModels.Documents;
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// This <see cref="DataTemplateSelector"/> determines the view to be shown for each viewmodel
	/// that is associated with the different stages of the <see cref="DirDiffDocViewModel"/>.
	/// </summary>
	internal class DirDiffContentTemplateSelector : DataTemplateSelector
	{
		/// <summary>
		/// Gets the view that is used to adjust options
		/// and settings for a particular directory comparison.
		/// </summary>
		public DataTemplate SetupPage { get; set; }

		/// <summary>
		/// Gets the view that is used to display directory diff results and browse the
		/// directory structure.
		/// </summary>
		public DataTemplate ViewPage { get; set; }

		/// <summary>
		/// Returns a <see cref="DataTemplate"/> based on custom logic.
		/// </summary>
		/// <param name="item">The data object for which to select the template.</param>
		/// <param name="container">The data-bound object.</param>
		/// <returns>Returns a <see cref="DataTemplate"/> or null. The default value is null.</returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is DirDiffDocViewViewModel)
				return ViewPage;

			if (item is DirDiffDocSetupViewModel)
				return SetupPage;

			return base.SelectTemplate(item, container);
		}
	}
}
