namespace AehnlichViewLib.Enums
{
	/// <summary>
	/// Enumerates UI elements in the diff view that can be focused by default.
	/// </summary>
	public enum Focus
	{
		/// <summary>No UI element to focus by default.</summary>
		None,

		/// <summary>Focus the left diff view UI element (control) by default.</summary>
		LeftView,

		/// <summary>Focus the right diff view UI element (control) by default.</summary>
		RightView
	}
}
