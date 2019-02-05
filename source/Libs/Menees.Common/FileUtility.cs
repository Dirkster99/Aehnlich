namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods for file and file name processing.
	/// </summary>
	public static class FileUtility
	{
		#region Private Data Members

		// The GetInvalidPathChars set should be a subset of the GetInvalidFileNameChars set, but this combines them for safety.
		private static readonly HashSet<char> InvalidNameCharacters = new HashSet<char>(
			Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()));

		private static readonly HashSet<string> ReservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"AUX", "CLOCK$", "CON", "NUL", "PRN",
				"COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
				"LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
			};

		private static readonly char[] NormalPathSeparators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
		private static readonly char[] LongPathSeparators = new[] { Path.DirectorySeparatorChar };

		#endregion

		#region Public Methods

		/// <summary>
		/// Tries to delete the specified file.
		/// </summary>
		/// <remarks>
		/// This method differs from .NET's System.IO.File.Delete method because
		/// this method will not raise an exception on failure.  This method simply
		/// returns false on failure.  If you need more information about the reason of
		/// the failure you can call <see cref="TryDeleteFile(string, out int)"/> or
		/// <see cref="Marshal.GetLastWin32Error"/>.
		/// </remarks>
		/// <param name="fileName">The name of the file to be deleted.</param>
		/// <returns>True if successful.  False otherwise.</returns>
		public static bool TryDeleteFile(string fileName)
		{
			Conditions.RequireString(fileName, "fileName");
			bool result = NativeMethods.DeleteFile(fileName);
			return result;
		}

		/// <summary>
		/// Tries to delete the specified file and returns a Win32 error code if it is unsuccessful.
		/// </summary>
		/// <remarks>
		/// This method differs from .NET's System.IO.File.Delete method because
		/// this method will not raise an exception on failure.  This method simply
		/// returns false on failure and will return the error code from
		/// <see cref="Marshal.GetLastWin32Error"/> as an out parameter. Then
		/// you can throw a new Win32Exception if you need to.
		/// </remarks>
		/// <param name="fileName">The name of the file to be deleted.</param>
		/// <param name="errorCode">The Win32 error code for deletion failure.</param>
		/// <returns>True if successful.  False otherwise.</returns>
		public static bool TryDeleteFile(string fileName, out int errorCode)
		{
			Conditions.RequireString(fileName, "fileName");

			errorCode = 0;
			bool result = NativeMethods.DeleteFile(fileName);

			if (!result)
			{
				errorCode = Marshal.GetLastWin32Error();
			}

			return result;
		}

		/// <summary>
		/// Deletes the specified file.
		/// </summary>
		/// <remarks>
		/// This will not raise an exception for ERROR_FILE_NOT_FOUND, ERROR_PATH_NOT_FOUND,
		/// or ERROR_INVALID_DRIVE errors.  All other Win32 error codes will raise a <see cref="Win32Exception"/>.
		/// </remarks>
		/// <param name="fileName">The name of the file to delete.</param>
		public static void DeleteFile(string fileName)
		{
			int errorCode;
			if (!TryDeleteFile(fileName, out errorCode))
			{
				// We'll ignore errors for file, path, or drive not found.
				// .NET's File.Delete method only ignores ERROR_FILE_NOT_FOUND.
				// http://msdn.microsoft.com/en-us/library/windows/desktop/ms681382(v=vs.85).aspx
				const int ERROR_FILE_NOT_FOUND = 2;
				const int ERROR_PATH_NOT_FOUND = 3;
				const int ERROR_INVALID_DRIVE = 15;

				if (errorCode != ERROR_FILE_NOT_FOUND &&
					errorCode != ERROR_PATH_NOT_FOUND &&
					errorCode != ERROR_INVALID_DRIVE)
				{
					// There was some other problem deleting the file (e.g., sharing violation, read-only attribute, security).
					throw Exceptions.Log(new Win32Exception(errorCode));
				}
			}
		}

		/// <summary>
		/// Expands a file name to include a full path, expanding environment variables
		/// and using the <see cref="ApplicationInfo"/>'s <see cref="ApplicationInfo.BaseDirectory"/>
		/// if no directory is specified.
		/// </summary>
		/// <param name="fileName">The file name to expand.</param>
		/// <returns>A full file name.</returns>
		public static string ExpandFileName(string fileName)
		{
			Conditions.RequireString(fileName, "fileName");

			// Expand environment variables first in case the variables add the directory.
			string result = Environment.ExpandEnvironmentVariables(fileName);

			// If the result doesn't have a directory, then add on the application's
			// base directory.  This is a much better alternative than depending on
			// Path.GetFullPath's logic of appending the current directory, which
			// isn't predictable in a multi-threaded process (since there's only one
			// current directory per process and any thread can change it).
			string directory = Path.GetDirectoryName(result);
			if (string.IsNullOrEmpty(directory))
			{
				result = Path.Combine(ApplicationInfo.BaseDirectory, result);
			}

			// Run it through GetFullPath to convert any short file names into long names.
			// This is important because sometimes system environment variables like %TEMP%
			// will be set to short paths like "C:\Windows\SERVIC~2\NETWOR~1\AppData\Local\Temp".
			// Calling GetFullPath will expand them into a long path like
			// "C:\Windows\ServiceProfiles\NetworkService\AppData\Local\Temp".
			// Path.GetTempPath does this internally, which is why it always returns long paths.
			result = Path.GetFullPath(result);
			return result;
		}

		/// <summary>
		/// Creates a unique file name using the specified extension and the system's temporary directory.
		/// </summary>
		/// <param name="extension">An extention to add to the file name.  This can be null or empty.</param>
		/// <returns>A new temporary file name using the system's temporary directory.</returns>
		public static string GetTempFileName(string extension)
		{
			string result = GetTempFileName(extension, Path.GetTempPath());
			return result;
		}

		/// <summary>
		/// Creates a unique file name using the specified extension and directory.
		/// </summary>
		/// <param name="extension">An extention to add to the file name.  This can be null or empty.</param>
		/// <param name="directory">The directory to create the file in.
		/// If this is null or empty, then a new file name with no path will be returned.</param>
		/// <returns>A new temporary file name.</returns>
		public static string GetTempFileName(string extension, string directory)
		{
			// This method differs from the System.IO.Path.GetTempFileName() method because it:
			//   (a) lets the caller specify the extension and directory and
			//   (b) doesn't create a zero byte temporary file.
			// This is possible because a new Guid is used to ensure the file name is unique.
			//
			// Note: I'm not using Win32's GetTempFileName method because it has a lot of
			// limitations such as requiring a zero byte file to be created, not allowing the
			// extension to be specified (just a prefix!), and only allowing 65535 temporary
			// names before a collision occurs.
			string uniqueId = Guid.NewGuid().ToString("N");

			// ChangeExtension will check for whether extension contains a period.
			string filename = Path.ChangeExtension(uniqueId, extension);
			string result = Path.Combine(directory, filename);
			return result;
		}

		/// <summary>
		/// Gets whether the file is read-only.
		/// </summary>
		/// <remarks>
		/// This method only checks the read-only attribute.  It does not check security
		/// or whether another user has an open handle to the file that would prevent
		/// writes.
		/// </remarks>
		/// <param name="fileName">The file to check.</param>
		/// <returns>True if the file's read-only attribute is set.  False otherwise.</returns>
		public static bool IsReadOnlyFile(string fileName)
		{
			Conditions.RequireString(fileName, "fileName");
			FileAttributes attr = File.GetAttributes(fileName);
			return (attr & FileAttributes.ReadOnly) != 0;
		}

		/// <summary>
		/// Gets the exact case used on the file system for an existing file or directory.
		/// </summary>
		/// <param name="path">A relative or absolute path.</param>
		/// <param name="exactPath">The full path using the correct case if the path exists.  Otherwise, null.</param>
		/// <returns>True if the exact path was found.  False otherwise.</returns>
		/// <remarks>
		/// This supports drive-lettered paths and UNC paths, but a UNC root
		/// will be returned in title case (e.g., \\Server\Share).
		/// </remarks>
		public static bool TryGetExactPath(string path, out string exactPath)
		{
			Conditions.RequireString(path, () => path);

			bool result = false;
			exactPath = null;

			// http://stackoverflow.com/questions/325931/getting-actual-file-name-with-proper-casing-on-windows-with-net
			// DirectoryInfo accepts either a file path or a directory path, and most of its properties work for either.
			// However, its Exists property only works for a directory path.
			DirectoryInfo directory = new DirectoryInfo(path);
			if (File.Exists(path) || directory.Exists)
			{
				List<string> parts = new List<string>();

				DirectoryInfo parentDirectory = directory.Parent;
				while (parentDirectory != null)
				{
					FileSystemInfo entry = parentDirectory.EnumerateFileSystemInfos(directory.Name).First();
					parts.Add(entry.Name);

					directory = parentDirectory;
					parentDirectory = directory.Parent;
				}

				// Handle the root part (i.e., drive letter or UNC \\server\share).
				string root = directory.FullName;
				if (root.Contains(Path.VolumeSeparatorChar))
				{
					root = root.ToUpper();
				}
				else
				{
					string[] rootParts = root.Split(NormalPathSeparators);
					root = string.Join(@"\", rootParts.Select(part => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(part)));
				}

				parts.Add(root);
				parts.Reverse();
				exactPath = Path.Combine(parts.ToArray());
				result = true;
			}

			return result;
		}

		/// <summary>
		/// Gets whether the specified name is valid as a file name, folder name, or device name.
		/// </summary>
		/// <param name="pathPartName">The name of a file, folder, or device to check.</param>
		/// <returns>True if the name is valid.  False otherwise.</returns>
		/// <remarks>
		/// This doesn't check existence.  It just checks for valid names that don't match Windows
		/// reserved names (e.g., AUX, CON), don't match reserved names plus an extension (e.g., CON.txt),
		/// and don't consist of all dots (e.g., "." and "..").
		/// </remarks>
		public static bool IsValidName(string pathPartName)
		{
			// Naming Files, Paths, and Namespaces - https://msdn.microsoft.com/en-us/library/aa365247.aspx
			bool result = !string.IsNullOrEmpty(pathPartName)
				&& !pathPartName.Any(ch => InvalidNameCharacters.Contains(ch))
				&& !pathPartName.All(ch => ch == '.')
				&& !ReservedNames.Contains(pathPartName)
				&& !ReservedNames.Any(name => pathPartName.StartsWith(name, StringComparison.OrdinalIgnoreCase) && pathPartName[name.Length] == '.');
			return result;
		}

		/// <summary>
		/// Gets whether the specified path is valid but does not check its existence.
		/// </summary>
		/// <param name="path">The path to validate.</param>
		/// <param name="options">Options that affect how the path is validated.</param>
		/// <returns>True if the path is valid.  False otherwise.</returns>
		public static bool IsValidPath(string path, ValidPathOptions options)
		{
			bool result = false;

			// Naming Files, Paths, and Namespaces - https://msdn.microsoft.com/en-us/library/aa365247.aspx
			if (!string.IsNullOrEmpty(path))
			{
				string remainingPath = path;
				char[] separators;
				int maxPath;
				bool allowRelative;
				bool allowDevicePaths;
				const string LongPathPrefix = @"\\?\";
				const string DevicePathPrefix = @"\\.\";
				if (options.HasFlag(ValidPathOptions.AllowLongPaths) && path.StartsWith(LongPathPrefix))
				{
					separators = LongPathSeparators;
					const int LongMaxPath = 32767;
					maxPath = LongMaxPath;
					allowRelative = false;
					allowDevicePaths = false;
					remainingPath = path.Substring(LongPathPrefix.Length);
				}
				else
				{
					separators = NormalPathSeparators;
					const int NormalMaxPath = 260;
					maxPath = NormalMaxPath;
					allowDevicePaths = options.HasFlag(ValidPathOptions.AllowDevicePaths);
					if (allowDevicePaths && path.StartsWith(DevicePathPrefix))
					{
						// Once the device prefix is removed, then only a relative path will remain (e.g., \\.\Disk1 --> Disk1).
						allowRelative = true;
						remainingPath = path.Substring(DevicePathPrefix.Length);
					}
					else
					{
						allowRelative = options.HasFlag(ValidPathOptions.AllowRelative);
					}
				}

				if (path.Length <= maxPath && remainingPath.Length > 0
					&& (options.HasFlag(ValidPathOptions.AllowTrailingSeparator) || !separators.Contains(path[path.Length - 1])))
				{
					string root;
					if (HasValidRoot(remainingPath, separators, allowDevicePaths, allowRelative, out root))
					{
						if (root.Length == remainingPath.Length)
						{
							result = true;
						}
						else
						{
							remainingPath = remainingPath.Substring(root.Length);
							result = IsValidUnrootedPath(remainingPath, separators, allowRelative);
						}
					}
					else if (allowRelative)
					{
						result = IsValidUnrootedPath(remainingPath, separators, allowRelative);
					}
				}
			}

			return result;
		}

		#endregion

		#region Private Methods

		private static bool HasValidRoot(string path, char[] separators, bool allowDeviceNameOnly, bool allowRelative, out string root)
		{
			root = null;

			const int DriveColonLength = 2;
			const int DriveColonSeparatorLength = DriveColonLength + 1;
			if (path.Length >= DriveColonLength)
			{
				// Check for X: format.
				if (char.IsLetter(path[0]) && path[1] == Path.VolumeSeparatorChar)
				{
					// See if there's a separator after the X: drive or if device names are allowed.
					if (path.Length >= DriveColonSeparatorLength && separators.Contains(path[DriveColonSeparatorLength - 1]))
					{
						root = path.Substring(0, DriveColonSeparatorLength);
					}
					else if (allowDeviceNameOnly && path.Length == DriveColonLength)
					{
						root = path;
					}
					else if (allowRelative && path.Length >= DriveColonSeparatorLength)
					{
						// Windows allows drives to be specified for relative paths like C:..\tmp.txt
						// per https://msdn.microsoft.com/en-us/library/aa365247.aspx.
						root = path.Substring(0, DriveColonSeparatorLength - 1);
					}
				}
				else if (path[0] == path[1] && separators.Contains(path[0]))
				{
					// Make sure something else comes after the \\ or // prefix.
					const int UncPrefixLength = 2;
					if (path.Length >= (UncPrefixLength + 1))
					{
						int shareStartSeparatorIndex = path.IndexOfAny(separators, UncPrefixLength);
						if (shareStartSeparatorIndex < 0 && allowDeviceNameOnly && IsValidName(path.Substring(UncPrefixLength)))
						{
							root = path;
						}
						else
						{
							const int MinServerShareLength = UncPrefixLength + 1 + 1 + 1; // Must be at least \\x\y
							if (shareStartSeparatorIndex >= (UncPrefixLength + 1) && path.Length >= MinServerShareLength)
							{
								// Make sure we don't have \\xx\
								int shareNameStartIndex = shareStartSeparatorIndex + 1;
								if (shareNameStartIndex < path.Length)
								{
									string serverName = path.Substring(UncPrefixLength, shareStartSeparatorIndex - UncPrefixLength);
									if (IsValidName(serverName))
									{
										// Note: Windows doesn't allow duplicate separators between Server and Share
										// (e.g., \\Server\\Share) like it does in other places (e.g., see IsValidUnrootedPath).
										int shareEndSeparatorIndex = path.IndexOfAny(separators, shareNameStartIndex);
										int shareNameLength = (shareEndSeparatorIndex < 0 ? path.Length : shareEndSeparatorIndex) - shareNameStartIndex;
										string shareName = path.Substring(shareNameStartIndex, shareNameLength);
										if (IsValidName(shareName))
										{
											int rootLength = shareNameStartIndex + shareNameLength;
											if (shareEndSeparatorIndex >= 0)
											{
												// Include the separator after the \\Server\Share root just like we do for
												// drive-based roots.  This makes things easier on the caller because it
												// can just parse the relative parts after the root without having to worry
												// about removing a separator first.
												rootLength++;
											}

											root = path.Substring(0, rootLength);
										}
									}
								}
							}
						}
					}
				}
			}

			bool result = !string.IsNullOrEmpty(root);
			return result;
		}

		private static bool IsValidUnrootedPath(string path, char[] separators, bool allowRelativeParts)
		{
			string[] parts = path.Split(separators);

			// Windows and Unix allow multiple separators to be collapsed to a single separator
			// in unrooted paths (X\\Y --> X\Y), so we'll always allow empty parts here.
			bool result = parts.All(part => string.IsNullOrEmpty(part) || IsValidName(part)
					|| (allowRelativeParts && (part == "." || part == "..")));

			return result;
		}

		#endregion
	}
}
