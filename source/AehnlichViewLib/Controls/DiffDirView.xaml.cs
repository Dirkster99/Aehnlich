namespace AehnlichViewLib.Controls
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// </summary>
    public partial class DiffDirView : Control
    {
        #region fields
        /// <summary>
        /// Implements the backing store of the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
            typeof(DiffDirView), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store of the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate",
            typeof(DataTemplate), typeof(DiffDirView), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store of the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style),
                typeof(DiffDirView), new PropertyMetadata(null));
        #endregion fields

        #region ctors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static DiffDirView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DiffDirView),
                new FrameworkPropertyMetadata(typeof(DiffDirView)));
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets/sets an <see cref="IEnumerable"/> ItemsSource for the list of objects
        /// that are shown within the list section of the control.
        /// 
        /// Ideally, this should be an ObservableCollection{T} since the control will
        /// look for the <see cref="INotifyCollectionChanged"/> event in order to listen
        /// for this type of event.
        /// </summary>
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets/sets the ItemTemplate for the ListBox portion of the control.
        /// </summary>
        [Bindable(true)]
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the System.Windows.Style that is applied to the container element
        ///  generated for each item.
        /// </summary>
        [Bindable(true)]
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Is called after the template was applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        #endregion methods
    }
}
