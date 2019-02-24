namespace AehnlichViewLib.Enums
{
    /// <summary>
    /// Enumerates a model value that hints whether 2 items are different
    /// (one of them was Added or Deleted or both are there but different: Changed)
    /// or if there is no difference between both lines (Context).
    /// </summary>
    public enum DiffContext
    {
        /// <summary>
        /// The left or right part of the compared items was added.
        /// </summary>
        Added,

        /// <summary>
        /// The left or right part of the compared items was deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// The left and right part are similar enough to hint that they are not
        /// the same but have changed between each other.
        /// </summary>
        Context,

        /// <summary>
        /// Both compared items are empty (eg. contain strings with length 0).
        /// </summary>
        Blank
    }
}
