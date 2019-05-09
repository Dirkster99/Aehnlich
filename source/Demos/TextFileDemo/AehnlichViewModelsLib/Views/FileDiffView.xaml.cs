﻿namespace AehnlichViewModelsLib.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for FileDiffView.xaml
    /// </summary>
    public partial class FileDiffView : UserControl
    {
        #region fields
        /// <summary>
        /// Implements the backing store of the <see cref="ViewLoadedCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewLoadedCommandProperty =
            DependencyProperty.Register("ViewLoadedCommand", typeof(ICommand),
                typeof(FileDiffView), new PropertyMetadata(null));
        #endregion fields

        /// <summary>
        /// Class constructor
        /// </summary>
        public FileDiffView()
        {
            InitializeComponent();

            this.Loaded += DocDiffDocControl_Loaded;
        }

        #region properties
        /// <summary>
        /// Gets/sets a command that can be bound on the viewmodel and is executed when
        /// the view has been fully loaded and is about to be activated for the first time.
        /// 
        /// This command binding is intended for usage on view intialization that takes usually
        /// longer than a few seconds or can even be minutes or longer.
        /// </summary>
        public ICommand ViewLoadedCommand
        {
            get { return (ICommand)GetValue(ViewLoadedCommandProperty); }
            set { SetValue(ViewLoadedCommandProperty, value); }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method is invoked when the view is fully loaded for the first time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocDiffDocControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Invoke this command on bound ViewModel if there is anything to invoke
            if (ViewLoadedCommand != null)
            {
                if (ViewLoadedCommand.CanExecute(null))
                {
                    // Check whether this attached behaviour is bound to a RoutedCommand
                    if (ViewLoadedCommand is RoutedCommand)
                    {
                        // Execute the routed command
                        (ViewLoadedCommand as RoutedCommand).Execute(null, this);
                    }
                    else
                    {
                        // Execute the Command as bound delegate
                        ViewLoadedCommand.Execute(null);
                    }
                }
            }
        }
        #endregion methods
    }
}
