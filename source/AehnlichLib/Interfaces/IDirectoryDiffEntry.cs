namespace AehnlichLib.Interfaces
{
    using AehnlichLib.Dir;
    using AehnlichLib.Enums;
    using System;

    public interface IDirectoryDiffEntry
    {
        #region Public Properties
        bool Different { get; set; }

        string Error { get; set; }

        bool InA  { get; }

        bool InB  { get; }

        /// <summary>
        /// Gets the context of this diff (none, delete, add, change)
        /// as an enumeration value.
        /// </summary>
        EditType EditContext { get; }

        bool IsFile  { get; }

        string BasePath { get; }

        string Name { get; }

        /// <summary>
        /// Gets the last time this item has been changed through a write access.
        /// </summary>
		DateTime LastUpdateA { get; }

        /// <summary>
        /// Gets the last time this item has been changed through a write access.
        /// </summary>
        DateTime LastUpdateB { get; }

        /// <summary>
        /// Gets the size, in bytes, of the current file system item.
        /// </summary>
        double LengthA { get; set; }

        /// <summary>
        /// Gets the size, in bytes, of the current file system item.
        /// </summary>
        double LengthB { get; set; }

        DirectoryDiffEntryCollection Subentries { get; }
        #endregion properties

        #region methods
        void AddSubEntry(IDirectoryDiffEntry entry);

        int CountSubDirectories();

        bool SetDiffBasedOnChildren(bool ignoreDirectoryComparison);
        #endregion methods
    }
}
