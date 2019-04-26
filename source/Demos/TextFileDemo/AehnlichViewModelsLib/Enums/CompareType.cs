namespace AehnlichViewModelsLib.Enums
{
    /// <summary>
    /// Controls the way of how the diff engine should diff the given information.
    /// </summary>
    public enum CompareType
    {
        /// <summary>
        /// Determine the right mode of diff automatically.
        /// </summary>
        Auto,

        /// <summary>
        /// Diff files as text files.
        /// </summary>
        Text,

        /// <summary>
        /// Diff files as XML files.
        /// </summary>
        Xml,

        /// <summary>
        /// Diff files as binary files.
        /// </summary>
        Binary
    }
}
