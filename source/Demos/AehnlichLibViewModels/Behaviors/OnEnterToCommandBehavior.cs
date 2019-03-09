namespace AehnlichLibViewModels.Behaviors
{
    using System.Windows;
    using System.Windows.Input;

    public static class OnEnterToCommandBehavior
    {
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object),
                typeof(OnEnterToCommandBehavior), new PropertyMetadata(0));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
                typeof(OnEnterToCommandBehavior), new PropertyMetadata(null, OnCommandChanged));

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
            var element = d as UIElement;
            if (element == null)
                return;

            if (e.OldValue == null && e.NewValue != null)
            {
                element.PreviewKeyDown += Element_PreviewKeyDown;
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                element.PreviewKeyDown -= Element_PreviewKeyDown;
            }
        }

        private static void Element_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Send should be this class or a descendent of it
            var uiElement = sender as UIElement;

            // Sanity check just in case this was somehow send by something else
            if (uiElement == null)
                return;

            if (e.Key == Key.Enter)
            {
                ICommand commandToExec = OnEnterToCommandBehavior.GetCommand(uiElement);
                object commandParam = OnEnterToCommandBehavior.GetCommandParameter(uiElement);

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
        }
    }
}
