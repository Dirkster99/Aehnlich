namespace AehnlichViewLib.Controls
{
	using AehnlichViewLib.Enums;
	using AehnlichViewLib.Events;
	using AehnlichViewLib.Interfaces;
	using AehnlichViewLib.Models;
	using ICSharpCode.AvalonEdit;
	using System;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Media;

	/// <summary>
	/// Implements a Text Diff control with synchronized left and rights view.
	/// </summary>
	[TemplatePart(Name = PART_LeftDiffView, Type = typeof(DiffView))]
	[TemplatePart(Name = PART_RightDiffView, Type = typeof(DiffView))]
	[TemplatePart(Name = PART_ColumnA, Type = typeof(ColumnDefinition))]
	[TemplatePart(Name = PART_ColumnB, Type = typeof(ColumnDefinition))]
	[TemplatePart(Name = PART_GridSplitter, Type = typeof(GridSplitter))]
	public class DiffTextView : Control, IGridSplitterSupport
	{
		#region fields
		public const string PART_RightDiffView = "PART_TextRight";
		public const string PART_LeftDiffView = "PART_TextLeft";

		#region Column IGridSplitterSupport
		/// <summary>
		/// Defines the name of the right required column. The size of this column can be
		/// synchronized with content in the control's client application via
		/// <see cref="IGridSplitterSupport"/>.
		/// </summary>
		public const string PART_ColumnA = "PART_ColumnA";

		/// <summary>
		/// Defines the name of the left required column. The size of this column can be
		/// synchronized with content in the control's client application via
		/// <see cref="IGridSplitterSupport"/>.
		/// </summary>
		public const string PART_ColumnB = "PART_ColumnB";

		/// <summary>
		/// Defines the name of the required middle gridsplitter.
		/// The size of the left and right column of the grid splitter can be
		/// synchronized with content in the control's client application via
		/// <see cref="IGridSplitterSupport"/>.
		/// </summary>
		public const string PART_GridSplitter = "PART_GridSplitter";
		#endregion Column IGridSplitterSupport

		/// <summary>
		/// Implements the backing store of the <see cref="LeftDiffView"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LeftDiffViewProperty =
			DependencyProperty.Register("LeftDiffView", typeof(IDiffView),
				typeof(DiffTextView), new PropertyMetadata(null));

		/// <summary>
		/// Implements the backing store of the <see cref="RightDiffView"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty RightDiffViewProperty =
			DependencyProperty.Register("RightDiffView", typeof(IDiffView),
				typeof(DiffTextView), new PropertyMetadata(null));

		/// <summary>
		/// Implements the backing store of the <see cref="SetFocus"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty SetFocusProperty =
			DependencyProperty.Register("SetFocus", typeof(Focus),
				typeof(DiffTextView), new PropertyMetadata(Enums.Focus.LeftFilePath, OnSetFocusChanged));

		/// <summary>
		/// Implements the backing store of the <see cref="LeftFileName"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LeftFileNameProperty =
			DependencyProperty.Register("LeftFileName", typeof(string),
				typeof(DiffTextView), new PropertyMetadata(null));

		/// <summary>
		/// Implements the backing store of the <see cref="RightFileName"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty RightFileNameProperty =
			DependencyProperty.Register("RightFileName", typeof(string),
				typeof(DiffTextView), new PropertyMetadata(null));

		/// <summary>
		/// Implements the backing store of the <see cref="ViewPortChangedCommand"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ViewPortChangedCommandProperty =
			DependencyProperty.Register("ViewPortChangedCommand", typeof(ICommand),
				typeof(DiffTextView), new PropertyMetadata(null));

		public static readonly DependencyProperty DiffViewOptionsProperty =
			DependencyProperty.Register("DiffViewOptions", typeof(TextEditorOptions),
				typeof(DiffTextView), new PropertyMetadata(new TextEditorOptions { IndentationSize = 4, ShowTabs = false, ConvertTabsToSpaces = true }));

		#region GridColumn Sync
		/// <summary>
		/// Implements the backing store of the <see cref="ColumnWidthA"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ColumnWidthAProperty =
			DependencyProperty.Register("ColumnWidthA", typeof(GridLength),
				typeof(DiffTextView),
				// Note: Note sure why the difference to ColumnWidthB property is necessary
				//       but the GridSplitter is initially frozen (cannot be dragged in themed App) if both values are 1.0
				new PropertyMetadata(new GridLength(1.0 - 0.00000000001,
					GridUnitType.Star), OnColumnWidthAChanged));

		/// <summary>
		/// Implements the backing store of the <see cref="ColumnWidthB"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ColumnWidthBProperty =
			DependencyProperty.Register("ColumnWidthB", typeof(GridLength),
				typeof(DiffTextView), new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), OnColumnWidthBChanged));
		#endregion GridColumn Sync

		private DiffView _PART_LeftDiffView, _PART_RightDiffView;
		private ScrollViewer _leftScrollViewer, _rightScrollViewer;
		private GridSplitter _PART_GridSplitter;
		private ColumnDefinition _PART_ColumnA, _PART_ColumnB;
		#endregion fields

		#region ctors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static DiffTextView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DiffTextView),
				new FrameworkPropertyMetadata(typeof(DiffTextView)));
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
		/// Gets/sets an interface definition that should be implemented by the viewmodel to configure
		/// and drive all available aspects of the left diff view A.
		/// </summary>
		public IDiffView LeftDiffView
		{
			get { return (IDiffView)GetValue(LeftDiffViewProperty); }
			set { SetValue(LeftDiffViewProperty, value); }
		}

		/// <summary>
		/// Gets/sets an interface definition that should be implemented by the viewmodel to configure
		/// and drive all available aspects of the right diff view B.
		/// </summary>
		public IDiffView RightDiffView
		{
			get { return (IDiffView)GetValue(RightDiffViewProperty); }
			set { SetValue(RightDiffViewProperty, value); }
		}

		public TextEditorOptions DiffViewOptions
		{
			get { return (TextEditorOptions)GetValue(DiffViewOptionsProperty); }
			set { SetValue(DiffViewOptionsProperty, value); }
		}

		public string LeftFileName
		{
			get { return (string)GetValue(LeftFileNameProperty); }
			set { SetValue(LeftFileNameProperty, value); }
		}

		public string RightFileName
		{
			get { return (string)GetValue(RightFileNameProperty); }
			set { SetValue(RightFileNameProperty, value); }
		}

		/// <summary>
		/// Implements a bindable command that can be invoked when the current view
		/// in the left or right view changes (window changes size resulting in less
		/// or more text lines being available in actual displayed view).
		/// </summary>
		public ICommand ViewPortChangedCommand
		{
			get { return (ICommand)GetValue(ViewPortChangedCommandProperty); }
			set { SetValue(ViewPortChangedCommandProperty, value); }
		}

		public Focus SetFocus
		{
			get { return (Focus)GetValue(SetFocusProperty); }
			set { SetValue(SetFocusProperty, value); }
		}

		#region GridColumn Sync
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
		#endregion
		#endregion GridColumn Sync

		#region methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_PART_ColumnA = GetTemplateChild(PART_ColumnA) as ColumnDefinition;
			_PART_ColumnB = GetTemplateChild(PART_ColumnB) as ColumnDefinition;

			_PART_GridSplitter = GetTemplateChild(PART_GridSplitter) as GridSplitter;

			this.Loaded += new RoutedEventHandler(this.OnLoaded);
		}

		#region private static
		private static void OnColumnWidthAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as DiffTextView).OnColumnWidthAChanged(e);
		}

		private static void OnColumnWidthBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as DiffTextView).OnColumnWidthBChanged(e);
		}
		#endregion private static

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_PART_LeftDiffView = GetTemplateChild(PART_LeftDiffView) as DiffView;
			_PART_RightDiffView = GetTemplateChild(PART_RightDiffView) as DiffView;

			_leftScrollViewer = GetScrollViewer(_PART_LeftDiffView);
			_rightScrollViewer = GetScrollViewer(_PART_RightDiffView);

			if (_leftScrollViewer != null)
				_leftScrollViewer.ScrollChanged += Scrollviewer_ScrollChanged;

			if (_rightScrollViewer != null)
				_rightScrollViewer.ScrollChanged += Scrollviewer_ScrollChanged;

			if (_PART_GridSplitter != null)
			{
				_PART_GridSplitter.DragCompleted += _PART_GridSplitter_DragCompleted;
				_PART_GridSplitter.DragDelta += _PART_GridSplitter_DragDelta;
			}
		}

		/// <summary>
		/// Is invoked when 1 of the scroll viewer view is changing to tell the
		/// viemlodel about it and give it a chance to update its background highlighting
		/// definition items.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Scrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			ScrollViewer scrollToSync = null;
			DiffView sourceToSync = null;

			// Determine the scroller that has send the event so we know the scroller to sync
			if (sender == _rightScrollViewer)
			{
				scrollToSync = _leftScrollViewer;
				sourceToSync = _PART_LeftDiffView;
			}
			else
			{
				scrollToSync = _rightScrollViewer;
				sourceToSync = _PART_RightDiffView;
			}

			scrollToSync.ScrollToVerticalOffset(e.VerticalOffset);
			scrollToSync.ScrollToHorizontalOffset(e.HorizontalOffset);

			// Get current view lines viewport in source of event
			if (sourceToSync.TextArea.TextView.VisualLines.Any())
			{
				var firstline = sourceToSync.TextArea.TextView.VisualLines.First();
				var lastline = sourceToSync.TextArea.TextView.VisualLines.Last();

				int fline = firstline.FirstDocumentLine.LineNumber;
				int lline = lastline.LastDocumentLine.LineNumber;

				int caretLine = sourceToSync.TextArea.Caret.Line;
				int caretCol = sourceToSync.TextArea.Caret.Column;

				if (ViewPortChangedCommand != null)
				{
					var currentViewPort = new DiffViewPort(fline, lline, sourceToSync.LineCount, caretLine, caretCol);

					if (ViewPortChangedCommand.CanExecute(currentViewPort))
					{
						// Check whether this attached behaviour is bound to a RoutedCommand
						if (ViewPortChangedCommand is RoutedCommand)
						{
							// Execute the routed command
							(ViewPortChangedCommand as RoutedCommand).Execute(currentViewPort, this);
						}
						else
						{
							// Execute the Command as bound delegate
							ViewPortChangedCommand.Execute(currentViewPort);
						}
					}
				}
			}

			e.Handled = true;
		}

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

		private void OnColumnWidthAChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_PART_ColumnA != null)
			{
				var newValue = (GridLength)e.NewValue;

				if (Math.Abs(newValue.Value - _PART_ColumnA.Width.Value) > 1)
				{
					_PART_ColumnA.Width = (GridLength)e.NewValue;
				}
			}
		}

		private void OnColumnWidthBChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_PART_ColumnB != null)
			{
				var newValue = (GridLength)e.NewValue;

				if (Math.Abs(newValue.Value - _PART_ColumnB.Width.Value) > 1)
				{
					_PART_ColumnB.Width = (GridLength)e.NewValue;
				}
			}
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

		#region SetFocus Dependency Property
		private static void OnSetFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as DiffTextView).OnSetFocusChanged(e);
		}

		private void OnSetFocusChanged(DependencyPropertyChangedEventArgs e)
		{
			if ((e.NewValue is Enums.Focus) == false)
				return;

			UIElement elementToFocus = null;

			switch ((Enums.Focus)e.NewValue)
			{
				case Enums.Focus.LeftView:
					elementToFocus = _PART_LeftDiffView;
					break;

				case Enums.Focus.RightView:
					elementToFocus = _PART_RightDiffView;
					break;

				case Enums.Focus.None:
					return;

				default:
					throw new ArgumentException(((Enums.Focus)e.NewValue).ToString());
			}

			if (elementToFocus != null)
			{
				elementToFocus.Focus();
				Keyboard.Focus(elementToFocus);
			}
		}
		#endregion SetFocus Dependency Property
		#endregion methods
	}
}
