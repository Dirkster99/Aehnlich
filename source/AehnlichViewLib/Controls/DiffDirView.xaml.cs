namespace AehnlichViewLib.Controls
{
    using AehnlichViewLib.Events;
    using AehnlichViewLib.Interfaces;
    using AehnlichViewLib.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
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
        /// <summary>
        /// Defines the name of the left required grid. This Grid is required to synchronize
        /// the middle splitter object with the controles that are outside of this control.
        /// </summary>
        public const string PART_GridA = "PART_GridA";

        /// <summary>
        /// Defines the name of the right required grid. This Grid is required to synchronize
        /// the middle splitter object with the controles that are outside of this control.
        /// </summary>
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
        #region Background
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
        #endregion Background

        #region Foreground
        /// <summary>
        /// Implements the backing store of the <see cref="ColorForegroundBlank"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorForegroundBlankProperty =
            DependencyProperty.Register("ColorForegroundBlank", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(default(SolidColorBrush), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorForegroundAdded"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorForegroundAddedProperty =
            DependencyProperty.Register("ColorForegroundAdded", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorForegroundDeleted"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorForegroundDeletedProperty =
            DependencyProperty.Register("ColorForegroundDeleted", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00)), OnColorChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ColorForegroundContext"/>
        /// dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorForegroundContextProperty =
            DependencyProperty.Register("ColorForegroundContext", typeof(SolidColorBrush),
                typeof(DiffDirView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00)), OnColorChanged));
        #endregion Foreground

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

        #region SelectedItems
        // Use each source list's hashcode as the key so that we don't hold on
        // to any references in case the DataGrid gets disposed without telling
        // to remove the source list from our registry.
        private readonly Dictionary<int, DataGridsAndInitiatedSelectionChange> selectedItemsSources;

        /// <summary>
        /// Implements the backing store of the <see cref="SelectedItemsSourceA"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsSourceAProperty =
            DependencyProperty.Register("SelectedItemsSourceA",
                typeof(INotifyCollectionChanged),
                typeof(DiffDirView), new PropertyMetadata(null, SelectedItemsSourceAChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="SelectedItemsSourceB"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsSourceBProperty =
            DependencyProperty.Register("SelectedItemsSourceB", typeof(INotifyCollectionChanged),
                typeof(DiffDirView), new PropertyMetadata(null, SelectedItemsSourceBChanged));
        #endregion SelectedItems

        #region ContextMenu
        /// <summary>
        /// Implements the backing store of the <see cref="ContextMenuA"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuAProperty =
            DependencyProperty.Register("ContextMenuA", typeof(ContextMenu),
                typeof(DiffDirView), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store of the <see cref="ContextMenuB"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuBProperty =
            DependencyProperty.Register("ContextMenuB", typeof(ContextMenu),
                typeof(DiffDirView), new PropertyMetadata(null));
        #endregion ContextMenu

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

        /// <summary>
        /// Standard class constructor
        /// </summary>
        public DiffDirView()
        {
            selectedItemsSources = new Dictionary<int, DataGridsAndInitiatedSelectionChange>();
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
        #region Background
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
        #endregion Background

        #region Foreground
        /// <summary>
        /// Gets/sets the Foreground color that is applied when drawing areas that
        /// signifies 2 blank lines in both of the two (text) lines being compared.
        /// 
        /// Normally, there should be no drawing required for this which is why the
        /// default is default(<see cref="SolidColorBrush"/>) - but sometimes it may be useful
        /// to color these lines which is why we have this property here.
        /// </summary>
        public SolidColorBrush ColorForegroundBlank
        {
            get { return (SolidColorBrush)GetValue(ColorForegroundBlankProperty); }
            set { SetValue(ColorForegroundBlankProperty, value); }
        }

        /// <summary>
        /// Gets/sets the Foreground color that is applied when drawing areas that
        /// signifies changed context (2 lines appear to be similar enough to align them
        /// but still mark them as different).
        /// </summary>
        public SolidColorBrush ColorForegroundContext
        {
            get { return (SolidColorBrush)GetValue(ColorForegroundContextProperty); }
            set { SetValue(ColorForegroundContextProperty, value); }
        }

        /// <summary>
        /// Gets/sets the Foreground color that is applied when drawing areas that
        /// signifies an element that is missing in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorForegroundDeleted
        {
            get { return (SolidColorBrush)GetValue(ColorForegroundDeletedProperty); }
            set { SetValue(ColorForegroundDeletedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the Foreground color that is applied when drawing areas that
        /// signifies an element that is added in one of the two (text) lines being compared.
        /// </summary>
        public SolidColorBrush ColorForegroundAdded
        {
            get { return (SolidColorBrush)GetValue(ColorForegroundAddedProperty); }
            set { SetValue(ColorForegroundAddedProperty, value); }
        }
        #endregion Foreground

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

        #region SelectedItems
        /// <summary>
        /// Gets/sets multiple (if any) selected items for the left View A.
        /// </summary>
        public INotifyCollectionChanged SelectedItemsSourceA
        {
            get { return (INotifyCollectionChanged)GetValue(SelectedItemsSourceAProperty); }
            set { SetValue(SelectedItemsSourceAProperty, value); }
        }

        /// <summary>
        /// Gets/sets multiple (if any) selected items for the right View B.
        /// </summary>
        public INotifyCollectionChanged SelectedItemsSourceB
        {
            get { return (INotifyCollectionChanged)GetValue(SelectedItemsSourceBProperty); }
            set { SetValue(SelectedItemsSourceBProperty, value); }
        }
        #endregion SelectedItems

        #region ContextMenu
        /// <summary>
        /// Gets/sets the context menu for the left view A.
        /// </summary>
        public ContextMenu ContextMenuA
        {
            get { return (ContextMenu)GetValue(ContextMenuAProperty); }
            set { SetValue(ContextMenuAProperty, value); }
        }

        /// <summary>
        /// Gets/sets the context menu for the left view B.
        /// </summary>
        public ContextMenu ContextMenuB
        {
            get { return (ContextMenu)GetValue(ContextMenuBProperty); }
            set { SetValue(ContextMenuBProperty, value); }
        }
        #endregion ContextMenu

        #region Column A B GridSplitter Synchronization
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
        #endregion Column A B GridSplitter Synchronization
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
                                                                 ColorBackgroundImaginaryLineDeleted,

                                                                 ColorForegroundBlank,
                                                                 ColorForegroundAdded,
                                                                 ColorForegroundDeleted,
                                                                 ColorForegroundContext);

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

        #region SelectedItems
        private static void SelectedItemsSourceAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var diffView = d as DiffDirView;
            diffView.SelectedItemsSourceAChanged(e, diffView._PART_GridA);
        }

        private static void SelectedItemsSourceBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var diffView = d as DiffDirView;
            diffView.SelectedItemsSourceAChanged(e, diffView._PART_GridB);
        }

        private void SelectedItemsSourceAChanged(DependencyPropertyChangedEventArgs args, DataGrid dataGrid)
        {
            IList selectedItemsSource = null;

            if (dataGrid == null)
                return;

            // Check if the app is setting the source to a new or different list, or if it is removing the binding
            if (args.NewValue != null)
            {
                selectedItemsSource = args.NewValue as IList;
                if (selectedItemsSource == null)
                {
                    throw new ArgumentException("The value for SelectedItemsSource must implement IList.");
                }

                INotifyCollectionChanged collection = args.NewValue as INotifyCollectionChanged;
                if (collection == null)
                {
                    throw new ArgumentException("The value for SelectedItemsSource must implement INotifyCollectionChanged.");
                }

                // Don't add the event handler if the DataGrid is not setting its SelectedItemsSource for the first time
                if (args.OldValue == null)
                {
                    // Sign up for changes to the DataGrid's selected items to enable a two-way binding effect
                    dataGrid.SelectionChanged += UpdateSourceListOnDataGridSelectionChanged;
                }

                // Track this DataGrid instance for the specified source list
                DataGridsAndInitiatedSelectionChange sourceListInfo = null;
                if (this.selectedItemsSources.TryGetValue(selectedItemsSource.GetHashCode(), out sourceListInfo))
                {
                    sourceListInfo.BoundDataGridReferences.Add(new WeakReference(dataGrid));
                }
                else
                {
                    // This is a new source collection
                    sourceListInfo = new DataGridsAndInitiatedSelectionChange() { InitiatedSelectionChange = false };
                    sourceListInfo.BoundDataGridReferences.Add(new WeakReference(dataGrid));
                    this.selectedItemsSources.Add(selectedItemsSource.GetHashCode(), sourceListInfo);

                    // Sign up for changes to the source only on the first time the source is added
                    collection.CollectionChanged += UpdateDataGridsOnSourceCollectionChanged;
                }

                // Now force the DataGrid to update its SelectedItems to match the current
                // contents of the source list
                sourceListInfo.InitiatedSelectionChange = true;
                UpdateDataGrid(dataGrid, selectedItemsSource, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                sourceListInfo.InitiatedSelectionChange = false;
            }
            else
            {
                // This DataGrid is removing its SelectedItems binding to any list
                dataGrid.SelectionChanged -= UpdateSourceListOnDataGridSelectionChanged;
                dataGrid.SelectedItems.Clear();
            }

            if (args.OldValue != null)
            {
                // Clean up the items source that was the old value

                // Remove the DataGrid from the source list's registry and remove the source list
                // if there are no more DataGrids bound to it.
                DataGridsAndInitiatedSelectionChange sourceListInfo = this.selectedItemsSources[args.OldValue.GetHashCode()];
                WeakReference dataGridReferenceNeedingRemoval = null;
                foreach (WeakReference dataGridReference in sourceListInfo.BoundDataGridReferences)
                {
                    if (dataGridReference.IsAlive && (dataGridReference.Target == dataGrid))
                    {
                        dataGridReferenceNeedingRemoval = dataGridReference;
                        break;
                    }
                }
                sourceListInfo.BoundDataGridReferences.Remove(dataGridReferenceNeedingRemoval);
                if (sourceListInfo.BoundDataGridReferences.Count == 0)
                {
                    this.selectedItemsSources.Remove(args.OldValue.GetHashCode());

                    // Detach the event handlers and clear DataGrid.SelectedItems since the source is now null
                    INotifyCollectionChanged collection = args.OldValue as INotifyCollectionChanged;
                    if (collection != null)
                    {
                        collection.CollectionChanged -= UpdateDataGridsOnSourceCollectionChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to update the items in DataGrid.SelectedItems based on the changes defined in the given NotifyCollectionChangedEventArgs
        /// </summary>
        /// <param name="dataGrid">DataGrid which owns the SelectedItems collection to update</param>
        /// <param name="sourceList">IList which is the SelectedItemsSource</param>
        /// <param name="collectionChangedArgs">The NotifyCollectionChangedEventArgs that was passed into the CollectionChanged event handler</param>
        private void UpdateDataGrid(DataGrid dataGrid, IList sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
        {
            switch (collectionChangedArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object newItem in collectionChangedArgs.NewItems)
                    {
                        dataGrid.SelectedItems.Add(newItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object oldItem in collectionChangedArgs.OldItems)
                    {
                        dataGrid.SelectedItems.Remove(oldItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // Unfortunately can not do the following two steps as an atomic change
                    // so the target list could raise multiple notifications as it gets updated
                    dataGrid.SelectedItems.Clear();
                    foreach (object item in sourceList)
                    {
                        dataGrid.SelectedItems.Add(item);
                    }
                    break;
                default:
                    throw new NotImplementedException("Only Add, Remove, and Reset actions are implemented.");
            }
        }

        /// <summary>
        /// INotifyCollectionChanged.CollectionChanged handler for updating DataGrid.SelectedItems when the source list changes
        /// </summary>
        private void UpdateDataGridsOnSourceCollectionChanged(object source, NotifyCollectionChangedEventArgs collectionChangedArgs)
        {
            DataGridsAndInitiatedSelectionChange sourceListInfo = this.selectedItemsSources[source.GetHashCode()];

            // For each DataGrid that is bound to this list, is alive, and did not initate selection changes, update its selection
            sourceListInfo.InitiatedSelectionChange = true;
            IList sourceList = source as IList;
            Debug.Assert(sourceList != null, "SelectedItemsSource must be of type IList");
            DataGrid dataGrid = null;
            foreach (WeakReference dataGridReference in sourceListInfo.BoundDataGridReferences)
            {
                if (dataGridReference.IsAlive && !this.GetInitiatedSelectionChange(dataGridReference.Target as DataGrid))
                {
                    dataGrid = dataGridReference.Target as DataGrid;
                    UpdateDataGrid(dataGrid, sourceList, collectionChangedArgs);
                }
            }
            sourceListInfo.InitiatedSelectionChange = false;
        }

        private bool GetInitiatedSelectionChange(DataGrid dataGrid)
        {
            if (dataGrid == null)
                return false;

            if (dataGrid == _PART_GridA || dataGrid == _PART_GridB)
                return true;

            return false;
        }

        /// <summary>
        /// DataGrid.SelectionChanged handler to update the source list given the SelectionChangedEventArgs
        /// </summary>
        private void UpdateSourceListOnDataGridSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedArgs)
        {
            DataGrid dataGrid = sender as DataGrid;
            IList selectedItemsSource = null;

            if (dataGrid == _PART_GridA)
                selectedItemsSource = this.SelectedItemsSourceA as IList;
            else
            {
                if (dataGrid == _PART_GridB)
                    selectedItemsSource = this.SelectedItemsSourceB as IList;
                else
                {
                    Debug.Fail("Failed to assign sender to a known Grid");
                }
            }

            Debug.Assert(selectedItemsSource != null, "SelectedItemsSource must be of type IList");

            // If the source list initiated the changes then don't pass the DataGrid's changes back down to the source list
            if (!this.selectedItemsSources[selectedItemsSource.GetHashCode()].InitiatedSelectionChange)
            {
                ////DataGridMultipleSelection.SetInitiatedSelectionChange(dataGrid, true);

                foreach (object removedItem in selectionChangedArgs.RemovedItems)
                {
                    selectedItemsSource.Remove(removedItem);
                }

                foreach (object addedItem in selectionChangedArgs.AddedItems)
                {
                    ////Not sure why this is necessary?
                    ////if (IsGenericList(selectedItemsSource.GetType(), addedItem.GetType()))
                    selectedItemsSource.Add(addedItem);
                }

                ////DataGridMultipleSelection.SetInitiatedSelectionChange(dataGrid, false);
            }
        }

////        /// <summary>
////        /// Compare a generic collection and determine whether its item-type matches
////        /// the type of a given item.
////        /// </summary>
////        /// <param name="type"></param>
////        /// <param name="itemType"></param>
////        /// <returns></returns>
////        private bool IsGenericList(Type type, Type itemType)
////        {
////            if (type == null)
////            {
////                throw new ArgumentNullException("type");
////            }
////
////            foreach (Type @interface in type.GetInterfaces())
////            {
////                if (@interface.IsGenericType)
////                {
////                    // Match type used as generic argument with type of item
////                    if (@interface.GenericTypeArguments[0] == itemType)
////                    {
////                        return true;
////                    }
////                }
////            }
////            return false;
////        }
        #endregion SelectedItems
        #endregion methods

        #region private classes
        /// <summary>
        /// Used in the selectedItemsSources private registry to hold state
        /// </summary>
        private class DataGridsAndInitiatedSelectionChange
        {
            public readonly List<WeakReference> BoundDataGridReferences = new List<WeakReference>();

            public bool InitiatedSelectionChange { get; set; }
        }

        #endregion private classes
    }
}
