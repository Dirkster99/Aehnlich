namespace AehnlichDirViewModelLib.Interfaces
{
    using AehnlichLib.Dir;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines an item in a list of items (directories or files) contained in a directory.
    /// </summary>
    public interface IDirEntryViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the name of item A and item B (file or directory).
        ///	This name is only applicable if both of these items actually exist.
		/// Otherwise, the name may only be applicable to item A or item B
		/// (<see cref="IsItemInA"/> and <see cref="IsItemInB"/>).
        /// </summary>
        string ItemName { get; }

        /// <summary>
        /// Gets the full path of item A (file or directory A) if it exists.
		/// <see cref="IsItemInA"/>
        /// </summary>
        string ItemPathA { get; }

        /// <summary>
        /// Gets the full path of item B (file or directory A) if it exists.
		/// <see cref="IsItemInB"/>
        /// </summary>
        string ItemPathB { get; }

        /// <summary>
        /// Gets whether the item A (file or directory A) actually exists or not.
		/// Item B (file or directory B) may exist in the case that A does not exist
		/// and this entry may then be here to represent that difference.
        /// </summary>
        bool IsItemInA { get; }

        /// <summary>
        /// Gets whether the item B (file or directory B) actually exists or not.
		/// Item A (file or directory A) may exist in the case that B does not exist
		/// and this entry may then be here to represent that difference.
        /// </summary>
        bool IsItemInB { get; }

        /// <summary>
        /// Gets whether the item A (file or directory A) and item B (file or directory B)
		/// in this entry are equal (false) or not (true). Inequality (true) indicates that
		/// only one of the given items actually exists or their content is simply different.
        /// </summary>
        bool IsItemDifferent { get; }

        /// <summary>
        /// Gets whether this entry represent a file (true), or not (directory or drive).
        /// </summary>
        bool IsFile { get; }

        /// <summary>
        /// Gets the size of an item A (file or directory) in bytes.
        /// </summary>
        double ItemLengthA { get; }

        /// <summary>
        /// Gets the size of an item B (file or directory) in bytes.
        /// </summary>
        double ItemLengthB { get; }

        /// <summary>
        /// Gets the last date and time at which an item A (file or directory)
		/// was changed.
        /// </summary>
        DateTime ItemLastUpdateA { get; }

        /// <summary>
        /// Gets the last date and time at which an item B (file or directory)
		/// was changed.
        /// </summary>
        DateTime ItemLastUpdateB { get; }

        /// <summary>
        /// Gets a list of sub-directories and files that are stored underneath this entry.
        /// </summary>
        DirectoryDiffEntryCollection Subentries { get; }
    }
}
