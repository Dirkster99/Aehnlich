namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.ComponentModel;

	#endregion

	/// <summary>
	/// The event arguments for the <see cref="FormSaver.LoadSettings"/> and <see cref="FormSaver.SaveSettings"/> events.
	/// </summary>
	[Description("The event arguments for the LoadSettings and SaveSettings events.")]
	public sealed class SettingsEventArgs : EventArgs
	{
		#region Private Data Members

		private ISettingsNode settings;

		#endregion

		#region Constructors

		internal SettingsEventArgs(ISettingsNode settings)
		{
			this.settings = settings;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Returns the settings node to use for loading or saving.
		/// </summary>
		public ISettingsNode SettingsNode
		{
			get { return this.settings; }
		}

		#endregion
	}
}
