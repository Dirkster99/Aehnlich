namespace AehnlichLib.Text
{
	using AehnlichLib.Files;
	using AehnlichLib.Interfaces;
	using AehnlichLib.Models;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Xml;

	public enum FileType
	{
		Unknown,
		NotExisting,
		Binary,
		Text,
		Xml
	}

	internal class FileCompInfo
	{
		#region Fields
		private bool? _fileExists;
		private FileType _typeOfFile;
		#endregion Fields

		#region Ctors
		public FileCompInfo(string fileNamePath)
			: this()
		{
			this.FileNamePath = fileNamePath;
		}

		protected FileCompInfo()
		{

		}
		#endregion Ctors

		#region properties
		public string FileNamePath { get; }

		public bool FileExists
		{
			get
			{
				if (_fileExists == null)
				{
					try
					{
						_fileExists = System.IO.File.Exists(FileNamePath);
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
					if (IsBinaryFile(FileNamePath))
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

			using (FileStream stream = File.OpenRead(FileNamePath))
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

		public T TryGetXmlText<T>(
			Func<string, bool, IDiffProgress, T> converter,
			bool throwOnError,
			TextBinaryDiffArgs args,
			IDiffProgress progress)
		{
			T result = default(T);
			try
			{
				if (FileExists)
				{
					result = converter(FileNamePath, args.IgnoreXmlWhitespace, progress);
					_typeOfFile = FileType.Xml;
				}
			}
			catch (XmlException ex)
			{
				if (throwOnError)
				{
					StringBuilder sb = new StringBuilder("An XML comparison was attempted, but an XML exception occurred while parsing ");
					sb.Append("'" + FileNamePath + "'").AppendLine(".").AppendLine();
					sb.AppendLine("Exception Message:").Append(ex.Message);

					throw new XmlException(sb.ToString(), ex);
				}
			}

			return result;
		}
		#endregion methods
	}
}