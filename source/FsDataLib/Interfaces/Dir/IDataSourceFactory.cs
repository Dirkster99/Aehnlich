namespace FsDataLib.Interfaces.Dir
{
	/// <summary>
	/// Provides factory routines to create data source objects
	/// that implement <see cref="IDataSource"/>
	/// on Windows file system objects.
	/// </summary>
	public interface IDataSourceFactory
	{
		/// <summary>
		/// Creates a new <see cref="IDataSource"/> provider and returns it.
		/// </summary>
		IDataSource CreateDataSource();
	}
}
