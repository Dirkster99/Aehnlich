namespace FsDataLib.Dir
{
	using FsDataLib.Interfaces.Dir;
	using System.IO;
	using System.Text;

	internal class FileInfoImpl : FileSystemInfoImpl, IFileInfo
	{
		#region fields
		private FileType _typeOfFile;
		private bool? _fileExists;
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		public FileInfoImpl(string path)
			: base(path)
		{
		}

		/// <summary>
		/// Hidden class constructor
		/// </summary>
		private FileInfoImpl()
		{
		}
		#endregion ctors

		#region properties
		/// <summary>
		/// Gets the size, in bytes, of the current file.
		/// </summary>
		public long Length
		{
			get
			{
				var info = new System.IO.FileInfo(_path);
				return info.Length;
			}
		}

		/// <summary>Gets a value indicating whether this FILE exists.</summary>
		/// <returns>true if the FILE exists; otherwise, false.</returns>
		public bool FileExists
		{
			get
			{
				if (_fileExists == null)
				{
					try
					{
						_fileExists = System.IO.File.Exists(_path);
					}
					catch
					{
						_fileExists = false;
					}
				}

				return (bool)_fileExists;
			}
		}

		public FileType Is
		{
			get
			{
				if (_typeOfFile != FileType.Unknown)
					return _typeOfFile;

				if (FileExists)
				{
					if (IsBinaryFile(FullName))
					{
						_typeOfFile = FileType.Binary;
						return _typeOfFile;
					}

					_typeOfFile = FileType.Text;
					return _typeOfFile;
				}

				_typeOfFile = FileType.NotExisting;
				return _typeOfFile;
			}
		}
		#endregion properties

		#region methods
		public bool IsBinaryFile(string fileName)
		{
			if (FileExists == false)
				return false;

			using (FileStream stream = File.OpenRead(FullName))
			{
				return IsBinaryFile(stream);
			}
		}

		public static bool IsBinaryFile(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			// First see if the file begins with any known Unicode byte order marks.
			// If so, then it is a text file.  Use a StreamReader instance to do the
			// auto-detection logic.
			//
			// NOTE: I'm not disposing of the StreamReader because that closes the
			// associated Stream.  The caller opened the file stream, so they should
			// be the one to close it.
			StreamReader reader = new StreamReader(stream, Encoding.Default, true, 1024, true);
			reader.Read(); // We have to force a Read for it to auto-detect.
			if (reader.CurrentEncoding != Encoding.Default)
			{
				return false;
			}

			reader.DiscardBufferedData();
			reader = null;

			// Since the type was Default encoding, that means there were no byte-order
			// marks to indicate its type.  So we have to scan.  If we find a NULL
			// character in the stream, that means it's a binary file.
			stream.Seek(0, SeekOrigin.Begin);
			int i;
			while ((i = stream.ReadByte()) > -1)
			{
				if (i == 0)
				{
					return true;
				}
			}

			return false;
		}
		#endregion methods
	}
}
