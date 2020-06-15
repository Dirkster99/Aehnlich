namespace AehnlichViewModelsLib.Enums
{
	/// <summary>Models a diff (text) document which can be either A (left)  or B (right) when comparing 2 documents side by side</summary>
	public enum ViewSource
	{
		/// <summary>Indicates document A (left)  when comparing 2 documents side by side</summary>
		Left,

		/// <summary>Indicates document B (right)  when comparing 2 documents side by side</summary>
		Right
	}
}
