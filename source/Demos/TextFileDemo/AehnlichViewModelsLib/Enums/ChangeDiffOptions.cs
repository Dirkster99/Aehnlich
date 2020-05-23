namespace AehnlichViewModelsLib.Enums
{
	/// <summary>
	/// Enumerates different modes of matching text or binary information.
	/// </summary>
	[System.Flags]
	public enum ChangeDiffOptions
	{
		/// <summary>
		/// All differences are considered different (even white spaces tabs etc).
		/// </summary>
		None = 0,

		/// <summary>
		/// Ignores lower or upper case differences in text A or B.
		/// </summary>
		IgnoreCase = 1,

		/// <summary>
		/// Ignores white space differences in text A or B.
		/// </summary>
		IgnoreWhitespace = 2,

		/// <summary>
		/// Ignores a binary prefix in text A or B.
		/// </summary>
		IgnoreBinaryPrefix = 4
	}
}
