namespace Menees
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// Methods for working with <see cref="Uri"/> instances.
	/// </summary>
	public static class UriUtility
	{
		#region Public Methods

		/// <summary>
		/// Clones the URI and then appends a file or sub-folder name to the new URI's Path.
		/// </summary>
		/// <param name="uri">The URI to append to.</param>
		/// <param name="newPathPart">A new file or folder name to append.</param>
		/// <returns>A new Uri if a path part is appended, or the original Uri if <paramref name="newPathPart"/> is null or empty.</returns>
		public static Uri AppendToPath(Uri uri, string newPathPart)
		{
			Uri result = uri;

			if (!string.IsNullOrEmpty(newPathPart))
			{
				UriBuilder builder = new UriBuilder(uri);

				string path = builder.Path;
				if (string.IsNullOrEmpty(path))
				{
					builder.Path = newPathPart;
				}
				else
				{
					// We'll do our own relative path combining instead of relying on the Uri(Uri, Uri) constructor.
					// It assumes that a path without a final slash or backslash is a file name, but we always want
					// the path to be treated as a folder at this point.
					//
					// Note: For file URIs, the Uri class will normalize '\' into '/', but we still have to be careful here
					// to not append a '\' immediately after a '/' (e.g., "file:///C:/" + "\Dev\Data" needs to become
					// "file:///C:/Dev/Data".
					const string Separator = "/";
					bool pathEndsWithSeparator = path.EndsWith(Separator);
					bool newPathPartStartsWithSeparator = newPathPart.StartsWith(Separator) ||
						(builder.Scheme == Uri.UriSchemeFile && newPathPart.StartsWith(@"\"));
					if (pathEndsWithSeparator && newPathPartStartsWithSeparator)
					{
						// Leave out a separator rather than have path//part.
						builder.Path += newPathPart.Substring(1);
					}
					else if (pathEndsWithSeparator || newPathPartStartsWithSeparator)
					{
						// Only one of the two has a separator, so we can just combine them.
						builder.Path += newPathPart;
					}
					else
					{
						// Neither had a separator, so we need to add it.
						builder.Path += Separator + newPathPart;
					}
				}

				result = builder.Uri;
			}

			return result;
		}

		#endregion
	}
}
