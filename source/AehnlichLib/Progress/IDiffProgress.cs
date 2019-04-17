using System;
using System.Threading;

namespace AehnlichLib.Progress
{
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

        Exception ErrorException { get; }

        string ErrorMessage { get; }

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

        void LogException(Exception exp);

        void LogErrorMessage(string error);
        #endregion methods
    }
}
