namespace AehnlichDirViewModelLib.Converters
{
    // Copyright © Microsoft Corporation.  All Rights Reserved.
    // This code released under the terms of the 
    // Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
    // 
    // Based on this source:
    // https://github.com/adamdriscoll/TfsIntegrationPlatform/blob/master/IntegrationPlatform/Shell/EditorFoundation/Source/View.Wpf/Converters/AssociatedIconConverter.cs
    //
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Indicates the size of the icon to retrieve.
    /// </summary>
    public enum IconSize : uint
    {
        /// <summary>
        /// A 16x16 icon.
        /// </summary>
        Small = AssociatedIconConverter.SHIL.SMALL,

        /// <summary>
        /// A 32x32 icon.
        /// </summary>
        Medium = AssociatedIconConverter.SHIL.LARGE,

        /// <summary>
        /// A 48x48 icon.
        /// </summary>
        Large = AssociatedIconConverter.SHIL.EXTRALARGE,

        /// <summary>
        /// A 256x256 icon.
        /// </summary>
        ExtraLarge = AssociatedIconConverter.SHIL.JUMBO
    }

    /// <summary>
    /// Represents a converter that converts a file path to an icon that is associated with that file.
    /// </summary>
    public class AssociatedIconConverter : IMultiValueConverter
    {
        #region Fields
        private static readonly AssociatedIconConverter _default = new AssociatedIconConverter();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default <see cref="AssociatedIconConverter"/>.
        /// </summary>
        public static AssociatedIconConverter Default
        {
            get
            {
                return AssociatedIconConverter._default;
            }
        }

        private static IconSize DefaultIconSize
        {
            get
            {
                // ExtraLarge (256x256) icons are only supported on Vista and later
                if (Environment.OSVersion.Version.Major < 6)
                {
                    return IconSize.Large;
                }
                else
                {
                    return IconSize.ExtraLarge;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="values">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if (values.Length < 1)
                return Binding.DoNothing;

            string path = values[0] as string;
            string secPath = null;

            string iconResourceId = null;
            if (values.Length == 2)
            {
                iconResourceId = values[1] as string;
            }
            else
            {
                if (values.Length >= 3)
                {
                    secPath = values[1] as string;
                    iconResourceId = values[2] as string;
                }
            }

            if (string.IsNullOrEmpty(path) == true) // Try secondary path if available
                path = secPath;

            if (path == null)
                return Binding.DoNothing;

            IconSize iconSize = AssociatedIconConverter.DefaultIconSize;

            if (parameter is IconSize)
                iconSize = (IconSize)parameter;

            if (string.IsNullOrEmpty(path) == false)
            {
                try
                {
                    // The resource for loading this item's icon is known
                    // So, we attempt to extract and display it
                    if (iconResourceId != null)
                    {
                        string[] resourceId = iconResourceId.Split(',');
                        if (resourceId.Length == 2)
                        {
                            int iconIndex = int.Parse(resourceId[1]);

                            if (resourceId != null && resourceId.Length == 2)
                            {
                                if (string.IsNullOrEmpty(resourceId[0]) == false)
                                {
                                    if (IsReferenceToUnknownIcon(resourceId[0], iconIndex) == false)
                                        return Extract(resourceId[0], iconIndex, iconSize);
                                }
                                else
                                {
                                    Debug.WriteLine(string.Format("---> 0 Failed to get icon from '{0}' with '{1}'",
                                        path, iconResourceId));
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(string.Format("---> 1 Failed to get icon from {0}: {1}{2}",
                        path, Environment.NewLine, exception.ToString()));
                }

                try
                {
                    // The resource for loading this item's icon is unknown
                    // So, we attempt to determine the associated icon and load it
                    return GetIconFromPath(path, iconSize);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(string.Format("---> 2 Failed to get icon from {0}: {1}{2}", path, Environment.NewLine, exception.ToString()));
                }
            }

            return null;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetTypes">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets if the icon points to a real association or just a blank page
        /// saying: "We don't really know the association but here is an icon anyway..."
        /// https://www.win7dll.info/imageres_dll.html
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsReferenceToUnknownIcon(string filename, int index)
        {
            if (index == -3)
            {
                const string imgresdll = "imageres.dll";
                if (filename.Length > imgresdll.Length)
                {
                    int idx = filename.IndexOf(imgresdll);
                    if (idx > 0)
                    {
                        string match = filename.Substring(idx);

                        if (string.Compare(match, imgresdll, true) == 0)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Extracts an image from a dll resource (such as imageres.dll) and returns
        /// it as an ImageSource for usage on a WPF Image control.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="iconIndex"></param>
        /// <param name="iconSize"></param>
        /// <returns></returns>
        public ImageSource Extract(string file, int iconIndex, IconSize iconSize)
        {
            if (string.IsNullOrEmpty(file))
                return null;

            IntPtr large = default(IntPtr);
            IntPtr small = default(IntPtr);
            var hr = NativeMethods.ExtractIconEx(file, iconIndex, out large, out small, 1);
            try
            {
                return GetPngImage(iconSize != IconSize.Small ? large : small);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (large != default(IntPtr))
                    NativeMethods.DestroyIcon(large);

                if (small != default(IntPtr))
                    NativeMethods.DestroyIcon(small);
            }
        }

        /// <summary>
        /// Gets an associated icon for the file at the specified path.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="iconSize">The size of the icon to retrieve.</param>
        /// <returns>An image representing the icon.</returns>
        private ImageSource GetIconFromPath(string filePath, IconSize iconSize)
        {
            // Get an image list
            Guid iidImageList = new Guid(IID_IImageList);
            IntPtr imageListHandle = default(IntPtr), iconHandle = default(IntPtr);
            try
            {
                // Get a shell file info for the specified file path
                SHFILEINFO shellFileInfo = new SHFILEINFO();
                NativeMethods.SHGetFileInfo(filePath, 0, ref shellFileInfo, (uint)Marshal.SizeOf(shellFileInfo), (uint)SHGFI.SYSICONINDEX);

                int result = NativeMethods.SHGetImageList((int)iconSize, ref iidImageList, ref imageListHandle);
                if (result != 0)
                {
                    return null;
                    // throw new COMException("SHGetImageList failed.", result);
                }

                // Get the icon index
                int iconIndex = shellFileInfo.iIcon;

                // Get an icon handle
                iconHandle = NativeMethods.ImageList_GetIcon(imageListHandle, iconIndex, (uint)ILD.TRANSPARENT);
                if (iconHandle == IntPtr.Zero)
                {
                    return null;
                    // throw new Exception("ImageList_GetIcon failed.");
                }

                return GetPngImage(iconHandle);
            }
            finally
            {
                if (iconHandle != default(IntPtr))
                    NativeMethods.DestroyIcon(iconHandle);

                if (imageListHandle != default(IntPtr))
                    Marshal.Release(imageListHandle);
            }
        }

        /// <summary>
        /// Converts an HIcon https://msdn.microsoft.com/en-us/library/windows/desktop/ms646973(v=vs.85).aspx
        /// into an ImageSource for display in a WPF Image control.
        /// </summary>
        /// <param name="iconHandle"></param>
        /// <returns></returns>
        private static ImageSource GetPngImage(IntPtr iconHandle)
        {
            MemoryStream memoryStream = null;
            PngBitmapDecoder bitmapDecoder = null;

            // Write the icon (as a png) into a memory stream
            memoryStream = new MemoryStream();

            Icon.FromHandle(iconHandle).ToBitmap().Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Decode the icon
            bitmapDecoder = new PngBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.Default);

            if (bitmapDecoder == null || bitmapDecoder.Frames == null || bitmapDecoder.Frames.Count == 0)
            {
                return null;
                // throw new Exception("Failed to decode icon.");
            }

            // Return the icon
            return bitmapDecoder.Frames[0];
        }
        #endregion

        #region Native Constants
        private const string IID_IImageList = "46EB5926-582E-4017-9FDF-E8998DAA0950";

        internal enum SHIL : uint
        {
            LARGE,
            SMALL,
            EXTRALARGE,
            SYSSMALL,
            JUMBO,
            LAST
        }

        [Flags]
        private enum SHGFI : uint
        {
            ICON = 0x000000100,
            DISPLAYNAME = 0x000000200,
            TYPENAME = 0x000000400,
            ATTRIBUTES = 0x000000800,
            ICONLOCATION = 0x000001000,
            EXETYPE = 0x000002000,
            SYSICONINDEX = 0x000004000,
            LINKOVERLAY = 0x000008000,
            SELECTED = 0x000010000,
            ATTR_SPECIFIED = 0x000020000,
            LARGEICON = 0x000000000,
            SMALLICON = 0x000000001,
            OPENICON = 0x000000002,
            SHELLICONSIZE = 0x000000004,
            PIDL = 0x000000008,
            USEFILEATTRIBUTES = 0x000000010,
            ADDOVERLAYS = 0x000000020,
            OVERLAYINDEX = 0x000000040
        }

        [Flags]
        private enum ILD : uint
        {
            NORMAL = 0x00000000,
            TRANSPARENT = 0x00000001,
            BLEND25 = 0x00000002,
            SELECTED = 0x00000004,
            MASK = 0x00000010,
            IMAGE = 0x00000020,
            ROP = 0x00000040,
            OVERLAYMASK = 0x00000F00,
            PRESERVEALPHA = 0x00001000,
            SCALE = 0x00002000,
            DPISCALE = 0x00004000
        }
        #endregion

        #region Native Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        #endregion

        #region private classes
        private static class NativeMethods
        {
            /// <summary>
            /// Retrieves information about an object in the file system,
            /// such as a file, folder, directory, or drive root.
            /// </summary>
            /// <param name="pszPath">
            /// Type: LPCTSTR
            /// 
            /// A pointer to a null-terminated string of maximum length MAX_PATH that
            /// contains the path and file name. Both absolute and relative paths are valid.
            /// 
            /// If the uFlags parameter includes the SHGFI_PIDL flag, this parameter must be
            /// the address of an ITEMIDLIST (PIDL) structure that contains the list of item
            /// identifiers that uniquely identifies the file within the Shell's namespace.
            /// The PIDL must be a fully qualified PIDL. Relative PIDLs are not allowed.
            /// 
            /// If the uFlags parameter includes the SHGFI_USEFILEATTRIBUTES flag,
            /// this parameter does not have to be a valid file name. The function
            /// will proceed as if the file exists with the specified name and with
            /// the file attributes passed in the dwFileAttributes parameter.This
            /// allows you to obtain information about a file type by passing just
            /// the extension for pszPath and passing FILE_ATTRIBUTE_NORMAL in
            /// dwFileAttributes.
            /// 
            /// This string can use either short (the 8.3 form) or long file names.</param>
            /// <param name="dwFileAttributes">
            /// Type: DWORD
            /// 
            /// A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h).
            /// If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.
            /// </param>
            /// <param name="psfi">Type: SHFILEINFO*
            /// Pointer to a SHFILEINFO structure to receive the file information.</param>
            /// <param name="cbFileInfo">Type: UINT
            /// The size, in bytes, of the SHFILEINFO structure pointed to by the psfi parameter.</param>
            /// <param name="uFlags">Type: UINT
            /// The flags that specify the file information to retrieve.</param>
            /// <returns></returns>
            [DllImport("shell32", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SHGetFileInfo(
                [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
                int dwFileAttributes,
                ref SHFILEINFO psfi,
                uint cbFileInfo,
                uint uFlags);

            [DllImport("shell32")]
            internal extern static int SHGetImageList(
                int iImageList,
                ref Guid riid,
                ref IntPtr handle);

            [DllImport("comctl32")]
            internal extern static IntPtr ImageList_GetIcon(
                IntPtr himl,
                int i,
                uint flags);

            /// <summary>
            /// Destroys an icon and frees any memory the icon occupied.
            /// </summary>
            /// <param name="hIcon">Handle to the icon to be destroyed. The icon must not be in use. </param>
            /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DestroyIcon(IntPtr hIcon);

            /// <summary>
            /// Creates an array of handles to large or small icons extracted from the specified
            /// executable file, DLL, or icon file.
            /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms648069(v=vs.85).aspx
            /// </summary>
            /// <param name="sFile">Type: LPCTSTR
            /// 
            /// The name of an executable file, DLL, or icon file from which icons will be
            /// extracted.</param>
            /// <param name="iIndex">Type: int
            /// The zero-based index of the first icon to extract.
            /// For example, if this value is zero, the function extracts the first icon in
            /// the specified file.
            /// 
            /// If this value is –1 and phiconLarge and phiconSmall are both NULL,
            /// the function returns the total number of icons in the specified file.
            /// If the file is an executable file or DLL, the return value is the number
            /// of RT_GROUP_ICON resources.If the file is an.ico file, the return value is 1.
            /// 
            /// If this value is a negative number and either phiconLarge or phiconSmall
            /// is not NULL, the function begins by extracting the icon whose resource
            /// identifier is equal to the absolute value of nIconIndex.For example,
            /// use -3 to extract the icon whose resource identifier is 3.</param>
            /// <param name="piLargeVersion"></param>
            /// <param name="piSmallVersion"></param>
            /// <param name="amountIcons"></param>
            /// <returns></returns>
            [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            internal static extern int ExtractIconEx(string sFile,
                                                     int iIndex,
                                                     out IntPtr piLargeVersion,
                                                     out IntPtr piSmallVersion,
                                                     int amountIcons);
        }
        #endregion  private classes
    }
}
