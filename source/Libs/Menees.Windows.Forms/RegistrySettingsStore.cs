namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using Microsoft.Win32;

	#endregion

	internal sealed class RegistrySettingsStore : ISettingsStore
	{
		#region Private Data Members

		private List<RegistrySettingsNode> nodes = new List<RegistrySettingsNode>();

		#endregion

		#region Constructors

		public RegistrySettingsStore()
		{
			string rootKeyPath = @"Software\Menees\" + ApplicationInfo.ApplicationName;
			RegistryKey rootKey = Registry.CurrentUser.CreateSubKey(rootKeyPath);
			this.RootNode = new RegistrySettingsNode(rootKey, null, this);
		}

		#endregion

		#region ISettingsStore Members

		public ISettingsNode RootNode
		{
			get;
			private set;
		}

		void ISettingsStore.Save()
		{
			// Registry changes occur immediately, so there's nothing pending to save.
		}

		public void Dispose()
		{
			foreach (var node in this.nodes)
			{
				node.Dispose();
			}

			this.nodes.Clear();
		}

		#endregion

		#region Private Methods

		private void AddNode(RegistrySettingsNode node)
		{
			this.nodes.Add(node);
		}

		#endregion

		#region Private Types

		private sealed class RegistrySettingsNode : ISettingsNode, IDisposable
		{
			#region Private Data Members

			private RegistryKey key;
			private RegistrySettingsNode parent;
			private RegistrySettingsStore store;

			#endregion

			#region Constructors

			public RegistrySettingsNode(RegistryKey key, RegistrySettingsNode parent, RegistrySettingsStore store)
			{
				this.key = key;
				this.parent = parent;
				this.store = store;
				this.store.AddNode(this);
			}

			#endregion

			#region ISettingsNode Members

			public string NodeName
			{
				get { return this.key.Name; }
			}

			public int SettingCount
			{
				get { return this.key.ValueCount; }
			}

			public int SubNodeCount
			{
				get { return this.key.SubKeyCount; }
			}

			public ISettingsNode ParentNode
			{
				get { return this.parent; }
			}

			public string GetValue(string settingName, string defaultValue)
			{
				string result = Convert.ToString(this.key.GetValue(settingName, defaultValue));
				return result;
			}

			public void SetValue(string settingName, string value)
			{
				this.key.SetValue(settingName, value);
			}

			public int GetValue(string settingName, int defaultValue)
			{
				int result = Convert.ToInt32(this.key.GetValue(settingName, defaultValue));
				return result;
			}

			public void SetValue(string settingName, int value)
			{
				this.key.SetValue(settingName, value);
			}

			public bool GetValue(string settingName, bool defaultValue)
			{
				bool result = this.GetValue(settingName, defaultValue ? 1 : 0) != 0;
				return result;
			}

			public void SetValue(string settingName, bool value)
			{
				this.SetValue(settingName, value ? 1 : 0);
			}

			public T GetValue<T>(string settingName, T defaultValue) where T : struct
			{
				string value = this.GetValue(settingName, string.Empty);

				T result = defaultValue;
				if (!string.IsNullOrEmpty(value) && !Enum.TryParse<T>(value, out result))
				{
					result = defaultValue;
				}

				return result;
			}

			public void SetValue<T>(string settingName, T value) where T : struct
			{
				this.SetValue(settingName, value.ToString());
			}

			public IList<string> GetSettingNames()
			{
				string[] result = this.key.GetValueNames();
				return result;
			}

			public void DeleteSetting(string settingName)
			{
				this.key.DeleteValue(settingName);
			}

			public IList<string> GetSubNodeNames()
			{
				string[] result = this.key.GetSubKeyNames();
				return result;
			}

			public void DeleteSubNode(string nodeName)
			{
				this.key.DeleteSubKeyTree(nodeName);
			}

			public ISettingsNode GetSubNode(string nodeName, bool createIfNotFound)
			{
				RegistrySettingsNode result = null;

				if (createIfNotFound)
				{
					result = new RegistrySettingsNode(this.key.CreateSubKey(nodeName), this, this.store);
				}
				else
				{
					RegistryKey subKey = this.key.OpenSubKey(nodeName, true);
					if (subKey != null)
					{
						result = new RegistrySettingsNode(subKey, this, this.store);
					}
				}

				return result;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (this.key != null)
				{
					this.key.Dispose();
				}
			}

			#endregion
		}

		#endregion
	}
}
