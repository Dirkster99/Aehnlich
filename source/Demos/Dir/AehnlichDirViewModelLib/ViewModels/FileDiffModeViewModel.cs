﻿namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Interfaces;
    using AehnlichLib.Enums;
    using System.Collections.Generic;

    internal class FileDiffModeViewModel : Base.ViewModelBase, IFileDiffModeViewModel
    {
        #region fields
        private readonly List<IDiffFileModeItemViewModel> _DiffFileModes;
        private IDiffFileModeItemViewModel _DiffFileModeSelected;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public FileDiffModeViewModel()
        {
            _DiffFileModes = new List<IDiffFileModeItemViewModel>();
            _DiffFileModeSelected = CreateCompareFileModes(_DiffFileModes);
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a list of modies that can be used to compare one directory
        /// and its contents, to the other directory.
        /// </summary>
        public List<IDiffFileModeItemViewModel> DiffFileModes
        {
            get
            {
                return _DiffFileModes;
            }
        }

        /// <summary>
        /// Gets/sets the mode that is currently used to compare one directory
        /// and its contents with the other directory.
        /// </summary>
        public IDiffFileModeItemViewModel DiffFileModeSelected
        {
            get
            {
                return _DiffFileModeSelected;
            }

            set
            {
                if (_DiffFileModeSelected != value)
                {
                    _DiffFileModeSelected = value;
                    NotifyPropertyChanged(() => DiffFileModeSelected);
                }
            }
        }
        #endregion properties

        #region methods

        private static IDiffFileModeItemViewModel CreateCompareFileModes(
            IList<IDiffFileModeItemViewModel> diffFileModes)
        {
            DiffFileModeItemViewModel defaultItem = null;

            diffFileModes.Add(
                new DiffFileModeItemViewModel("File Length",
                "Compare the byte length of each file",
                DiffDirFileMode.ByteLength));

            diffFileModes.Add(
                new DiffFileModeItemViewModel("Last Change",
                "Compare last modification time of change of each file",
                DiffDirFileMode.LastUpdate));

            defaultItem = new DiffFileModeItemViewModel("File Length + Last Change",
                "Compare the byte length and last modification time of each file",
                DiffDirFileMode.ByteLength_LastUpdate);

            diffFileModes.Add(defaultItem);

            diffFileModes.Add(new DiffFileModeItemViewModel("All Bytes",
                "Compare each file by their length, last modification time, and byte-by-byte sequence",
                DiffDirFileMode.ByteLength_LastUpdate_AllBytes));

            return defaultItem;
        }
        #endregion methods
    }
}
