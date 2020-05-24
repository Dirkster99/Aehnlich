namespace AehnlichLib.Files
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;

	public class FileContentInfo
	{
		public FileContentInfo(bool isBinary, IList<string> lines)
			: this()
		{
			TextContent = null;
			Lines = lines;
		}

		public FileContentInfo()
		{
			IsBinary = false;
			TextEncoding = Encoding.Default;
			TextContent = string.Empty;
			Lines = new List<string>();
		}

		public Encoding TextEncoding { get; set; }

		public string TextContent { get; set; }

		public IList<string> Lines { get; set; }

		public bool IsBinary { get; }
	}

	/// <summary>
	/// File Utility Class
	/// https://stackoverflow.com/questions/13167934/how-to-async-files-readalllines-and-await-for-results
	/// </summary>
	public static class FileEx
	{
		/// <summary>
		/// This is the same default buffer size as
		/// <see cref="StreamReader"/> and <see cref="FileStream"/>.
		/// </summary>
		private const int DefaultBufferSize = 4096;

		/// <summary>
		/// Indicates that
		/// 1. The file is to be used for asynchronous reading.
		/// 2. The file is to be accessed sequentially from beginning to end.
		/// </summary>
		private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

		/// <summary>
		/// Reads the  text content of a file and determines its Encoding.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static async Task<FileContentInfo> GetFileTextAsync(string path)
		{
			var contentInfo = new FileContentInfo();

//// This should be created from in-memory text to save IO and support editing
////			// Open the FileStream with the same FileMode, FileAccess
////			// and FileShare as a call to File.OpenText would've done.
////			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
////			{
////				var bom = new byte[4];               // Decode bom (if any) and continue to read text content
////				await stream.ReadAsync(bom, 0, 4);
////				stream.Seek(0, SeekOrigin.Begin);
////
////				contentInfo.TextEncoding = GetEncoding(bom);
////
////				using (var reader = new StreamReader(stream, contentInfo.TextEncoding))
////				{
////					string line;
////					while ((line = await reader.ReadLineAsync()) != null)
////					{
////						contentInfo.Lines.Add(line);
////					}
////				}
////			}

			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
			{
				var bom = new byte[4];               // Decode bom (if any) and continue to read text content
				await stream.ReadAsync(bom, 0, 4);
				stream.Seek(0, SeekOrigin.Begin);

				contentInfo.TextEncoding = GetEncoding(bom);

				using (StreamReader reader = new StreamReader(stream, contentInfo.TextEncoding))
				{
					contentInfo.TextContent = reader.ReadToEnd();
				}
			}

			return contentInfo;
		}

		public static async Task<List<string>> GetFileTextLinesAsync(string path)
		{
			var lines = new List<string>();

			// Open the FileStream with the same FileMode, FileAccess
			// and FileShare as a call to File.OpenText would've done.
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
			{
				var bom = new byte[4];               // Decode bom (if any) and continue to read text content
				await stream.ReadAsync(bom, 0, 4);
				stream.Seek(0, SeekOrigin.Begin);
				Encoding encoding = GetEncoding(bom);

				using (var reader = new StreamReader(stream, encoding))
				{
					string line;
					while ((line = await reader.ReadLineAsync()) != null)
					{
						lines.Add(line);
					}
				}
			}

			return lines;
		}

		/// <summary>
		/// Gets the encoding of a file from its first 4 bytes.
		/// </summary>
		/// <param name="bom">BOM to be translated into an <see cref="Encoding"/>.
		/// This should be at least 4 bytes long.</param>
		/// <returns>Recommended <see cref="Encoding"/> to be used to read text from this file.</returns>
		public static Encoding GetEncoding(byte[] bom)
		{
			// Analyze the BOM
			if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
				return Encoding.UTF7;

			if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
				return Encoding.UTF8;

			if (bom[0] == 0xff && bom[1] == 0xfe)
				return Encoding.Unicode; //UTF-16LE

			if (bom[0] == 0xfe && bom[1] == 0xff)
				return Encoding.BigEndianUnicode; //UTF-16BE

			if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
				return Encoding.UTF32;

			return Encoding.Default;
		}
	}
}
