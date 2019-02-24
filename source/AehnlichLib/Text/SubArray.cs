namespace AehnlichLib.Text
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Allows 1..M access for a selected portion of an int array.
    /// </summary>
    internal sealed class SubArray<T> where T : IComparable<T>
	{
		#region Private Data Members

		private readonly IList<T> data;
		private readonly int length; // Stores the length of this subarray
		private readonly int offset; // Stores the 0-based offset into this.arData where this subarray should start

		#endregion

		#region Constructors

		public SubArray(IList<T> data)
		{
			this.data = data;
			this.offset = 0;
			this.length = this.data.Count;
		}

		public SubArray(SubArray<T> data, int offset, int length)
		{
			this.data = data.data;

			// Subtract 1 here because offset will be 1-based
			this.offset = data.offset + offset - 1;
			this.length = length;
		}

		#endregion

		#region Public Properties

		public int Length => this.length;

		public int Offset => this.offset;

		public T this[int index] => this.data[this.offset + index - 1];

		#endregion

		#region Public Methods

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(3 * this.length);
			for (int i = 0; i < this.length; i++)
			{
				sb.AppendFormat("{0} ", this.data[this.offset + i]);
			}

			return sb.ToString();
		}

		#endregion
	}
}
