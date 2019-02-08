namespace DiffLib.Text
{
    using DiffLib.Enums;
    using System.Collections.Generic;

    /// <summary>
    /// This class uses the MyersDiff helper class to difference two
    /// string lists.  It hashes each string in both lists and then
    /// differences the resulting integer arrays.
    /// </summary>
    public sealed class TextDiff
	{
		#region Private Data Members

		private bool supportChangeEditType;
		private StringHasher hasher;

		#endregion

		#region Constructors
	    /// <summary>
	    /// Class constructor.
	    /// </summary>
        /// <param name="hashType">The type of hashing algorithm to associate a given text string
		/// with an int value in order to efficiently compare strings.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case ('A' versus 'a')
		/// when comparing strings. Strings that contain the same letters in different cases
		/// are considered to be a match.
		/// Two strings like 'A' and 'a' are considered equal.</param>
        /// <param name="ignoreWhiteSpace">Whether to ignore starting and ending white spaces
		/// when comparing strings. Strings that contain only whitespaces are considered equal.
		/// Two strings like '  A' and 'A  ' are considered equal.</param>
		public TextDiff(HashType hashType,
		                bool ignoreCase,
						bool ignoreWhiteSpace)
			: this(hashType, ignoreCase, ignoreWhiteSpace, 0, true)
		{
		}

	    /// <summary>
	    /// Class constructor.
	    /// </summary>
        /// <param name="hashType">The type of hashing algorithm to associate a given text string
		/// with an int value in order to efficiently compare strings.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case ('A' versus 'a')
		/// when comparing strings. Strings that contain the same letters in different cases
		/// are considered to be a match.
		/// Two strings like 'A' and 'a' are considered equal.</param>
        /// <param name="ignoreWhiteSpace">Whether to ignore starting and ending white spaces
		/// when comparing strings. Strings that contain only whitespaces are considered equal.
		/// Two strings like '  A' and 'A  ' are considered equal.</param>
		/// <param name="leadingCharactersToIgnore">Whether to ignore the first n characters
		/// when comparing a stringA with stringB.</param>
		/// <param name="supportChangeEditType">Configures whether the underlying engine reports
		/// a Change edit type to express a difference or will use only Insert and Delete
		/// edit types to express differences between two given strings.</param>
		public TextDiff(HashType hashType,
						bool ignoreCase,
						bool ignoreWhiteSpace,
						int leadingCharactersToIgnore,
						bool supportChangeEditType)
		{
			this.hasher = new StringHasher(hashType, ignoreCase, ignoreWhiteSpace, leadingCharactersToIgnore);
			this.supportChangeEditType = supportChangeEditType;
		}
		#endregion

		#region Public Methods
	    /// <summary>
	    /// Gets an <see cref="EditScript"/> for comparing a list of right and left strings
		/// (sometimes also refered to as stringA and stringB or sequenceA and sequenceB).
	    /// </summary>
		/// <param name="listA">sequenceA</param>
		/// <param name="listB">sequenceB</param>
		public EditScript Execute(IList<string> listA, IList<string> listB)
		{
			// Convert input string lists into arrays of integer hash values
			int[] hashA = this.HashStringList(listA);
			int[] hashB = this.HashStringList(listB);

			// Construct MyersDiff<int> object from arrays of int hash values that represent the original string input
			MyersDiff<int> diff = new MyersDiff<int>(hashA, hashB, this.supportChangeEditType);

			// Gets an EditScript instance that describes all Edits necessary to transform ListA into ListB.
			EditScript result = diff.Execute();

			return result;
		}
		#endregion

		#region Private Methods
	    /// <summary>
	    /// Converts a list of strings into an array of integer hash values.
	    /// </summary>
		private int[] HashStringList(IList<string> lines)
		{
			int numLines = lines.Count;
			int[] result = new int[numLines];

			for (int i = 0; i < numLines; i++)
			{
				result[i] = this.hasher.GetHashCode(lines[i]);
			}

			return result;
		}

		#endregion
	}
}
