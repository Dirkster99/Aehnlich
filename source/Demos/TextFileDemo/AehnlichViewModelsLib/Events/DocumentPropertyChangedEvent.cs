namespace AehnlichViewModelsLib.Events
{
	using AehnlichViewModelsLib.Enums;
	using System;

	/// <summary>Indicates the type of change when comunicating document property changes in a raised event.</summary>
	public enum DocumentPropertyChangeType
	{
		/// <summary>The IsDirty property of the document has changed.</summary>
		IsDirty,

		/// <summary>The document name has changed.</summary>
		Name
	}


	/// <summary>Implements an event to indicate a change in a document property (name, IsDirty).</summary>
	public class DocumentPropertyChangedEvent : EventArgs
	{
		#region ctors
		/// <summary>class constructor</summary>
		/// <param name="source"></param>
		/// <param name="isDirty"></param>
		/// <param name="filePath"></param>
		/// <param name="changeType"></param>
		public DocumentPropertyChangedEvent(ViewSource source
		                                  , bool isDirty
		                                  , string filePath
		                                  , DocumentPropertyChangeType changeType)
			:this()
		{
			Source = source;
			ChangeType = changeType;
			IsDirty = isDirty;
			FilePath = filePath;
		}

		/// <summary>hidden class constructor</summary>
		protected DocumentPropertyChangedEvent()
		{
		}
		#endregion ctors

		/// <summary>Gets the source document that has a property change.</summary>
		public ViewSource Source { get; }

		/// <summary>Gets the type of document property change indicated in this.</summary>
		public DocumentPropertyChangeType ChangeType { get; }

		/// <summary>Gets the name that is available when the event was raised.</summary>
		public string FilePath { get; }

		/// <summary>Gets whether the document was changed without saving to file system, or not, when the event was raised.</summary>
		public bool IsDirty { get; }
	}
}
