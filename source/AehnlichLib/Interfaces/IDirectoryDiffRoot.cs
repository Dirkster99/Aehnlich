namespace AehnlichLib.Interfaces
{
    using AehnlichLib.Dir;

    public interface IDirectoryDiffRoot
    {
		string RootPathA { get; }
	
		string RootPathB { get; }
		
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
    }
}
