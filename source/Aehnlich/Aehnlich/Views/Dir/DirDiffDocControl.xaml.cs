namespace Aehnlich.Views.Dir
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Implements a view that can be used to display directory diff information.
	/// </summary>
	public class DirDiffDocControl : Control
	{
		#region ctors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static DirDiffDocControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DirDiffDocControl),
				new FrameworkPropertyMetadata(typeof(DirDiffDocControl)));
		}
		#endregion ctors
	}
}
