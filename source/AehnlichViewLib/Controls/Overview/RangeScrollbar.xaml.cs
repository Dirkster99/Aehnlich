namespace AehnlichViewLib.Controls.Overview
{
	using System;
	using System.Collections;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Input;
	using System.Windows.Markup;

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

		/// <summary>
		/// Implements the backing store of the <see cref="IsRepeatButtonVisible"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsRepeatButtonVisibleProperty =
			DependencyProperty.Register("IsRepeatButtonVisible", typeof(bool),
				typeof(RangeScrollbar), new PropertyMetadata(true));



		public double DocumentSize
		{
			get { return (double)GetValue(DocumentSizeProperty); }
			set { SetValue(DocumentSizeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DocumentSize.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DocumentSizeProperty =
			DependencyProperty.Register("DocumentSize", typeof(double),
				typeof(RangeScrollbar), new PropertyMetadata(0d));



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

		/// <summary>
		/// Implements a change notification to inform a bound object if the
		/// <see cref="Items"/> collection is updated with elements from the
		/// content of this control.
		/// </summary>
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

		/// <summary>
		/// Gets/sets a dependency property to determine whether the repeat buttons
		/// are visible or not.
		/// </summary>
		[Bindable(true), Category("Content")]
		public bool IsRepeatButtonVisible
		{
			get { return (bool)GetValue(IsRepeatButtonVisibleProperty); }
			set { SetValue(IsRepeatButtonVisibleProperty, value); }
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Creates the visual tree for the <see cref="ScrollBar"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.Loaded += RangeScrollbar_Loaded;
		}

		private void RangeScrollbar_Loaded(object sender, RoutedEventArgs e)
		{
			_PART_RangeOverlay = base.GetTemplateChild("PART_RangeOverlay") as RangeItemsControl;
			_PART_Track = base.GetTemplateChild("PART_Track") as Track;

			if (_PART_RangeOverlay == null)
				return;

			if (_PART_Track != null)
			{
				_PART_Track.Thumb.DragEnter += Thumb_DragEnter;
				_PART_Track.Thumb.DragLeave += Thumb_DragLeave;
				this.PreviewMouseDown += _PART_RangeOverlay_PreviewMouseDown;

				this.MouseWheel += RangeScrollbar_MouseWheel;
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

		/// <summary>
		/// User finished dragging the thumb control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Thumb_DragLeave(object sender, DragEventArgs e)
		{
			_ThumbIsDragging = false;
		}

		/// <summary>
		/// User started to drag the thumb control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
			// The user should still be able to use right mouse button (context menu)
			if ((e.LeftButton == MouseButtonState.Pressed) == false)
				return;

			// The user should still be able to drag the thumb to update displays in synced fashion
			if (sender != this || _ThumbIsDragging == true)
				return;

			IInputElement inputElement = Mouse.DirectlyOver;

			// Lets keep the thumb draggable by ignoring thumb mouse clicks here
			var thumbMousover = _PART_Track.Thumb.InputHitTest(e.GetPosition(_PART_Track.Thumb));
			if (thumbMousover != null)
				return;

			// Find the percentage value where the mouse click occurred on the track
			Point p = e.GetPosition(_PART_Track);

			// Filter out clicks on repeat button (or other elements below or above Track)
			if (Orientation == Orientation.Vertical)
			{
				if (p.Y > _PART_Track.ActualHeight || p.Y < 0)
					return;

				double factor = p.Y / _PART_Track.ActualHeight;

				// Convert the percentage value into the actual value scale
				double targetValue = Math.Abs(Maximum - Minimum) * factor;
				this.Value = Minimum + targetValue;

				e.Handled = true;
			}
			else
			{
				if (Orientation == Orientation.Horizontal)
				{
					if (p.X > _PART_Track.ActualWidth || p.X < 0)
						return;

					double factor = p.X / _PART_Track.ActualWidth;

					// Convert the percentage value into the actual value scale
					double targetValue = Math.Abs(Maximum - Minimum) * factor;
					this.Value = Minimum + targetValue;

					e.Handled = true;
				}
			}
		}

		/// <summary>
		/// Lets the user scroll the thumb with the mouse wheel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RangeScrollbar_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			double largeChange = Math.Abs(LargeChange);

			if (e.Delta > 0)
			{
				if (Minimum < (Value - largeChange))
					Value -= largeChange;
				else
					Value = Minimum;
			}
			else
			{
				if (e.Delta < 0)
				{
					if (Maximum > (Value + largeChange))
						Value += largeChange;
					else
						Value = Maximum;
				}
			}
		}
		#endregion methods
	}
}
