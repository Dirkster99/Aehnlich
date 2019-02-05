namespace Diff.Net
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Windows.Forms;
    using DiffLib.Dir;
    using DiffLib.Enums;
    using Menees;

	internal static class Options
	{
		#region Private Data Members

		private static IList<string> customFilters = new List<string>();
		private static bool changed;
		private static bool checkDirExists = true;
		private static bool checkFileExists = true;
		private static bool showWSInLineDiff = true;
		private static bool showWSInMainDiff;
		private static bool showMdiTabs = true;
		private static HashType hashType = HashType.HashCode;
		private static int updateLevel;
		private static Font viewFont = new Font("Courier New", 9.75F, GraphicsUnit.Point);

		#endregion

		#region Public Events

		public static event EventHandler OptionsChanged;

		#endregion

		#region Non-Event-Firing Public Properties

		public static int BinaryFootprintLength { get; set; } = 8;

		public static bool CheckDirExists => checkDirExists;

		public static bool CheckFileExists => checkFileExists;

		public static CompareType CompareType { get; set; }

		public static DirectoryDiffFileFilter FileFilter { get; set; }

		public static bool GoToFirstDiff { get; set; } = true;

		public static HashType HashType => hashType;

		public static bool IgnoreCase { get; set; }

		public static bool IgnoreDirectoryComparison { get; set; } = true;

		public static bool IgnoreTextWhitespace { get; set; }

		public static bool IgnoreXmlWhitespace { get; set; }

		public static string LastDirA { get; set; } = string.Empty;

		public static string LastDirB { get; set; } = string.Empty;

		public static string LastFileA { get; set; } = string.Empty;

		public static string LastFileB { get; set; } = string.Empty;

		public static string LastTextA { get; set; } = string.Empty;

		public static string LastTextB { get; set; } = string.Empty;

		public static int LineDiffHeight { get; set; }

		public static bool OnlyShowDirDialogIfShiftPressed { get; set; }

		public static bool OnlyShowFileDialogIfShiftPressed { get; set; }

		public static bool Recursive { get; set; } = true;

		public static bool ShowChangeAsDeleteInsert { get; set; }

		public static bool ShowDifferent { get; set; } = true;

		public static bool ShowOnlyInA { get; set; } = true;

		public static bool ShowOnlyInB { get; set; } = true;

		public static bool ShowSame { get; set; } = true;

		#endregion

		#region Event-Firing Public Properties

		public static bool ShowWSInLineDiff
		{
			get
			{
				return showWSInLineDiff;
			}

			set
			{
				SetValue(ref showWSInLineDiff, value);
			}
		}

		public static bool ShowWSInMainDiff
		{
			get
			{
				return showWSInMainDiff;
			}

			set
			{
				SetValue(ref showWSInMainDiff, value);
			}
		}

		public static Font ViewFont
		{
			get
			{
				return viewFont;
			}

			set
			{
				SetValue(ref viewFont, value);
			}
		}

		public static bool ShowMdiTabs
		{
			get
			{
				return showMdiTabs;
			}

			set
			{
				SetValue(ref showMdiTabs, value);
			}
		}

		#endregion

		#region Public Methods

		public static bool IsShiftPressed
			/* We have to use Keys.Shift instead of Keys.ShiftKey because "Shift"
			is "The SHIFT modifier key", and it's what ModifierKeys returns. */
			=> (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

		public static void AddCustomFilter(string filter)
		{
			// Case-insensitively look for the current filter
			int index = -1;
			for (int i = 0; i < customFilters.Count; i++)
			{
				if (string.Compare(customFilters[i], filter, true) == 0)
				{
					index = i;
					break;
				}
			}

			// Remove the old filter if necessary and add it
			// back at the beginning of the list.
			if (index >= 0)
			{
				customFilters.RemoveAt(index);
			}

			customFilters.Insert(0, filter);

			// Limit the history count to 20.
			const int MaxFilters = 20;
			while (customFilters.Count > MaxFilters)
			{
				customFilters.RemoveAt(customFilters.Count - 1);
			}
		}

		public static void BeginUpdate()
		{
			updateLevel++;
		}

		public static bool DirExists(string directoryName)
		{
			if (CheckDirExists)
			{
				return Directory.Exists(directoryName);
			}
			else
			{
				return true;
			}
		}

		public static void EndUpdate()
		{
			updateLevel--;

			if (updateLevel == 0 && changed)
			{
				changed = false;
				OptionsChanged?.Invoke(null, EventArgs.Empty);
			}
		}

		public static bool FileExists(string fileName)
		{
			if (CheckFileExists)
			{
				return File.Exists(fileName);
			}
			else
			{
				return true;
			}
		}

		public static string[] GetCustomFilters()
		{
			int numFilters = customFilters.Count;
			string[] filters = new string[numFilters];
			for (int i = 0; i < numFilters; i++)
			{
				filters[i] = customFilters[i];
			}

			return filters;
		}

		public static void Load(ISettingsNode node)
		{
			showWSInMainDiff = node.GetValue("ShowWSInMainDiff", false);
			showWSInLineDiff = node.GetValue("ShowWSInLineDiff", true);
			IgnoreCase = node.GetValue("IgnoreCase", IgnoreCase);
			IgnoreTextWhitespace = node.GetValue("IgnoreTextWhitespace", IgnoreTextWhitespace);
			CompareType = node.GetValue("CompareType", CompareType);
			ShowOnlyInA = node.GetValue("ShowOnlyInA", ShowOnlyInA);
			ShowOnlyInB = node.GetValue("ShowOnlyInB", ShowOnlyInB);
			ShowDifferent = node.GetValue("ShowDifferent", ShowDifferent);
			ShowSame = node.GetValue("ShowSame", ShowSame);
			Recursive = node.GetValue("Recursive", Recursive);
			IgnoreDirectoryComparison = node.GetValue("IgnoreDirectoryComparison", IgnoreDirectoryComparison);
			OnlyShowFileDialogIfShiftPressed = node.GetValue("OnlyShowFileDialogIfShiftPressed", OnlyShowFileDialogIfShiftPressed);
			OnlyShowDirDialogIfShiftPressed = node.GetValue("OnlyShowDirDialogIfShiftPressed", OnlyShowDirDialogIfShiftPressed);
			GoToFirstDiff = node.GetValue("GoToFirstDiff", GoToFirstDiff);
			checkFileExists = node.GetValue("CheckFileExists", true);
			checkDirExists = node.GetValue("CheckDirExists", true);
			ShowChangeAsDeleteInsert = node.GetValue("ShowChangeAsDeleteInsert", ShowChangeAsDeleteInsert);
			showMdiTabs = node.GetValue("ShowMdiTabs", true);

			hashType = node.GetValue<HashType>("HashType", HashType.HashCode);
			IgnoreXmlWhitespace = node.GetValue("IgnoreXmlWhitespace", IgnoreXmlWhitespace);
			LineDiffHeight = node.GetValue("LineDiffHeight", LineDiffHeight);
			BinaryFootprintLength = node.GetValue("BinaryFootprintLength", BinaryFootprintLength);

			LastFileA = node.GetValue("LastFileA", LastFileA);
			LastFileB = node.GetValue("LastFileB", LastFileB);
			LastDirA = node.GetValue("LastDirA", LastDirA);
			LastDirB = node.GetValue("LastDirB", LastDirB);
			/* Note: We don't save or load the last text. */

			// Consolas has been around for 5+ years now, and it renders without misaligned hatch brushes when scrolling.
			string fontName = GetInstalledFontName(node.GetValue("FontName", "Consolas"), "Consolas", "Courier New", FontFamily.GenericMonospace.Name);
			FontStyle fontStyle = node.GetValue<FontStyle>("FontStyle", FontStyle.Regular);
			float fontSize = float.Parse(node.GetValue("FontSize", "9.75"));
			viewFont = new Font(fontName, fontSize, fontStyle, GraphicsUnit.Point);

			// Load custom filters
			customFilters.Clear();
			node = node.GetSubNode("Custom Filters", false);
			if (node == null)
			{
				// It appears to be the first time the program has run,
				// so add in some default filters.
				customFilters.Add("*.cs");
				customFilters.Add("*.cpp;*.h;*.idl;*.rc;*.c;*.inl");
				customFilters.Add("*.vb");
				customFilters.Add("*.xml");
				customFilters.Add("*.htm;*.html");
				customFilters.Add("*.txt");
				customFilters.Add("*.sql");
				customFilters.Add("*.obj;*.pdb;*.exe;*.dll;*.cache;*.tlog;*.trx;*.FileListAbsolute.txt");
			}
			else
			{
				IList<string> names = node.GetSettingNames();
				for (int i = 0; i < names.Count; i++)
				{
					customFilters.Add(node.GetValue(names[i], string.Empty));
				}
			}
		}

		public static void Save(ISettingsNode node)
		{
			node.SetValue("CompareType", CompareType);

			node.SetValue("ShowWSInMainDiff", showWSInMainDiff);
			node.SetValue("ShowWSInLineDiff", showWSInLineDiff);
			node.SetValue("IgnoreCase", IgnoreCase);
			node.SetValue("IgnoreTextWhitespace", IgnoreTextWhitespace);
			node.SetValue("ShowOnlyInA", ShowOnlyInA);
			node.SetValue("ShowOnlyInB", ShowOnlyInB);
			node.SetValue("ShowDifferent", ShowDifferent);
			node.SetValue("ShowSame", ShowSame);
			node.SetValue("Recursive", Recursive);
			node.SetValue("IgnoreDirectoryComparison", IgnoreDirectoryComparison);
			node.SetValue("OnlyShowFileDialogIfShiftPressed", OnlyShowFileDialogIfShiftPressed);
			node.SetValue("OnlyShowDirDialogIfShiftPressed", OnlyShowDirDialogIfShiftPressed);
			node.SetValue("GoToFirstDiff", GoToFirstDiff);
			node.SetValue("CheckFileExists", checkFileExists);
			node.SetValue("CheckDirExists", checkDirExists);
			node.SetValue("ShowChangeAsDeleteInsert", ShowChangeAsDeleteInsert);
			node.SetValue("IgnoreXmlWhitespace", IgnoreXmlWhitespace);
			node.SetValue("ShowMdiTabs", showMdiTabs);

			node.SetValue("HashType", hashType);
			node.SetValue("LineDiffHeight", LineDiffHeight);
			node.SetValue("BinaryFootprintLength", BinaryFootprintLength);

			node.SetValue("LastFileA", LastFileA);
			node.SetValue("LastFileB", LastFileB);
			node.SetValue("LastDirA", LastDirA);
			node.SetValue("LastDirB", LastDirB);
			/* Note: We don't save or load the last text. */

			node.SetValue("FontName", viewFont.Name);
			node.SetValue("FontStyle", viewFont.Style);
			node.SetValue("FontSize", Convert.ToString(viewFont.SizeInPoints));

			// Save custom filters
			if (node.GetSubNode("Custom Filters", false) != null)
			{
				node.DeleteSubNode("Custom Filters");
			}

			if (customFilters.Count > 0)
			{
				node = node.GetSubNode("Custom Filters", true);
				for (int i = 0; i < customFilters.Count; i++)
				{
					node.SetValue(i.ToString(), customFilters[i]);
				}
			}
		}

		#endregion

		#region Private Methods

		private static string GetInstalledFontName(params string[] fontNames)
		{
			string result = null;

			foreach (string fontName in fontNames)
			{
				// Set result here, so if none of the fonts are installed,
				// then we'll at least return the last name passed in.
				result = fontName;

				// http://stackoverflow.com/questions/113989/test-if-a-font-is-installed
				Font font = new Font(fontName, 9.75f);
				if (font.Name == fontName)
				{
					break;
				}
			}

			return result;
		}

		private static void SetValue<T>(ref T member, T value)
		{
			if (!object.Equals(member, value))
			{
				BeginUpdate();
				member = value;
				changed = true;
				EndUpdate();
			}
		}

		#endregion
	}
}
