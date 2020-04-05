namespace Aehnlich.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using AvalonDock;

    /// <summary>
    ///
    /// </summary>
    [TemplatePartAttribute(Name = PART_DockView, Type = typeof(DockingManager))]
    public partial class AvalonDockView : UserControl
    {
        #region fields
        /// <summary>
        /// defines the nme of the required <see cref="DockingManager"/> control in the attached control XAML
        /// </summary>
        const string PART_DockView = "PART_DockView";
        private DockingManager _PART_DockView;
        #endregion fields

        #region constructors
        static AvalonDockView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AvalonDockView),
                                                     new FrameworkPropertyMetadata(typeof(AvalonDockView)));
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public AvalonDockView()
        {
        }
        #endregion constructors

        #region properties

        #endregion properties

        #region methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += AvalonDockView_Loaded;
        }

        private void AvalonDockView_Loaded(object sender, RoutedEventArgs e)
        {
            _PART_DockView = Template.FindName("PART_DockView", this) as DockingManager;
        }
        #endregion methods
    }
}
