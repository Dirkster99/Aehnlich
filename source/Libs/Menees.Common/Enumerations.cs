namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	#region Public ValidPathOptions

	/// <summary>
	/// The supported options for <see cref="FileUtility.IsValidPath"/>.
	/// </summary>
	[Flags]
	public enum ValidPathOptions
	{
		/// <summary>
		/// Use the default behavior with no options.
		/// </summary>
		None = 0,

		/// <summary>
		/// Whether a path is allowed to end with a separator (e.g., if the caller knows they're validating a directory path).
		/// </summary>
		AllowTrailingSeparator = 1,

		/// <summary>
		/// Whether paths can contain parts equal to "." or ".." and can omit a root (e.g., Drive: or \\Server\Share).
		/// </summary>
		AllowRelative = 2,

		/// <summary>
		/// Whether paths over MAX_PATH are allowed if prefixed with \\?\.
		/// </summary>
		/// <remarks>
		/// When a long path is used, then '/' can't be used as a part separator, and the path can't be relative.
		/// </remarks>
		AllowLongPaths = 4,

		/// <summary>
		/// Whether a prefix of "\\.\" can be used to reference device paths (e.g., \\.\PhysicalDisk1)
		/// and whether drive names (e.g., C:) and UNC paths without a share can be used (e.g., \\Server).
		/// </summary>
		AllowDevicePaths = 8,
	}

	#endregion
}
