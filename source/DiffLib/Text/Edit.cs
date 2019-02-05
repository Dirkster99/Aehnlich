namespace DiffLib.Text
{
    using DiffLib.Enums;
    using System.Diagnostics;

    [DebuggerDisplay("Type = {Type}, StartA = {StartA}, StartB = {StartB}, Length = {Length}")]
	public sealed class Edit
	{
		#region Private Data Members

		private EditType editType;
		private int length;
		private int startA;	// Where to Delete, Insert, or Change in the "A" sequence
		private int startB;	// Where to Insert or Change in the "B" sequence

		#endregion

		#region Constructors

		internal Edit(EditType editType, int startA, int startB, int length)
		{
			this.editType = editType;
			this.startA = startA;
			this.startB = startB;
			this.length = length;
		}

		#endregion

		#region Public Properties

		public int Length => this.length;

		public int StartA => this.startA;

		public int StartB => this.startB;

		public EditType EditType => this.editType;

		#endregion

		#region Public Methods

		public void Offset(int offsetA, int offsetB)
		{
			this.startA += offsetA;
			this.startB += offsetB;
		}

		#endregion
	}
}
