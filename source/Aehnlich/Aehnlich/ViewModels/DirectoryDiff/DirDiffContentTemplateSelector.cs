namespace Aehnlich.Views
{
    using Aehnlich.ViewModels.Documents;
    using System.Windows;
    using System.Windows.Controls;

    internal class DirDiffContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SetupPage { get; set; }

        public DataTemplate ViewPage { get; set; }

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
