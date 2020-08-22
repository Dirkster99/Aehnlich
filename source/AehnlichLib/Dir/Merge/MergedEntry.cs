namespace AehnlichLib.Dir.Merge
{
	using FsDataLib.Interfaces.Dir;
	using System.Diagnostics;

	[DebuggerDisplay("InfoA = {InfoA}, InfoB = {InfoB}, BothGotChildren = {BothGotChildren}")]
	internal class MergedEntry
	{
		#region ctors
		public MergedEntry(IFileSystemInfo infoA,
							IFileSystemInfo infoB)
			: this()
		{
			this.InfoA = infoA;
			this.InfoB = infoB;
		}

		/// <summary>
		/// Hidden standard constructor
		/// </summary>
		protected MergedEntry()
		{
		}
		#endregion ctors

		public IFileSystemInfo InfoA { get; }

		public IFileSystemInfo InfoB { get; }
	}
}
