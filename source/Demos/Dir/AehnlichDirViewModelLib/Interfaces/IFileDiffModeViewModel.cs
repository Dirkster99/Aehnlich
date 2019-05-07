namespace AehnlichDirViewModelLib.Interfaces
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Implements an interface to a viewmodel that defines a comparison strategy for files
    /// (using lastupdate, size in bytes, and/or byte-by-byte comparison)
    /// </summary>
    public interface IFileDiffModeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a list of modies that can be used to compare one directory
        /// and its contents, to the other directory.
        /// </summary>
        List<IDiffFileModeItemViewModel> DiffFileModes { get; }

        /// <summary>
        /// Gets/sets the mode that is currently used to compare one directory
        /// and its contents with the other directory.
        /// </summary>
        IDiffFileModeItemViewModel DiffFileModeSelected { get; set; }
    }
}