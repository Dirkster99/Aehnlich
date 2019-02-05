namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.Windows.Forms;

	#endregion

	#region IDifferenceDlg

	internal interface IDifferenceDialog
	{
		string NameA { get; set; }

		string NameB { get; set; }

		bool OnlyShowIfShiftPressed { get; }

		bool RequiresInput { get; }

		bool ShowShiftCheck { set; }

		DialogResult ShowDialog(IWin32Window owner);
	}

	#endregion

	#region IDifferenceForm

	internal interface IDifferenceForm
	{
		string ToolTipText { get; }

		void ShowDifferences(ShowDiffArgs e);

		void UpdateUI();
	}

	#endregion
}
