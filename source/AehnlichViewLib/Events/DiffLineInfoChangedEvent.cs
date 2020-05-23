namespace AehnlichViewLib.Events
{
	using System;
	using System.Collections.Generic;

	public enum DiffLineInfoChange
	{
		/// <summary>
		/// Is used with the <see cref="DiffLineInfoChangedEvent"/> to
		/// hint that Line Edit Script Segments should be re-drawn since
		/// this information has changed.
		/// </summary>
		LineEditScriptSegments
	}

	public class DiffLineInfoChangedEvent : EventArgs
	{
		public DiffLineInfoChangedEvent(DiffLineInfoChange parTypeOfInfoChange,
										IList<int> parLinesChanged)
		{
			TypeOfInfoChange = parTypeOfInfoChange;
			LinesChanged = parLinesChanged;
		}

		public DiffLineInfoChange TypeOfInfoChange { get; }

		public IList<int> LinesChanged { get; }
	}
}
