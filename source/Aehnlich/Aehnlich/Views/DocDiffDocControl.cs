namespace Aehnlich.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///
    /// </summary>
    public class DocDiffDocControl : Control
    {
        static DocDiffDocControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocDiffDocControl),
                new FrameworkPropertyMetadata(typeof(DocDiffDocControl)));
        }
    }
}
