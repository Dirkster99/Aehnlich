namespace AehnlichViewModelsLib.Behaviors
{
    using SuggestBoxLib;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Implements a <see cref="SuggestBox"/> control behavior that executes a bound command
    /// based on the NewLocationRequestEvent of the <see cref="SuggestBox"/> control.
    /// </summary>
    public static class NewLocationRequestBehavior
    {
        #region fields
        /// <summary>
        /// Implements the backing store of the Command
        /// attached dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
                typeof(NewLocationRequestBehavior), new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Implements the backing store of the CommandParameter
        /// attached dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object),
                typeof(NewLocationRequestBehavior), new PropertyMetadata(null));
        #endregion fields

        #region methods
        /// <summary>
        /// Gets the attachable command dependency property that is executed when the associated
        /// event has been raised.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the attachable command dependency property that is executed when the associated
        /// event has been raised.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets the (optional) attachable CommandParameter dependency property
        /// that is send with bound comand when the associated event has been raised.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the (optional) attachable CommandParameter dependency property
        /// that is send with bound comand when the associated event has been raised.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as SuggestBox;
            if (element == null)
                return;

            if (e.OldValue == null && e.NewValue != null)
            {
                element.NewLocationRequestEvent += Element_NewLocationRequestEvent;
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                element.NewLocationRequestEvent -= Element_NewLocationRequestEvent;
            }
        }

        private static void Element_NewLocationRequestEvent(object sender, SuggestBoxLib.Events.NextTargetLocationArgs e)
        {
            // Send should be this class or a descendent of it
            var uiElement = sender as SuggestBox;

            // Sanity check just in case this was somehow send by something else
            if (uiElement == null)
                return;

            ICommand commandToExec = NewLocationRequestBehavior.GetCommand(uiElement);

            // There may not be a command bound to this after all
            if (commandToExec == null)
                return;

            // Send the event as parameter if nothing else was specified
            object commandParam = NewLocationRequestBehavior.GetCommandParameter(uiElement);
            if (commandParam == null)
                commandParam = e;

            if (commandToExec is RoutedCommand)
            {
                if (((RoutedCommand)commandToExec).CanExecute(commandParam, uiElement))
                    ((RoutedCommand)commandToExec).Execute(commandParam, uiElement);
            }
            else
            {
                if (commandToExec.CanExecute(commandParam))
                    commandToExec.Execute(commandParam);
            }
        }
        #endregion methods
    }
}
