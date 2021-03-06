namespace AehnlichLib_UnitTests.Mock_IFileSystemInfo
{
	using FsDataLib.Interfaces.Dir;
	using System;

	/// <summary>Implement a mock-up class for usage with UNIT TESTING.</summary>
	internal class FileSystemInfoImpl : IFileSystemInfo
	{
		#region fields
		private string _path;
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="path"></param>
		public FileSystemInfoImpl(string path)
			: this()
		{
			_path = path;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		protected FileSystemInfoImpl()
		{
		}
		#endregion ctors

		#region properties

		public string FullName => _path;

		public string Name => throw new NotImplementedException();

		public bool DirectoryExists => throw new NotImplementedException();

		public bool FileExists => throw new NotImplementedException();

		public DateTime LastWriteTime => throw new NotImplementedException();
		#endregion properties

		#region methods
		#endregion methods
	}
}
