namespace AehnlichLib.Dir.Merge
{
	using AehnlichLib.Interfaces.Dir;
	using System.Diagnostics;
	using System.Linq;

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

		#region properties
		public IFileSystemInfo InfoA { get; }

		public IFileSystemInfo InfoB { get; }

		public bool BothGotChildren
		{
			get
			{
				var dirA = InfoA as IDirectoryInfo;
				var dirB = InfoB as IDirectoryInfo;

				if (dirA == null || dirB == null)
					return false;

				try
				{
					if (dirA.Exists == false || dirB.Exists == false)
						return false;

					return dirA.GetDirectories().Any() && dirB.GetDirectories().Any();
				}
				catch
				{
					// GetDirectories() can throw exceptions
					return false;
				}
			}
		}
		#endregion properties
	}
}
