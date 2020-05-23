namespace AehnlichLib.Text
{
	using AehnlichLib.Enums;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Implements an edit script object that holds a sequence of operations
	/// (none, insert, delete, change) necessary to transform sequenceA into sequenceB.
	///
	/// The resulting object is a collection of <see cref="Edit"/> objects,
	/// which in turn, define each edit operation (none, delete, insert, replace)
	/// at the appropriate position and length in sequenceA.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
		Justification = "Using the same name as in Myers's paper.")]
	public sealed class EditScript : ReadOnlyCollection<Edit>
	{
		#region Private Data Members
		private int totalEditLength;
		#endregion

		#region Constructors
		/// <summary>
		/// Class constructor.
		/// </summary>
		internal EditScript(double similarity)
			: base(new List<Edit>())
		{
			this.Similarity = similarity;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the sum of all edit lengths <see cref="Edit.Length"/>
		/// of all <see cref="Edit"/> objects in this collection.
		///
		/// A Change operation is multiplied with a factor of 2
		/// while all other operations Insert, Delete, and None (?) are counted as 1 edit.
		/// </summary>
		public int TotalEditLength => this.totalEditLength;

		/// <summary>
		/// Gets a similarity value that must be available at contructor time
		/// (computed externally).
		/// </summary>
		public double Similarity { get; }

		#endregion

		#region Internal Methods

		/// <summary>
		/// Adds another edit operation into the sequence of operations.
		/// </summary>
		internal void Add(Edit edit)
		{
			if (edit.EditType == EditType.Change)
			{
				this.totalEditLength += 2 * edit.Length;
			}
			else
			{
				this.totalEditLength += edit.Length;
			}

			this.Items.Add(edit);
		}

		#endregion
	}
}
