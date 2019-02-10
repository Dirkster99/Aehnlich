namespace DiffLib.Text
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Implements a vector from -MAX to MAX
    /// </summary>
    internal sealed class DiagonalVector
    {
        #region Private Data Members

        private readonly int[] data;
        private readonly int max;

        #endregion

        #region Constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        public DiagonalVector(int n, int m)
        {
            int delta = n - m;

            // We have to add Delta to support reverse vectors, which are
            // centered around the Delta diagonal instead of the 0 diagonal.
            this.max = n + m + Math.Abs(delta);

            // Create an array of size 2*MAX+1 to hold -MAX..+MAX.
            this.data = new int[(2 * this.max) + 1];
        }
        #endregion

        #region Public Properties

        public int this[int userIndex]
        {
            get
            {
                return this.data[this.GetActualIndex(userIndex)];
            }

            set
            {
                this.data[this.GetActualIndex(userIndex)] = value;
            }
        }

        #endregion

        #region Private Methods

        private int GetActualIndex(int userIndex)
        {
            int result = userIndex + this.max;
            Debug.Assert(result >= 0 && result < this.data.Length, "The actual index must be within the actual data array's bounds.");
            return result;
        }

        #endregion
    }
}
