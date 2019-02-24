namespace AehnlichLib.Enums
{
    /// <summary>
    /// Defines a type of edit operation that is necessary to transform stringA into stringB.
	/// 
	/// This is applicable to use case where users want to compare stringA with stringB and
	/// which to display differences (mergeing text). Typically, these differences are based
	/// on a type of difference (insert, delete, change, none) which is denoted by this enumeration.
    /// </summary>
    public enum EditType
    {
        /// <summary>
        /// The sequences in stringA and stringB are equal. Therefore, no edit operation is required
        /// to transform stringA into stringB.
        /// </summary>
        None,

        /// <summary>
        /// The sequences in stringA and stringB are NOT equal.
        /// A delete operation (eg: delete letter 'A' in position n)
        /// in stringA is required to make stringA
        /// more equal in comparison towards stringB.
        /// </summary>
        Delete,

        /// <summary>
        /// The sequences in stringA and stringB are NOT equal.
        /// An insert operation (eg: insert letter 'A' in position n)
        /// in stringA is required to make stringA
        /// more equal in comparison towards stringB.
        /// </summary>
        Insert,

        /// <summary>
        /// The sequences in stringA and stringB are NOT equal.
        /// An change operation (eg: exchange letter 'A' in position n with letter 'B')
        /// in stringA is required to make stringA more equal in comparison towards stringB.
        /// </summary>
        Change
    }
}
