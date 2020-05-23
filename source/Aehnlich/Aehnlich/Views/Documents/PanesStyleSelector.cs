namespace Aehnlich.Views.Documents
{
	using Aehnlich.Interfaces;
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Provides a way to apply styles based on custom logic.
	/// </summary>
	internal class PanesStyleSelector : StyleSelector
	{
		public Style DocumentStyle
		{
			get;
			set;
		}

		/// <summary>
		/// Returns a System.Windows.Style based on custom logic.
		/// </summary>
		/// <param name="item">The content.</param>
		/// <param name="container">The element to which the style will be applied.</param>
		/// <returns>An application-specific style to apply; otherwise, null.</returns>
		public override Style SelectStyle(object item, System.Windows.DependencyObject container)
		{
			if (item is IDocumentBaseViewModel)
				return DocumentStyle;

			return base.SelectStyle(item, container);
		}
	}
}
