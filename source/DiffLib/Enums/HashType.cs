namespace DiffLib.Enums
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the type of hashing algorithm that is used to
    /// efficiently compare a sequenceA with sequenceB and determine
    /// whether they can be matched or not.
    /// </summary>
    public enum HashType
    {
        /// <summary>
        /// Default for hash based comparison of sequenceA and sequenceB is based on the GetHashCode() method
        /// implemented in <see cref="Object"/> class or its override of an inheriting class
        /// (eg. new string("XXX").GetHashCode()).
        ///
        /// https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode
        /// </summary>
        HashCode,

        /// <summary>
	    /// The hash based comparison of sequenceA and sequenceB is based on the CRC32 algorithm.
        /// For more info see:
	    /// http://www.efg2.com/Lab/Mathematics/CRC.htm.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Crc",
            Justification = "CRC is for 'cyclic redundancy check'.")]
        Crc32,

        /// <summary>
	    /// The hash based comparison of sequenceA and sequenceB is based on the unique occurence of a sequence.
        /// That is, each unique sequence will have the same hash code. Consider fpr example the following
	    /// sequence of strings (sequences):
        ///
        /// Number String    Hash Value
        /// 0      'XXXX' -> 0
        /// 1      'Abc'  -> 1
        /// 2      'cda'  -> 2
        /// 3      'XXXX' -> 0
        /// 4      'Abc'  -> 1
        ///
        /// The hash value is derived from the line number in which a given unique string occured for the first time.
        /// </summary>
        Unique
    }
}
