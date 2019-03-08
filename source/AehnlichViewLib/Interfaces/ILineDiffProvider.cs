namespace AehnlichViewLib.Interfaces
{
    using AehnlichViewLib.Events;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an object that can match additional lines on demand when
    /// the user scrolls the view outside of the matched lines area
    /// (which has initially zero lines).
    /// 
    /// The rquest method below is used to initiate the computation and the
    /// result event is sent when the computation is finished and results in
    /// new highlightings.
    /// 
    /// The computation should be done on a non-ui thread to ensure a fluent UI.
    /// </summary>
    public interface ILineDiffProvider
    {
        /// <summary>
        /// Event is raised when newly requested line diff edit script segments
        /// have been computed and are available for hightlighting.
        /// </summary>
        event EventHandler<DiffLineInfoChangedEvent> DiffLineInfoChanged;

        /// <summary>
        /// Implements a method that is invoked by the view to request
        /// the matching (edit script computation) of the indicates text lines.
        /// 
        /// This method should be called on the UI thread since
        /// the resulting event <see cref="ILineDiffProvider.DiffLineInfoChanged"/>
        /// will be raised on the calling thread.
        /// </summary>
        /// <returns>Number of lines matched (may not be as requested if line appears to have been matched already).</returns>
        void RequestLineDiff(IEnumerable<int> linenumbers);
    }
}
