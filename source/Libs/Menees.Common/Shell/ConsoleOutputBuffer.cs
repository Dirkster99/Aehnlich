namespace Menees.Shell
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Text;

	#endregion

	/// <summary>
	/// This class is used to capture the output of a console application.
	/// </summary>
	[DebuggerDisplay("Text = {GetText()}")]
	public sealed class ConsoleOutputBuffer
	{
		#region Private Data Members

		private List<string> lines = new List<string>();
		private bool includeErrorStream;
		private int processExitCode;
		private bool hasProcessExited;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates an instance for the specified process.
		/// </summary>
		/// <param name="startInfo">
		/// The start information for the command-line process.  This instance will
		/// be modified to redirect the console streams so the output can be
		/// captured.
		/// </param>
		/// <param name="includeErrorStream">Whether the lines from the Process.StandardError stream should be included.</param>
		public ConsoleOutputBuffer(ProcessStartInfo startInfo, bool includeErrorStream)
			: this(startInfo, includeErrorStream, TimeSpan.FromMilliseconds(int.MaxValue))
		{
		}

		/// <summary>
		/// Creates an instance for the specified process and waits a specified amount of time for it to finish.
		/// </summary>
		/// <param name="startInfo">
		/// The start information for the command-line process.  This instance will
		/// be modified to redirect the console streams so the output can be
		/// captured.
		/// </param>
		/// <param name="includeErrorStream">Whether the lines from the Process.StandardError stream should be included.</param>
		/// <param name="waitTime">The time to wait for the process to finish.
		/// If the process fails to finish, then <see cref="HasProcessExited"/>
		/// will be set to false.
		/// </param>
		public ConsoleOutputBuffer(ProcessStartInfo startInfo, bool includeErrorStream, TimeSpan waitTime)
		{
			Conditions.RequireReference(startInfo, "StartInfo");

			this.includeErrorStream = includeErrorStream;

			// Make it hidden, and use CreateProcess so we can redirect the streams.
			startInfo.CreateNoWindow = true;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.UseShellExecute = false;

			// Some console apps (e.g., XCopy on Windows 2000) require that if
			// one stream is redirected, then they must all be redirected.  Since we
			// don't know what we're dealing with here, we'll just redirect them all.
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardInput = true;
			startInfo.RedirectStandardError = true;

			// Create a new process instance.
			using (Process proc = new Process())
			{
				proc.StartInfo = startInfo;

				// Attach to the asynchronous stream events.
				proc.OutputDataReceived += this.Process_OutputDataReceived;
				proc.ErrorDataReceived += this.Process_ErrorDataReceived;

				// Start the process.
				proc.Start();

				// Begin asynchronous reading of the streams.
				proc.BeginOutputReadLine();
				proc.BeginErrorReadLine();

				// TimeSpans can go a lot longer (~29227 years) than WaitForExit's integer millisecond limit (~24.5 days).
				// If the TimeSpan is too large, just truncate it to the OS's "infinite" limit, which is -1.  Also, using -1 has
				// special meaning to .NET (up through 4.0 at least).  Calling WaitForExit(-1) includes special logic to force
				// the async stream reading to finish up.  If any other wait timeout is used, then control can return to the
				// caller before all async console output comes in.
				double waitMillisecondsDouble = Math.Abs(waitTime.TotalMilliseconds);
				int waitMilliseconds = waitMillisecondsDouble >= int.MaxValue ? -1 : (int)waitMillisecondsDouble;

				// Wait for the process to finish.
				this.hasProcessExited = proc.WaitForExit(waitMilliseconds);
				if (this.hasProcessExited)
				{
					this.processExitCode = proc.ExitCode;

					// Call WaitForExit(-1) to force the async console stream reading to finish before we return to the caller.
					proc.WaitForExit(-1);
				}
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the exit code of the process.
		/// </summary>
		/// <remarks>
		/// If you specified a wait timeout to the constructor, then you must ensure
		/// that <see cref="HasProcessExited"/> is true before pulling this property.  If
		/// <see cref="HasProcessExited"/> is false, then this will throw an
		/// <see cref="InvalidOperationException"/>.
		/// </remarks>
		public int ProcessExitCode
		{
			get
			{
				if (!this.hasProcessExited)
				{
					throw Exceptions.NewInvalidOperationException(
						"The process exit code cannot be returned because the process did not exit during the specified wait time.");
				}

				return this.processExitCode;
			}
		}

		/// <summary>
		/// Gets whether the process has exited.
		/// </summary>
		public bool HasProcessExited => this.hasProcessExited;

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets all of the console output lines.
		/// </summary>
		/// <returns>An array of console output lines.</returns>
		public string[] GetLines()
		{
			lock (this.lines)
			{
				string[] result = this.lines.ToArray();
				return result;
			}
		}

		/// <summary>
		/// Gets all of the console output
		/// </summary>
		/// <returns></returns>
		[SuppressMessage(
			"Microsoft.Design",
			"CA1024:UsePropertiesWhereAppropriate",
			Justification = "This has to dynamically build a long string from an array of strings.")]
		public string GetText()
		{
			string[] lines = this.GetLines();
			string result = string.Join(Environment.NewLine, lines);
			return result;
		}

		#endregion

		#region Private Methods

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			this.AddData(e);
		}

		private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (this.includeErrorStream)
			{
				this.AddData(e);
			}
		}

		private void AddData(DataReceivedEventArgs e)
		{
			// For some apps, we'll get null data lines, so I'm treating
			// those as empty strings.  I don't want callers to have to
			// check for null lines in the array returned from GetLines().
			string line = e.Data ?? string.Empty;

			// Some apps (like VC++) output lines that end with an
			// embedded NULL character.  We need to strip that off
			// so the lines can be concatenated together and still
			// display correctly.
			int length = line.Length;
			if (length > 0 && line[length - 1] == '\0')
			{
				line = line.Substring(0, length - 1);
			}

			// Lock because this event can fire from any thread.
			lock (this.lines)
			{
				this.lines.Add(line);
			}
		}

		#endregion
	}
}
