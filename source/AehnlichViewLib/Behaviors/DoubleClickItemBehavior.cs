namespace AehnlichViewLib.Behaviors
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;

	/// <summary>
	/// Implements a behavior that attaches itself to <see cref="UIElement"/>
	/// and executes the bound Command with the bound CommandParameter when
	/// the user hits the <see cref="Key.Enter"/>.
	/// </summary>
	public static class DoubleClickItemBehavior
	{
		#region fields
		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.RegisterAttached("CommandParameter", typeof(object),
				typeof(DoubleClickItemBehavior), new PropertyMetadata(null));

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached("Command", typeof(ICommand),
				typeof(DoubleClickItemBehavior), new PropertyMetadata(null, OnCommandChanged));
		#endregion fields

		#region methods
		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		public static object GetCommandParameter(DependencyObject obj)
		{
			return (object)obj.GetValue(CommandParameterProperty);
		}

		public static void SetCommandParameter(DependencyObject obj, object value)
		{
			obj.SetValue(CommandParameterProperty, value);
		}

		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var element = d as Control;
			if (element == null)
				return;

			if (e.OldValue == null && e.NewValue != null)
			{
				element.MouseDoubleClick += Element_MouseDoubleClick;
			}
			else if (e.OldValue != null && e.NewValue == null)
			{
				element.MouseDoubleClick -= Element_MouseDoubleClick;
			}
		}

		private static void Element_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// Send should be this class or a descendent of it
			var uiElement = sender as Control;

			// Sanity check just in case this was somehow send by something else
			if (uiElement == null)
				return;

			ICommand commandToExec = DoubleClickItemBehavior.GetCommand(uiElement);
			object commandParam = DoubleClickItemBehavior.GetCommandParameter(uiElement);

			// There may not be a command bound to this after all
			if (commandToExec == null)
				return;

			// Check whether this attached behaviour is bound to a RoutedCommand
			if (commandToExec is RoutedCommand)
			{
				// Execute the routed command
				if ((commandToExec as RoutedCommand).CanExecute(commandParam, uiElement))
				{
					(commandToExec as RoutedCommand).Execute(commandParam, uiElement);
					e.Handled = true;
				}
			}
			else
			{
				if (commandToExec.CanExecute(commandParam))
				{
					// Execute the Command as bound delegate
					commandToExec.Execute(commandParam);
					e.Handled = true;
				}
			}
		}
		#endregion methods
	}
}
