namespace AehnlichViewLib.Controls
{
    using AehnlichViewLib.Events;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// </summary>
    [TemplatePart(Name = PART_GridA, Type = typeof(DataGrid))]
    [TemplatePart(Name = PART_GridB, Type = typeof(DataGrid))]
    public partial class DiffDirView : Control
    {
        #region fields
        public const string PART_GridA = "PART_GridA";
        public const string PART_GridB = "PART_GridB";

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

        #region Column A B Synchronization
        public static readonly DependencyProperty ColumnWidthAProperty =
            DependencyProperty.Register("ColumnWidthA", typeof(GridLength),
                typeof(DiffDirView), new PropertyMetadata(default(GridLength), OnColumnWidthAChanged));

        public static readonly DependencyProperty ColumnWidthBProperty =
            DependencyProperty.Register("ColumnWidthB", typeof(GridLength),
                typeof(DiffDirView), new PropertyMetadata(default(GridLength), OnColumnWidthBChanged));
        #endregion Column A B Synchronization

        private ColumnDefinition _PART_ColumnA, _PART_ColumnB;
        private GridSplitter _PART_GridSplitter;
        private DataGrid _PART_GridA, _PART_GridB;
        private ScrollViewer _leftGridScrollViewer, _rightGridScrollViewer;
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

        public event EventHandler<ColumnWidthChangedEvent> ColumnWidthChanged;

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
        /// generated for each item.
        /// </summary>
        [Bindable(true)]
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public GridLength ColumnWidthA
        {
            get { return (GridLength)GetValue(ColumnWidthAProperty); }
            set { SetValue(ColumnWidthAProperty, value); }
        }

        public GridLength ColumnWidthB
        {
            get { return (GridLength)GetValue(ColumnWidthBProperty); }
            set { SetValue(ColumnWidthBProperty, value); }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Is called after the template was applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.Loaded += new RoutedEventHandler(this.OnLoaded);
        }

        /// <summary>
        /// Wire up synchronisation event handlers when control was loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _PART_ColumnA = GetTemplateChild("PART_ColumnA") as ColumnDefinition;
            _PART_ColumnB = GetTemplateChild("PART_ColumnB") as ColumnDefinition;

            _PART_GridSplitter = GetTemplateChild("PART_GridSplitter") as GridSplitter;

            _PART_GridA = GetTemplateChild(PART_GridA) as DataGrid;
            _PART_GridB = GetTemplateChild(PART_GridB) as DataGrid;

            _leftGridScrollViewer = GetScrollViewer(_PART_GridA);
            _rightGridScrollViewer = GetScrollViewer(_PART_GridB);

            if (_PART_GridA != null && _PART_GridB != null &&
                _leftGridScrollViewer != null && _rightGridScrollViewer != null)
            {
                _leftGridScrollViewer.ScrollChanged += Grid_ScrollChanged;
                _rightGridScrollViewer.ScrollChanged += Grid_ScrollChanged;

                _PART_GridA.SelectionChanged += Grid_SelectionChanged;
                _PART_GridB.SelectionChanged += Grid_SelectionChanged;
            }

            if (_PART_GridSplitter != null)
            {
                _PART_GridSplitter.DragCompleted += _PART_GridSplitter_DragCompleted;
                _PART_GridSplitter.DragDelta += _PART_GridSplitter_DragDelta;
            }
        }

        #region private statics
        private static ScrollViewer GetScrollViewer(UIElement element)
        {
            if (element == null)
                return null;

            ScrollViewer retour = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && retour == null; i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is ScrollViewer)
                {
                    retour = (ScrollViewer)(VisualTreeHelper.GetChild(element, i));
                }
                else
                {
                    retour = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
                }
            }

            return retour;
        }

        private static void OnColumnWidthAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DiffDirView).OnColumnWidthAChanged(e);
        }

        private static void OnColumnWidthBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DiffDirView).OnColumnWidthBChanged(e);
        }
        #endregion private statics

        private void Grid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_leftGridScrollViewer == null && _rightGridScrollViewer == null)
                return;

            ScrollViewer scrollToSync = null;

            // Determine the scroller that has send the event so we know the scroller to sync
            if (sender == _PART_GridB)
                scrollToSync = _leftGridScrollViewer;
            else
                scrollToSync = _rightGridScrollViewer;

            var src_scrollToSync = sender as ScrollViewer;  // Sync scrollviewers on both side of DiffControl

            scrollToSync.ScrollToVerticalOffset(e.VerticalOffset);
            scrollToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gridDestinationToSync = null;
            DataGrid gridSourceToSync = null;

            if (sender == _PART_GridA)
            {
                gridDestinationToSync = _PART_GridB;
                gridSourceToSync = _PART_GridA;
            }
            else
            {
                if (sender == _PART_GridB)
                {
                    gridDestinationToSync = _PART_GridA;
                    gridSourceToSync = _PART_GridB;
                }
            }

            // Not sure who's been sending this so we can't process it
            if (gridDestinationToSync == null || gridSourceToSync == null)
                return;

            if (gridDestinationToSync.SelectedIndex != gridSourceToSync.SelectedIndex)
            {
                gridDestinationToSync.SelectedIndex = gridSourceToSync.SelectedIndex;
            }
        }

        private void OnColumnWidthAChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_PART_ColumnA != null)
                _PART_ColumnA.Width = (GridLength)e.NewValue;
        }

        private void OnColumnWidthBChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_PART_ColumnB != null)
                _PART_ColumnB.Width = (GridLength)e.NewValue;
        }

        private void _PART_GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (_PART_ColumnA != null && _PART_ColumnB != null)
            {
                this.ColumnWidthChanged?.Invoke(this, new ColumnWidthChangedEvent(_PART_ColumnA.Width,
                                                                                  _PART_ColumnB.Width));
            }
        }

        private void _PART_GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (_PART_ColumnA != null && _PART_ColumnB != null)
            {
                this.ColumnWidthChanged?.Invoke(this, new ColumnWidthChangedEvent(_PART_ColumnA.Width,
                                                                                  _PART_ColumnB.Width));
            }
        }

        #endregion methods
    }
}
