namespace AehnlichViewLib.Controls
{
    using AehnlichViewLib.Events;
    using AehnlichViewLib.Interfaces;
    using AehnlichViewLib.Models;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Control implements a directory diff view with a side by side list of directories and files
    /// and their properties (size last update etc).
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

        #region Diff Color Definitions
        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundBlank"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundBlankProperty =
            DependencyProperty.Register("ColorBackgroundBlank", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(default(SolidColorBrush), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundAdded"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundAddedProperty =
            DependencyProperty.Register("ColorBackgroundAdded", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xba, 0xff)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundDeleted"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundDeletedProperty =
            DependencyProperty.Register("ColorBackgroundDeleted", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x80, 0x80)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundContext"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundContextProperty =
            DependencyProperty.Register("ColorBackgroundContext", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0xFF, 0x80)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundImaginaryLineAdded"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundImaginaryLineAddedProperty =
            DependencyProperty.Register("ColorBackgroundImaginaryLineAdded", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x60, 0x00, 0xba, 0xff)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorBackgroundImaginaryLineDeleted"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorBackgroundImaginaryLineDeletedProperty =
            DependencyProperty.Register("ColorBackgroundImaginaryLineDeleted", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x60, 0xFF, 0x80, 0x80)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="DiffColorDefinitions"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty DiffColorDefinitionsProperty =
            DependencyProperty.Register("DiffColorDefinitions", typeof(IDiffColorDefinitions),
                typeof(DiffDirView), new PropertyMetadata(null));
        #endregion Diff Color Definitions

        #region ViewActivation
        /// <summary>
        /// Implements the backing store of the <see cref="ActivationTimeStamp_A"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActivationTimeStamp_AProperty =
            DependencyProperty.Register("ActivationTimeStamp_A", typeof(DateTime),
                typeof(DiffDirView), new PropertyMetadata(default(DateTime)));

        /// <summary>
        /// Implements the backing store of the <see cref="ActivationTimeStamp_B"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActivationTimeStamp_BProperty =
            DependencyProperty.Register("ActivationTimeStamp_B", typeof(DateTime),
                typeof(DiffDirView), new PropertyMetadata(default(DateTime)));
        #endregion ViewActivation

        #region SelectedItem
        /// <summary>
        /// Implements the backing store of the <see cref="SelectedItem_A"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItem_AProperty =
            DependencyProperty.Register("SelectedItem_A", typeof(object),
                typeof(DiffDirView), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store of the <see cref="SelectedItem_B"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItem_BProperty =
            DependencyProperty.Register("SelectedItem_B", typeof(object),
                typeof(DiffDirView), new PropertyMetadata(null));
        #endregion SelectedItem

        #region Column A B GridSplitter Synchronization
        /// <summary>
        /// Implements the backing store of the <see cref="ColumnWidthA"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnWidthAProperty =
            DependencyProperty.Register("ColumnWidthA", typeof(GridLength),
                typeof(DiffDirView), new PropertyMetadata(default(GridLength), OnColumnWidthAChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColumnWidthB"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnWidthBProperty =
            DependencyProperty.Register("ColumnWidthB", typeof(GridLength),
                typeof(DiffDirView), new PropertyMetadata(default(GridLength), OnColumnWidthBChanged));
        #endregion Column A B GridSplitter Synchronization

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

        /// <summary>
        /// Implements an event that is invoked when the column view with columns A and B
        /// (PART_GridA and PART_GridB separated by a GridSplitter) is being
        /// resized in the way that the visible proportional width has been changed.
        /// </summary>
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

        #region Diff Color definitions
        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies changed context (2 lines appear to be similar enough to align them
        /// but still mark them as different).
        /// </summary>
        public SolidColorBrush ColorBackgroundContext
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundContextProperty); }
            set { SetValue(ColorBackgroundContextProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundDeleted
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundDeletedProperty); }
            set { SetValue(ColorBackgroundDeletedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is added in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundAdded
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundAddedProperty); }
            set { SetValue(ColorBackgroundAddedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies 2 blank lines in both of the two (text) lines being compared.
        /// 
        /// Normally, there should be no drawing required for this which is why the
        /// default is default(<see cref="SolidColorBrush"/>) - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        public SolidColorBrush ColorBackgroundBlank
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundBlankProperty); }
            set { SetValue(ColorBackgroundBlankProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundImaginaryLineDeleted
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundImaginaryLineDeletedProperty); }
            set { SetValue(ColorBackgroundImaginaryLineDeletedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the background color that is applied when drawing areas that
        /// signifies an element that is added as an imaginary line in one of the two
		/// (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorBackgroundImaginaryLineAdded
        {
            get { return (SolidColorBrush)GetValue(ColorBackgroundImaginaryLineAddedProperty); }
            set { SetValue(ColorBackgroundImaginaryLineAddedProperty, value); }
        }

        /// <summary>
        /// Gets a set of color definitions that can be bound to in one place to process all
        /// color definition based on one object binding rather than 5-6 additional bindings.
        /// </summary>
        public IDiffColorDefinitions DiffColorDefinitions
        {
            get { return (IDiffColorDefinitions)GetValue(DiffColorDefinitionsProperty); }
            set { SetValue(DiffColorDefinitionsProperty, value); }
        }
        #endregion Diff Color Definitions

        #region ViewActivation
        /// <summary>
        /// Gets the timestamp for the last time when the left view A was active (had received focus).
        /// </summary>
        public DateTime ActivationTimeStamp_A
        {
            get { return (DateTime)GetValue(ActivationTimeStamp_AProperty); }
            set { SetValue(ActivationTimeStamp_AProperty, value); }
        }

        /// <summary>
        /// Gets the timestamp for the last time when the right view B was active (had received focus).
        /// </summary>
        public DateTime ActivationTimeStamp_B
        {
            get { return (DateTime)GetValue(ActivationTimeStamp_BProperty); }
            set { SetValue(ActivationTimeStamp_BProperty, value); }
        }
        #endregion ViewActivation

        #region Column A B Synchronization
        /// <summary>
        /// Gets the width of column A in a view with columns A and B being separated by a GridSplitter.
        /// </summary>
        public GridLength ColumnWidthA
        {
            get { return (GridLength)GetValue(ColumnWidthAProperty); }
            set { SetValue(ColumnWidthAProperty, value); }
        }

        /// <summary>
        /// Gets the width of column B in a view with columns A and B being separated by a GridSplitter.
        /// </summary>
        public GridLength ColumnWidthB
        {
            get { return (GridLength)GetValue(ColumnWidthBProperty); }
            set { SetValue(ColumnWidthBProperty, value); }
        }
        #endregion Column A B Synchronization

        #region SelectedItem
        /// <summary>
        /// Gets the selecteditem (if any) from the left PART_GridA.
        /// </summary>
        public object SelectedItem_A
        {
            get { return (object)GetValue(SelectedItem_AProperty); }
            set { SetValue(SelectedItem_AProperty, value); }
        }

        /// <summary>
        /// Gets the selecteditem (if any) from the right PART_GridB.
        /// </summary>
        public object SelectedItem_B
        {
            get { return (object)GetValue(SelectedItem_BProperty); }
            set { SetValue(SelectedItem_BProperty, value); }
        }
        #endregion SelectedItem
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

                // Attach more event handlers
                _PART_GridA.GotFocus += DiffView_GotFocus;
                _PART_GridB.GotFocus += DiffView_GotFocus;
            }

            if (_PART_GridSplitter != null)
            {
                _PART_GridSplitter.DragCompleted += _PART_GridSplitter_DragCompleted;
                _PART_GridSplitter.DragDelta += _PART_GridSplitter_DragDelta;
            }

            // Initialize background color defintions for line background highlighting
            OnColorChanged(null);
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

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DiffDirView)d).OnColorChanged(e.NewValue);
        }
        #endregion private statics

        private void Grid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_leftGridScrollViewer == null && _rightGridScrollViewer == null)
                return;

            ScrollViewer scrollToSync = null;

            // Determine the scroller that has send the event so we know the scroller to sync
            if (sender == _rightGridScrollViewer)
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

        private void OnColorChanged(object newValue)
        {
            this.DiffColorDefinitions = new DiffColorDefinitions(ColorBackgroundBlank,
                                                                ColorBackgroundAdded,
                                                                ColorBackgroundDeleted,
                                                                ColorBackgroundContext,
                                                                ColorBackgroundImaginaryLineAdded,
                                                                ColorBackgroundImaginaryLineDeleted);

            this.InvalidateVisual();
        }

        /// <summary>
        /// Method is invoked to record each time the left or right view has been focused.
        /// To record its last time of activation in <see cref="ActivationTimeStamp_A"/>
		/// or <see cref="ActivationTimeStamp_B"/> property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffView_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender == _PART_GridA)
                ActivationTimeStamp_A = DateTime.Now;
            else
            {
                if (sender == _PART_GridB)
                    ActivationTimeStamp_B = DateTime.Now;
            }
        }
        #endregion methods
    }
}
