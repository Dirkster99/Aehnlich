namespace Aehnlich
{
	using Settings.UserProfile;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MWindowLib.MetroWindow
									 , IViewSize  // Implements saving and loading/repositioning of Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
	}
}
