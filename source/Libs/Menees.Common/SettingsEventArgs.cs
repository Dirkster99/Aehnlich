namespace Menees
{
	#region Using Directives

	using System;
	using System.ComponentModel;

	#endregion

	/// <summary>
	/// The event arguments for loading and saving using an <see cref="ISettingsNode"/>.
	/// </summary>
	public sealed class SettingsEventArgs : EventArgs
	{
		#region Private Data Members

		private ISettingsNode settings;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance for the specified settings node.
		/// </summary>
		/// <param name="settings">A settings node.</param>
		public SettingsEventArgs(ISettingsNode settings)
		{
			Conditions.RequireReference(settings, () => settings);
			this.settings = settings;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Returns the settings node to use for loading or saving.
		/// </summary>
		public ISettingsNode SettingsNode => this.settings;

		#endregion
	}
}
