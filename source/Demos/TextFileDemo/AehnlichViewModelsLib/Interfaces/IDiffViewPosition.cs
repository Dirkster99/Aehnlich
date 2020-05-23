namespace AehnlichViewModelsLib.ViewModels
{
	using System;
	using System.ComponentModel;

	/// <summary>
	/// Defines the public properties and methods of the diff view position object
	/// which can be used to locate a location in text (via columns and lines).
	/// </summary>
	public interface IDiffViewPosition : IEquatable<IDiffViewPosition>, INotifyPropertyChanged
	{
		#region Properties
		/// <summary>
		/// Gets/sets the column of a display position.
		/// </summary>
		int Column { get; set; }

		/// <summary>
		/// Gets/sets the line of a display position.
		/// </summary>
		int Line { get; set; }
		#endregion Properties

		#region Methods
		/// <summary>
		/// Gets a number equal zero if both positions refer to the same location.
		/// Returns otherwise a non-zero integer value.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		int CompareTo(IDiffViewPosition position);

		/// <summary>
		/// Gets whether a the positions is equal to this position or not.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		bool Equals(object obj);

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		int GetHashCode();
		#endregion Methods
	}
}