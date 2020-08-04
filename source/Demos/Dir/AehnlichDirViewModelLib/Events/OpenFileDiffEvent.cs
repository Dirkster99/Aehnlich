namespace AehnlichDirViewModelLib.Events
{
	using AehnlichDirViewModelLib.Interfaces;
	using AehnlichLib.Enums;
	using System;

	/// <summary>
	/// Implements an event that notifies the listner about the user wanting to compare
	/// the contents of 2 files against each other.
	/// </summary>
	public class OpenFileDiffEventArgs : EventArgs
	{
		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="pathItem"></param>
		/// <param name="compareAs"></param>
		public OpenFileDiffEventArgs(IDirEntryViewModel pathItem, CompareType compareAs)
			: this(pathItem)
		{
			CompareAs = compareAs;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="pathItem"></param>
		public OpenFileDiffEventArgs(IDirEntryViewModel pathItem)
			: this()
		{
			ItemPathA = pathItem.ItemPathA;
			ItemPathB = pathItem.ItemPathB;

			IsItemInA = pathItem.IsItemInA;
			IsItemInB = pathItem.IsItemInB;

			IsFile = pathItem.IsFile;
		}

		/// <summary>
		/// Hidden class constructor
		/// </summary>
		protected OpenFileDiffEventArgs()
		{
			CompareAs = CompareType.Auto;
		}
		#endregion ctors

		#region properties
		/// <summary>
		/// Gets the full path of item A (file or directory A) if it exists.
		/// <see cref="IsItemInA"/>
		/// </summary>
		public string ItemPathA { get; }

		/// <summary>
		/// Gets the full path of item B (file or directory A) if it exists.
		/// <see cref="IsItemInB"/>
		/// </summary>
		public string ItemPathB { get; }

		/// <summary>
		/// Gets whether the item A (file or directory A) actually exists or not.
		/// Item B (file or directory B) may exist in the case that A does not exist
		/// and this entry may then be here to represent that difference.
		/// </summary>
		public bool IsItemInA { get; }

		/// <summary>
		/// Gets whether the item B (file or directory B) actually exists or not.
		/// Item A (file or directory A) may exist in the case that B does not exist
		/// and this entry may then be here to represent that difference.
		/// </summary>
		public bool IsItemInB { get; }

		/// <summary>
		/// Gets whether this entry represent a file (true), or not (directory or drive).
		/// </summary>
		public bool IsFile { get; }

		/// <summary>
		/// Determines the mode of comparison when files are compared (default is Auto).
		/// </summary>
		public CompareType CompareAs { get; }
		#endregion properties
	}
}
