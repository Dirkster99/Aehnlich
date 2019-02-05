namespace DiffLib.BinaryFileDiff
{
    using DiffLib.Interfaces;

    public sealed class Addition : IAddCopy
	{
		#region Private Data Members

		private byte[] bytes;

		#endregion

		#region Constructors

		internal Addition(byte[] bytes)
		{
			this.bytes = bytes;
		}

		#endregion

		#region Public Properties

		public bool IsAdd => true;

		#endregion

		#region Public Methods

		public byte[] GetBytes() => this.bytes;

		#endregion
	}
}
