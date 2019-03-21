namespace AehnlichLib.Dir
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using AehnlichLib.Interfaces;

    public sealed class DirectoryDiffEntryCollection : ReadOnlyCollection<IDirectoryDiffEntry>
	{
		#region Constructors

		internal DirectoryDiffEntryCollection()
			: base(new List<IDirectoryDiffEntry>())
		{
		}

		#endregion

		#region Internal Methods

		internal void Add(IDirectoryDiffEntry entry)
		{
			this.Items.Add(entry);
		}

		#endregion
	}
}
