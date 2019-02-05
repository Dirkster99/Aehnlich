namespace DiffLib.BinaryFileDiff
{
    using DiffLib.Interfaces;

    public sealed class Copy : IAddCopy
	{
		#region Constructors

		internal Copy(int baseOffset, int length)
		{
			this.BaseOffset = baseOffset;
			this.Length = length;
		}

		#endregion

		#region Public Properties

		public bool IsAdd => false;

		public int BaseOffset { get; }

		public int Length { get; }

		#endregion
	}
}
