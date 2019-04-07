namespace WpfRangeControls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Markup;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Collections;
    using System.Windows.Input;

    [TemplatePart(Name = "PART_RangeOverlay", Type = typeof(RangeItemsControl))]
    [ContentProperty("Items")]
    public class RangeScrollbar : ScrollBar, INotifyPropertyChanged
    {
        #region fields
        /// <summary>
        /// Implements the backing store for the AlternationCount dependency property.
        /// </summary>        
        public static readonly DependencyProperty AlternationCountProperty =
            ItemsControl.AlternationCountProperty.AddOwner(typeof(RangeScrollbar));

        public static readonly DependencyProperty ItemsSourceProperty =
            ItemsControl.ItemsSourceProperty.AddOwner(typeof(RangeScrollbar));

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(RangeScrollbar));

        private RangeItemsControl _PART_RangeOverlay;
        private ObservableCollection<UIElement> _iItems = new ObservableCollection<UIElement>();
        #endregion fields        

        #region ctors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static RangeScrollbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeScrollbar),
                                                     new FrameworkPropertyMetadata(typeof(RangeScrollbar)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public RangeScrollbar()
        {
        }
        #endregion ctors

        public event PropertyChangedEventHandler PropertyChanged;

        #region properties
        /// <summary>
        /// Gets the control that hosts an ItemsControl to paint overlay indicators over the scrollbar background.
        /// </summary>
        [Bindable(false), Category("Content")]
        public RangeItemsControl RangeControl
        {
            get { return _PART_RangeOverlay; }
        }

        /// <summary>        
        /// Gets the list of items (if any) that is bound to the RangeItemsControl hosted in this control.
        /// 
        /// Items is also the property name for the property that is the content property of this control.
        /// as defined via [ContentProperty("Items")] at the top of this class definition.
        /// </summary>
        [Bindable(true), Category("Content")]
        public IList Items
        {
            get
            {
                if (_PART_RangeOverlay == null)
                    ApplyTemplate();

                if (_PART_RangeOverlay != null)
                    return _PART_RangeOverlay.Items;

                return _iItems;
            }
        }

        /// <summary>        
        /// Gets/sets the source of items that can be bound to the RangeItemsControl hosted in this control.
        /// </summary>        
        [Bindable(true), Category("Content")]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>        
        /// Gets/sets the item's data template of the items that can be bound
        /// to the RangeItemsControl hosted in this control.
        /// </summary>        
        [Bindable(true), Category("Content")]
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>        
        /// Gets/sets an alternation property that can be used to style each nth entry
        /// according to its alternation position via Trigger or converter.
        ///
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.itemscontrol.alternationcount
        /// </summary>        
        [Bindable(true), Category("Content")]
        public int AlternationCount
        {
            get { return (int)GetValue(AlternationCountProperty); }
            set { SetValue(AlternationCountProperty, value); }
        }
        #endregion properties

        #region methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _PART_RangeOverlay = base.GetTemplateChild("PART_RangeOverlay") as RangeItemsControl;

            if (_PART_RangeOverlay == null)
                return;

            // Are there any items specified for rendering in the contents property of this control?
            if (_iItems != null && _iItems.Count > 0)
            {
                // Copy the items and raise property changed for items collection
                foreach (var item in _iItems)
                    _PART_RangeOverlay.Items.Add(item);

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Items"));

                _iItems = null;
            }
        }
        #endregion methods
    }
}
