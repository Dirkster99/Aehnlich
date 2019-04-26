namespace AehnlichViewModelsLib.Interfaces
{
    using AehnlichLib.Text;
    using AehnlichViewLib.Interfaces;
    using AehnlichViewModelsLib.Enums;
    using AehnlichViewModelsLib.ViewModels.LineInfo;
    using System.ComponentModel;

    /// <summary>
    /// Implements a viewmodel that provides the data necessary to show highlighting
    /// and text content for one line in the textual diff view.
    /// </summary>
    public interface IDiffLineViewModel : IDiffLineInfo, INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Gets the original text that was used when comparing this line to its
        /// <see cref="Counterpart"/> line.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets the equivalent line from the left view to the right view
        /// and vice versa.
        /// </summary>
        DiffViewLine Counterpart { get; }
        #endregion properties

        /// <summary>
        /// Matches this line against its counterpart model line and
        /// outputs an edit script to highlight operations necessary
        /// to transfer this line's content into the content of the other line.
        /// </summary>
        /// <param name="changeDiffOptions"></param>
        /// <param name="spacesPerTab"></param>
        /// <returns></returns>
        EditScript GetChangeEditScript(ChangeDiffOptions changeDiffOptions, int spacesPerTab);
    }
}
