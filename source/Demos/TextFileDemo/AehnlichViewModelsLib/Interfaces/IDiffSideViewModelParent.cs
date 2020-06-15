namespace AehnlichViewModelsLib.Interfaces
{
	using AehnlichViewModelsLib.Enums;

	/// <summary>An Interface that is used to message document property changes back to root of a document viewmodel.</summary>
	internal interface IDiffSideViewModelParent
	{
		/// <summary>The parent viewmodel supports this callback method to inform the parent that the IsDirty property has changed its value.</summary>
		/// <param name="source"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		void IsDirtyChangedCallback(ViewSource source, bool oldValue, bool newValue);
	}
}
