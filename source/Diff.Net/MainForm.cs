namespace Diff.Net
{
	#region Using Directives

	using System;
	using System.ComponentModel;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using System.Windows.Forms;
	using Menees;
	using Menees.Diffs.Controls;
	using Menees.Windows.Forms;

	#endregion

	internal sealed partial class MainForm : ExtendedForm
	{
		#region Private Data Members

		private IDifferenceDialog currentDifferenceDlg;

		#endregion

		#region Constructors

		public MainForm()
		{
			this.InitializeComponent();
			this.Text = ApplicationInfo.ApplicationName;

			// Turn off the 3D border for the MDI client area, so the app looks more modern.
			WindowsUtility.SetMdiClientBorderStyle(this, BorderStyle.None);

			Application.Idle += this.UpdateUIOnIdle;
			Options.OptionsChanged += this.Options_OptionsChanged;
		}

		#endregion

		#region Private Properties

		private bool NewChildShouldBeMaximized
		{
			get
			{
				bool result = true;

				Form child = this.ActiveMdiChild;
				if (child != null)
				{
					result = child.WindowState == FormWindowState.Maximized;
				}

				return result;
			}
		}

		#endregion

		#region Public Methods

		public static void HandleDragEnter(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		public void HandleDragDrop(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				// Post a message back to ourselves to process these files.
				// It isn't safe to pop up a modal during an OLE/shell drop.
				this.PostFiles(files, false);
			}
		}

		public void ShowFileDifferences(string fileNameA, string fileNameB, DialogDisplay display)
		{
			using (FileDiffDialog dialog = new FileDiffDialog())
			{
				dialog.NameA = fileNameA;
				dialog.NameB = fileNameB;

				if (this.GetDialogResult(dialog, display) == DialogResult.OK)
				{
					Options.LastFileA = dialog.NameA;
					Options.LastFileB = dialog.NameB;

					this.ShowDifferences(dialog.NameA, dialog.NameB, DiffType.File);
				}
			}
		}

		public void ShowTextDifferences(string textA, string textB)
		{
			Options.LastTextA = textA;
			Options.LastTextB = textB;

			this.ShowDifferences(textA, textB, DiffType.Text);
		}

		#endregion

		#region Main Entry Point

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			WindowsUtility.InitializeApplication("Diff.Net", null);
			Application.Run(new MainForm());
		}

		#endregion

		#region Private Methods

		private static string BuildRecentItemMenuString(string itemA, string itemB) => itemA + "\n      " + itemB;

		private void FormSave_LoadSettings(object sender, SettingsEventArgs e)
		{
			Options.Load(e.SettingsNode.GetSubNode("Options", true));
			DiffOptions.Load(e.SettingsNode.GetSubNode("DiffOptions", true));

			this.ApplyOptions();

			// Handle command-line options
			string[] names = CommandLineProcessor.Names;
			if (names != null)
			{
				// Post a message to ourselves to process the command-line
				// arguments after the form finishes displaying.
				this.PostFiles(names, true);
			}
		}

		private void FormSave_SaveSettings(object sender, SettingsEventArgs e)
		{
			Options.Save(e.SettingsNode.GetSubNode("Options", true));
			DiffOptions.Save(e.SettingsNode.GetSubNode("DiffOptions", true));
		}

		private DialogResult GetDialogResult(IDifferenceDialog dialog, DialogDisplay display)
		{
			DialogResult result = DialogResult.OK;

			bool showDialog = false;
			if (display == DialogDisplay.Always || dialog.RequiresInput)
			{
				showDialog = true;
			}
			else if (display == DialogDisplay.UseOption)
			{
				showDialog = !dialog.OnlyShowIfShiftPressed || Options.IsShiftPressed;
			}

			if (showDialog)
			{
				dialog.ShowShiftCheck = display != DialogDisplay.Always;

				this.currentDifferenceDlg = dialog;
				try
				{
					result = dialog.ShowDialog(this);
				}
				finally
				{
					this.currentDifferenceDlg = null;
				}
			}

			return result;
		}

		private void HandlePostedFiles(string[] files, bool commandLine)
		{
			int numFiles = files.Length;
			if (numFiles == 1 || numFiles == 2)
			{
				string fileNameA = files[0];
				string fileNameB = string.Empty;
				if (numFiles == 2)
				{
					fileNameB = files[1];
				}

				// See if the first arg is a file.  I'm using the File.Exists
				// method here instead of Options.FileExists because I really
				// always need to know what type of argument was passed in.
				bool fileExists = File.Exists(fileNameA);

				DialogDisplay display = DialogDisplay.UseOption;
				if (commandLine)
				{
					if (fileExists)
					{
						display = CommandLineProcessor.DisplayFileDialog;
					}
					else
					{
						display = CommandLineProcessor.DisplayDirDialog;
					}
				}

				// See if a diff dialog is currently displayed.  If so,
				// then we need to route the args to it instead of popping
				// up a new dialog.
				if (this.currentDifferenceDlg != null)
				{
					if (numFiles == 1)
					{
						if (this.currentDifferenceDlg.NameA.Length == 0)
						{
							this.currentDifferenceDlg.NameA = fileNameA;
						}
						else
						{
							this.currentDifferenceDlg.NameB = fileNameA;
						}
					}
					else
					{
						this.currentDifferenceDlg.NameA = fileNameA;
						this.currentDifferenceDlg.NameB = fileNameB;
					}
				}
				else if (fileExists)
				{
					this.ShowFileDifferences(fileNameA, fileNameB, display);
				}
				else
				{
					this.ShowDirDifferences(fileNameA, fileNameB, display);
				}
			}
		}

		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			this.HandleDragDrop(e);
		}

		private void MainForm_DragEnter(object sender, DragEventArgs e)
		{
			HandleDragEnter(e);
		}

		private void About_Click(object sender, EventArgs e)
		{
			WindowsUtility.ShowAboutBox(this, Assembly.GetExecutingAssembly());
		}

		private void Cascade_Click(object sender, EventArgs e)
		{
			this.LayoutMdi(MdiLayout.Cascade);
		}

		private void CloseAll_Click(object sender, EventArgs e)
		{
			foreach (Form child in this.MdiChildren)
			{
				child.Close();
			}
		}

		private void CompareDirectories_Click(object sender, EventArgs e)
		{
			this.ShowDirDifferences(Options.LastDirA, Options.LastDirB, DialogDisplay.Always);
		}

		private void CompareFiles_Click(object sender, EventArgs e)
		{
			this.ShowFileDifferences(Options.LastFileA, Options.LastFileB, DialogDisplay.Always);
		}

		private void CompareText_Click(object sender, EventArgs e)
		{
			TextDiffForm textDiff = new TextDiffForm();
			if (this.NewChildShouldBeMaximized)
			{
				textDiff.WindowState = FormWindowState.Maximized;
			}

			textDiff.MdiParent = this;
			textDiff.Show();
		}

		private void Exit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Options_Click(object sender, EventArgs e)
		{
			using (OptionsDialog dialog = new OptionsDialog())
			{
				dialog.ShowDialog(this);
			}
		}

		private void TileHorizontally_Click(object sender, EventArgs e)
		{
			this.LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void TileVertically_Click(object sender, EventArgs e)
		{
			this.LayoutMdi(MdiLayout.TileVertical);
		}

		private void PostFiles(string[] files, bool commandLine)
		{
			// Post a message back to ourselves to process these files.
			this.BeginInvoke(new Action<string[], bool>(this.HandlePostedFiles), new object[] { files, commandLine });
		}

		private void RecentDirs_ItemClick(object sender, RecentItemClickEventArgs e)
		{
			this.ShowDirDifferences(e.Values.ElementAt(0), e.Values.ElementAt(1), DialogDisplay.UseOption);
		}

		private void RecentFiles_ItemClick(object sender, RecentItemClickEventArgs e)
		{
			this.ShowFileDifferences(e.Values.ElementAt(0), e.Values.ElementAt(1), DialogDisplay.UseOption);
		}

		private void ReportError(string message)
		{
			Cursor.Current = Cursors.Default;
			WindowsUtility.ShowError(this, message);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Top-level method for showing differences.")]
		private void ShowDifferences(string itemA, string itemB, DiffType diffType)
		{
			using (WaitCursor wc = new WaitCursor(this))
			{
				try
				{
					Form frmNew;
					if (diffType == DiffType.Directory)
					{
						frmNew = new DirDiffForm();
						this.RecentDirs.Add(BuildRecentItemMenuString(itemA, itemB), new string[] { itemA, itemB });
					}
					else
					{
						// Use a FileDiffForm for file or text diffs.
						frmNew = new FileDiffForm();

						if (diffType == DiffType.File)
						{
							this.RecentFiles.Add(BuildRecentItemMenuString(itemA, itemB), new string[] { itemA, itemB });
						}
					}

					if (this.NewChildShouldBeMaximized)
					{
						frmNew.WindowState = FormWindowState.Maximized;
					}

					frmNew.MdiParent = this;

					IDifferenceForm frmDiff = (IDifferenceForm)frmNew;
					frmDiff.ShowDifferences(new ShowDiffArgs(itemA, itemB, diffType));

					MdiTab tab = this.mdiTabStrip.FindTab(frmNew);
					if (tab != null)
					{
						tab.ToolTipText = frmDiff.ToolTipText;
					}
				}
				catch (Exception ex)
				{
					this.ReportError(ex.Message);
				}
			}
		}

		private void ShowDirDifferences(string directoryA, string directoryB, DialogDisplay display)
		{
			using (DirDiffDialog dialog = new DirDiffDialog())
			{
				dialog.NameA = directoryA;
				dialog.NameB = directoryB;

				if (this.GetDialogResult(dialog, display) == DialogResult.OK)
				{
					Options.LastDirA = dialog.NameA;
					Options.LastDirB = dialog.NameB;

					this.ShowDifferences(dialog.NameA, dialog.NameB, DiffType.Directory);
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Application.Idle handlers must catch all exceptions.")]
		private void UpdateUIOnIdle(object sender, EventArgs e)
		{
			try
			{
				Form child = this.ActiveMdiChild;
				IDifferenceForm frmChild = child as IDifferenceForm;
				if (frmChild != null)
				{
					frmChild.UpdateUI();
				}

				// There can be MDI children that are not IDifferenceForms (e.g., TextDiffForm).
				bool hasMdiChildren = child != null;
				this.mnuTileVertically.Enabled = hasMdiChildren;
				this.mnuTileHorizontally.Enabled = hasMdiChildren;
				this.mnuCascade.Enabled = hasMdiChildren;
				this.mnuCloseAll.Enabled = hasMdiChildren;
			}
			catch (Exception ex)
			{
				// We must explicitly call this because Application.Idle
				// doesn't run inside the normal ThreadException protection
				// that the Application provides for the main message pump.
				Application.OnThreadException(ex);
			}
		}

		private void Options_OptionsChanged(object sender, EventArgs e)
		{
			this.ApplyOptions();
		}

		private void ApplyOptions()
		{
			// The MdiTabStrip manages its own visiblity by hiding when no tabs exist.  However,
			// it respects the control's Enabled property when deciding whether to add or remove
			// tabs and toggle the visibility.
			this.mdiTabStrip.Enabled = Options.ShowMdiTabs;

			if (!Options.ShowMdiTabs)
			{
				// The strip is disabled, so we have to force visibility off since the strip
				// won't do any checking of tab existence now.
				this.mdiTabStrip.Visible = false;
			}
			else
			{
				// The strip is enabled, so we need to let the strip decide whether
				// it should be visible based on whether any tabs exist.
				this.mdiTabStrip.BeginUpdate();
				this.mdiTabStrip.EndUpdate();

				// Make sure all the tab tooltips are up-to-date since windows may have
				// been opened while the tab strip was disabled.
				foreach (Form child in this.MdiChildren)
				{
					// Not all MDI children implement IDifferenceForm (e.g., TextDiffForm doesn't).
					IDifferenceForm form = child as IDifferenceForm;
					if (form != null)
					{
						MdiTab tab = this.mdiTabStrip.FindTab(child);
						if (tab != null && !string.IsNullOrEmpty(tab.ToolTipText))
						{
							tab.ToolTipText = form.ToolTipText;
						}
					}
				}
			}
		}

		private void CloseTab_Click(object sender, EventArgs e)
		{
			this.mdiTabStrip.CloseActiveTab();
		}

		private void CloseAllButThisTab_Click(object sender, EventArgs e)
		{
			this.mdiTabStrip.CloseAllButActiveTab();
		}

		private void MdiTabContext_Opening(object sender, CancelEventArgs e)
		{
			MdiTab mouseOverTab = this.mdiTabStrip.FindTabAtScreenPoint(Cursor.Position);
			e.Cancel = mouseOverTab == null;
		}

		#endregion
	}
}
