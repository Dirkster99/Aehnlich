namespace AehnlichLib.Dir
{
	using AehnlichLib.Interfaces;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

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
