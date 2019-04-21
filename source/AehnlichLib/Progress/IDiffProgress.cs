namespace AehnlichLib.Progress
{
    using System;
    using System.Threading;

    /// <summary>
    /// Exposes the properties of a Progress Display (ProgressBar) to enable
    /// UI feedback on long running processings.
    /// </summary>
    public interface IDiffProgress
    {
        #region properties
        /// <summary>
        /// Gets whether the progress indicator is currenlty displayed or not.
        /// </summary>
        bool IsProgressbarVisible { get; }

        /// <summary>
        /// Gets whether the current progress display <seealso cref="IsProgressbarVisible"/>
        /// is indeterminate or not.
        /// </summary>
        bool IsIndeterminate { get; }

        /// <summary>
        /// Gets the current progress value that should be displayed if the progress is turned on
        /// via <seealso cref="IsProgressbarVisible"/> and is not indeterminated as indicated in
        /// <seealso cref="IsIndeterminate"/>.
        /// </summary>
        double ProgressValue { get; }

        /// <summary>
        /// Gets the minimum progress value that should be displayed if the progress is turned on
        /// via <seealso cref="IsProgressbarVisible"/> and is not indeterminated as indicated in
        /// <seealso cref="IsIndeterminate"/>.
        /// </summary>
        double MinimumProgressValue { get; }

        /// <summary>
        /// Gets the maximum progress value that should be displayed if the progress is turned on
        /// via <seealso cref="IsProgressbarVisible"/> and is not indeterminated as indicated in
        /// <seealso cref="IsIndeterminate"/>.
        /// </summary>
        double MaximumProgressValue { get; }

        /// <summary>
        /// Gets/sets the result data object that is available when task
        /// progressing has finished.
        /// </summary>
        object ResultData { get; set; }

        /// <summary>
        /// Gets an <see cref="Exception"/> (if set by background task)
        /// that can be used to displayed advanced error messages in the UI.
        /// </summary>
        Exception ErrorException { get; }

        /// <summary>
        /// Gets an error message string (if set by background task)
        /// that can be used to display error messages in the UI.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets a cancellation token (if any is set by the caller) that can be used
        /// to cancel the current processing (if it was setup a background task).
        /// </summary>
        CancellationToken Token { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method enables properties such that display of
        /// indeterminate progress is turned on.
        /// </summary>
        void ShowIndeterminatedProgress();

        /// <summary>
        /// Method enables properties such that display of
        /// determinate progress is turned on.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        void ShowDeterminatedProgress(double value,
                                      double minimum = 0,
                                      double maximum = 100);

        /// <summary>
        /// Method updates a display of determinate progress
        /// which should previously been turned on via
        /// <seealso cref="ShowDeterminatedProgress(double, double, double)"/>
        /// is turned on.
        /// </summary>
        /// <param name="value"></param>
        void UpdateDeterminatedProgress(double value);

        /// <summary>
        /// Method turns the current progress display off.
        /// </summary>
        void ProgressDisplayOff();

        /// <summary>
        /// This method can be called by the backgound task to log an exception
        /// for later display in the UI.
        /// </summary>
        /// <param name="exp"></param>
        void LogException(Exception exp);

        /// <summary>
        /// This method can be called by the backgound task to log an error message
        /// for later display in the UI.
        /// </summary>
        /// <param name="error"></param>
        void LogErrorMessage(string error);
        #endregion methods
    }
}
