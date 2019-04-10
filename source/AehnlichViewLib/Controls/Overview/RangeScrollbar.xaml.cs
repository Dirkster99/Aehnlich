namespace AehnlichViewLib.Controls.Overview
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Markup;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Collections;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using System;

    /// <summary>
    /// Original Source: https://github.com/Ttxman/WpfRangeControls
    /// </summary>
    [TemplatePart(Name = "PART_RangeOverlay", Type = typeof(RangeItemsControl))]
    [TemplatePart(Name = "PART_Track", Type = typeof(Track))]
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
        private Track _PART_Track;
        private ObservableCollection<UIElement> _iItems = new ObservableCollection<UIElement>();
        private bool _ThumbIsDragging;
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
            _PART_Track = base.GetTemplateChild("PART_Track") as Track;

            if (_PART_RangeOverlay == null)
                return;

            if (_PART_Track != null)
            {
                _PART_Track.Thumb.DragEnter += Thumb_DragEnter;
                _PART_Track.Thumb.DragLeave += Thumb_DragLeave;
                this.PreviewMouseDown += _PART_RangeOverlay_PreviewMouseDown;
            }

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

        private void Thumb_DragLeave(object sender, DragEventArgs e)
        {
            _ThumbIsDragging = false;
        }

        private void Thumb_DragEnter(object sender, DragEventArgs e)
        {
            _ThumbIsDragging = true;
        }

        /// <summary>
        /// Handles the mouse click event on the <see cref="Track"/> part of the control
        /// to position the thumb right where the click ocurred.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _PART_RangeOverlay_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed) == false)
                return;

            if (sender != this || _ThumbIsDragging == true)
                return;

            IInputElement inputElement = Mouse.DirectlyOver;
            Rectangle rect = inputElement as Rectangle;

            // Lets keep the thumb draggable by ignoring thumb mouse clicks here
            var thumbMousover = _PART_Track.Thumb.InputHitTest(e.GetPosition(_PART_Track.Thumb));
            if (thumbMousover != null)
                return;

            var sb = sender as ScrollBar;

            // Find the percentage value where the mouse click occured on the track
            double trackHeight = _PART_Track.ActualHeight;
            Point p = e.GetPosition(_PART_Track);
            double factor = p.Y / _PART_Track.ActualHeight;

            // Convert the percentage value into the actual value scale
            double targetValue = Math.Abs(Maximum - Minimum) * factor;
            this.Value = Minimum + targetValue;

            e.Handled = true;
        }
        #endregion methods
    }
}
