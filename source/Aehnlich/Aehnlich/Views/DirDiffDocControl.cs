namespace Aehnlich.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// </summary>
    public class DirDiffDocControl : Control
    {
        static DirDiffDocControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirDiffDocControl),
                new FrameworkPropertyMetadata(typeof(DirDiffDocControl)));
        }
    }
}
