namespace AehnlichLib.Text
{
    using AehnlichLib.Enums;
    using System.Diagnostics;

    /// <summary>
    /// Class models a single edit operation that should be applied to stringA in order to make
	/// it equal to another stringB.
    /// </summary>
    [DebuggerDisplay("StartA = {StartA}, StartB = {StartB}, Length = {Length}")]
	public sealed class Edit
	{
		#region Private Data Members

		private readonly EditType editType;
		private readonly int length; // Length of the Delete, Insert or Change in the "A" or "B" sequence
		private int startA;	// Where to Delete, Insert, or Change in the "A" sequence
		private int startB;	// Where to Insert or Change in the "B" sequence

		#endregion

		#region Constructors
	    /// <summary>
	    /// Class constructor.
	    /// </summary>
        /// <param name="editType">Models the type of the edit operation
		/// that should be applied to transform string_A into String_B.</param>
        /// <param name="startA">The starting offset where the edit operation should be applied.</param>
        /// <param name="startB">The starting offset where the application of the edit operation
		/// will lead to equality in this starting position of stringB.</param>
        /// <param name="length">The length of the sub-string that is affected in stringA or stringB
		/// when the edit operation is applied in stringA or the inverted edit operation is applied in stringB.</param>
		internal Edit(EditType editType, int startA, int startB, int length)
		{
			this.editType = editType;
			this.startA = startA;
			this.startB = startB;
			this.length = length;
		}
		#endregion

		#region Public Properties

	    /// <summary>
	    /// Gets the Length of the Insert, Delete or Change in the "A" or "B" sequence
	    /// </summary>
		public int Length => this.length;

	    /// <summary>
	    /// Gets the index at which to Delete, Insert, or Change in the "A" sequence.
	    /// </summary>
		public int StartA => this.startA;

	    /// <summary>
	    /// Gets the index at which to Delete, Insert, or Change in the "B" sequence.
	    /// </summary>
		public int StartB => this.startB;

	    /// <summary>
	    /// Gets the type of change (Delete, Insert, None or Change) in the "A" sequence.
	    /// </summary>
		public EditType EditType => this.editType;

		#endregion

		#region Public Methods
	    /// <summary>
	    /// Adds an offset to <see cref="StartA"/> and <see cref="StartB"/>.
	    /// </summary>
        /// <param name="offsetA">The offset to add to <see cref="StartA"/>.</param>
        /// <param name="offsetB">The offset to add to <see cref="StartB"/>.</param>
		public void Offset(int offsetA, int offsetB)
		{
			this.startA += offsetA;
			this.startB += offsetB;
		}
		#endregion
	}
}
