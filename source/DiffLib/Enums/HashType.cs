namespace DiffLib.Enums
{
    using System.Diagnostics.CodeAnalysis;

    public enum HashType
    {
        HashCode,

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Crc",
            Justification = "CRC is for 'cyclic redundancy check'.")]
        Crc32,

        Unique
    }
}
