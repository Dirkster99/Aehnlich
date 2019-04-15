namespace AehnlichLib.Interfaces
{
    using AehnlichLib.Dir;

    /// <summary>
    /// Defines an object root of an object data graph that models directories
    /// A (left side) and B (right side), their sub-directories, and their files.
    /// 
    /// The data in this structure captures directory and file differences in either
    /// (sub-)directory in terms of directories and/or files that have beend:
    /// deleted, added, changed, or appear to be equal in both.
    /// </summary>
    public interface IDirectoryDiffRoot
    {
        /// <summary>
        /// Gets the root directory level path of the diff directory A (left side).
        /// </summary>
        string RootPathA { get; }

        /// <summary>
        /// Gets the root directory level path of directory B (right side).
        /// </summary>
        string RootPathB { get; }

        /// <summary>
        /// Gets whether this root represents a diff between two directories
        /// with more than one level (false) or multiple levels of sub-directories (true).
        /// </summary>
        bool Recursive { get; }
		
		DirectoryDiffFileFilter Filter { get; }

        /// <summary>
        /// Gets a hierarchical collection of directories (and their files)
		/// that are different in the directories below <see cref="RootPathA"/> and <see cref="RootPathB"/>.
        /// </summary>
        IDirectoryDiffEntry RootEntry { get; }

        /// <summary>
        /// Gets a collection of files that are different in the directories below
		/// <see cref="RootPathA"/> and <see cref="RootPathB"/>.
        /// </summary>
        DirectoryDiffEntryCollection DifferentFiles { get; }

        /// <summary>
        /// Gets the number of files deleted in directory A (left side)
        /// when comparing directory A (left side) with directory B (right side).
        /// </summary>
        int CountFilesDeleted { get; }

        /// <summary>
        /// Gets the number of files added in directory B (left side)
        /// when comparing directory A (left side) with directory B (right side).
        /// </summary>
        int CountFilesAdded { get; }

        /// <summary>
        /// Gets the number of changes files in directory A and B
        /// when comparing directory A (left side) with directory B (right side).
        /// </summary>
        int CountFilesChanged { get; }
    }
}
