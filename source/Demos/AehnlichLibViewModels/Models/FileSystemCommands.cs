namespace AehnlichLibViewModels.Models
{
    using System.Diagnostics;

    /// <summary>
    /// Class implements base services for opening and working with folders and files in Windows.
    /// </summary>
    public static class FileSystemCommands
    {
        /// <summary>
        /// Convinience method to open Windows Explorer with a selected file (if it exists).
        /// Otherwise, Windows Explorer is opened in the location where the file should be at.
        /// Returns falsem if neither file nor given directory exist.
        /// </summary>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        public static bool OpenContainingFolder(string sFileName)
        {
            return OpenContainingFolder(sFileName, null);
        }

        /// <summary>
        /// Convinience method to open Windows Explorer with a selected file (if it exists).
        /// Otherwise, Windows Explorer is opened in the location where the file should be at.
        /// Returns falsem if neither file nor given directory exist.
        /// </summary>
        /// <param name="sFileName"></param>
        /// <param name="sParent"></param>
        /// <returns></returns>
        public static bool OpenContainingFolder(string sFileName, string sParent)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return false;

            try
            {
                // combine the arguments together it doesn't matter if there is a space after ','
                string argument = @"/select, " + sFileName;

                System.Diagnostics.Process.Start("explorer.exe", argument);
                return true;
            }
            catch
            {
                try
                {
                    if (sParent != null)
                    {
                        // combine the arguments together it doesn't matter if there is a space after ','
                        string argument = @"/select, " + sParent;
                        System.Diagnostics.Process.Start("explorer.exe", argument);

                        return true;
                    }
                }
                catch
                {
                    // Catch this and return false in case run-time error occurs
                }

                return false;
            }
        }

        /// <summary>
        /// Opens a file with the current Windows default application.
        /// 
        /// Throws variaous exceptions (eg.: System.IO.FileNotFoundException)
        /// see <see cref="Process.Start()"/> for details.
        /// </summary>
        /// <param name="sFileName"></param>
        public static void OpenInWindows(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return;

            // re-throw to let caller know this was not a success
            Process.Start(new ProcessStartInfo(sFileName));
        }

        /// <summary>
        /// Copies the given string into the Windows clipboard.
        /// </summary>
        /// <param name="sTextToCopy"></param>
        public static void CopyString(string sTextToCopy)
        {
            if (string.IsNullOrEmpty(sTextToCopy) == true)
                return;

            try
            {
                System.Windows.Clipboard.SetText(sTextToCopy);
            }
            catch
            {
                // We should not get here but just in case we did...
            }
        }
    }
}
