namespace AehnlichLib.Enums
{
    /// <summary>
    /// Determines the mode of matching that is implemented to determine whether two files
    /// in two different directories are equal or not.
    /// </summary>
    public enum DiffDirFileMode : uint
    {
        /// <summary>
        /// Two files in the same relative location A (left dir tree) and B (right dir tree)
        /// are considered equal:
        /// 1) if their file name is the same and if their byte length is equal. 
        /// </summary>
        ByteLength = 1,

        LastUpdate = 2,

        AllBytes = 4,

        /// <summary>
        /// Two files in the same relative location A (left dir tree) and B (right dir tree)
        /// are considered equal:
        /// 1) if their file name is the same and if their byte length is equal and
        /// 2) if their date and time of last change is equal.
        /// </summary>
        ByteLength_LastUpdate = (ByteLength | LastUpdate),

        ByteLength_AllBytes = (ByteLength | AllBytes),

        /// <summary>
        /// Two files in the same relative location A (left dir tree) and B (right dir tree)
        /// are considered equal:
        /// 1) if their file name is the same and if their byte length is equal and
        /// 2) if their date and time of last change is equal and
        /// 3) if they are equal in a byte by byte comparison.
        /// </summary>
        ByteLength_LastUpdate_AllBytes = (ByteLength | LastUpdate| AllBytes),

        LastUpdate_AllBytes = (LastUpdate | AllBytes)
    }
}
