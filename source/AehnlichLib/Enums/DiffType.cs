namespace AehnlichLib.Enums
{
    /// <summary>
    /// Describes the types of target object use case for the comparison
    /// of 2 files, 2 directories, or 2 text documents.
    /// </summary>
    public enum DiffType
    {
        /// <summary>
        /// Not determined indicates an error if this ever pops-up in an
        /// constructed object when comparison is intended.
        /// </summary>
        Unknown,

        /// <summary>
        /// We are comparing files here.
        /// </summary>
        File,

        /// <summary>
        /// We are comparing directories here.
        /// </summary>
        Directory,

        /// <summary>
        /// We are comparing text here.
        /// </summary>
        Text
    }
}
