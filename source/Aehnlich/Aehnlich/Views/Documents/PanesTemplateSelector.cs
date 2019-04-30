namespace Aehnlich.Views.Documents
{
    using Aehnlich.ViewModels.Documents;
    using System.Windows;
    using System.Windows.Controls;
    using Xceed.Wpf.AvalonDock.Layout;

    /// <summary>
    /// Provides a way to choose a System.Windows.DataTemplate based on the data object
    /// and the data-bound element.
    /// </summary>
    internal class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DirDiffDocumentViewTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a System.Windows.DataTemplate based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>Returns a <see cref="DataTemplate"/> or null. The default value is null.</returns>
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is DirDiffDocViewModel)
                return DirDiffDocumentViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
