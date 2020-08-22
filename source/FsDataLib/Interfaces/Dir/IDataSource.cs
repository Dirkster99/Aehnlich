namespace FsDataLib.Interfaces.Dir
{
	/// <summary>
	/// Provides routines and objects for working with data objects that refers to directories and files.
	/// </summary>
	public interface IDataSource
	{
		#region Members
		/// <summary>
		/// Gets a normalized path to a directory if it exists or null.
		/// </summary>
		/// <param name="leftDir"></param>
		/// <returns></returns>
		string GetPathIfDirExists(string leftDir);

		/// <summary>
		/// Gets a normalized path to a directory
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		string NormalizePath(string path);

		/// <summary>
		/// Creates an <see cref="IDirectoryInfo"/> object from the given path and returns it.
		/// </summary>
		/// <param name="path">The path to create a directory data source object for.</param>
		/// <returns>
		/// The created <see cref="IDirectoryInfo"/> object.
		/// </returns>
		IDirectoryInfo CreateDirectory(string path);

		/// <summary>
		/// Creates an <see cref="IFileInfo"/> object from the given path and returns it.
		/// </summary>
		/// <param name="path">The path to create a file data source object for.</param>
		/// <returns>
		/// The created <see cref="IFileInfo"/> object.
		/// </returns>
		IFileInfo CreateFile(string path);

		/// <summary>
		/// Combines two strings into a path.
		/// </summary>
		/// <param name="a">The first path to combine.</param>
		/// <param name="b">The second path to combine.</param>
		/// <returns>
		/// The combined paths. If one of the specified paths is a zero-length string, this
		/// method returns the other path. If path2 contains an absolute path, this method
		/// returns path2.
		/// </returns>
		string Combine(string a, string b);

		/// <summary>
		/// Returns false if both files are equal and true if they differ
		/// (based on a byte size comparison or byte by byte comparison).
		/// </summary>
		/// <param name="info1"></param>
		/// <param name="info2"></param>
		/// <returns></returns>
		bool AreBinaryFilesDifferent(string fileName1, string fileName2);

		/// <summary>
		/// Returns false if both files are equal and true if they differ
		/// (based on a byte size comparison or byte by byte comparison).
		/// </summary>
		/// <param name="info1"></param>
		/// <param name="info2"></param>
		/// <returns></returns>
		bool AreBinaryFilesDifferent(IFileInfo info1, IFileInfo info2);
		#endregion Members
	}
}
