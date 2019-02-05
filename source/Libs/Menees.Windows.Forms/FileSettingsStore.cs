namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Security;
	using System.Text;
	using System.Xml.Linq;

	#endregion

	internal sealed class FileSettingsStore : ISettingsStore
	{
		#region Private Data Members

		private XElement root;

		#endregion

		#region Constructors

		public FileSettingsStore()
		{
			foreach (string fileName in GetPotentialFileNames())
			{
				if (File.Exists(fileName))
				{
					this.root = XElement.Load(fileName);
					break;
				}
			}

			if (this.root == null)
			{
				this.root = FileSettingsNode.CreateNodeElement(ApplicationInfo.ApplicationName);
			}

			this.RootNode = new FileSettingsNode(this.root, null);
		}

		#endregion

		#region ISettingsStore Members

		public ISettingsNode RootNode
		{
			get;
			private set;
		}

		public void Save()
		{
			bool saved = false;
			Dictionary<string, object> errorLogProperties = new Dictionary<string, object>();

			foreach (string fileName in GetPotentialFileNames())
			{
				try
				{
					// Make sure the directory we're trying to save to actually exists.
					string directoryName = Path.GetDirectoryName(fileName);
					Directory.CreateDirectory(directoryName);

					if (File.Exists(fileName))
					{
						// Make sure the file isn't hidden, so XElement.Save can write to it.
						File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.Hidden);
					}

					// Attempt to save the file.
					this.root.Save(fileName);
					saved = true;

					// Hide the file from normal listings since we'll be creating files for each user.
					File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.Hidden);
					break;
				}
				catch (IOException inputOutputEx)
				{
					errorLogProperties.Add(fileName, inputOutputEx);
				}
				catch (SecurityException securityEx)
				{
					errorLogProperties.Add(fileName, securityEx);
				}
				catch (UnauthorizedAccessException accessEx)
				{
					errorLogProperties.Add(fileName, accessEx);
				}
			}

			if (!saved)
			{
				const string Message = "Unable to save the user settings to a file store.";

				// Log out the exceptions first since we can't pass those additional
				// details into the new exception we're throwing.
				Log.Error(typeof(FileSettingsStore), Message, null, errorLogProperties);
				throw Exceptions.NewInvalidOperationException(Message);
			}
		}

		public void Dispose()
		{
			// There's nothing to do here since XElement isn't disposable.
		}

		#endregion

		#region Private Methods

		private static IEnumerable<string> GetPotentialFileNames()
		{
			string fileName = ApplicationInfo.ApplicationName + "-" + Environment.UserDomainName + "-" + Environment.UserName + ".stgx";

			List<string> result = new List<string>();

			// First, try the application's base directory.  That gives the best isolation for side-by-side app usage,
			// but a non-admin user may not have permissions to write to this directory.
			string path = Path.Combine(ApplicationInfo.BaseDirectory, fileName);
			result.Add(path);

			// Next, try a Menees folder under the user's AppData\Local folder.  The user should be able to write
			// to their local AppData folder if it exists (e.g., if the user is not in a roaming profile).
			string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			if (!string.IsNullOrEmpty(localAppDataPath))
			{
				path = Path.Combine(localAppDataPath, "Menees", fileName);
				result.Add(path);
			}

			// If all else fails, try to use the current temp directory (which may or may not be user-specific);
			path = Path.Combine(Path.GetTempPath(), "Menees", fileName);
			result.Add(path);

			return result;
		}

		#endregion

		#region Private Types

		private sealed class FileSettingsNode : ISettingsNode
		{
			#region Private Data Members

			private XElement element;
			private FileSettingsNode parent;

			#endregion

			#region Constructors

			public FileSettingsNode(XElement element, FileSettingsNode parent)
			{
				this.element = element;
				this.parent = parent;
			}

			#endregion

			#region ISettingsNode Members

			public string NodeName
			{
				get
				{
					string result = GetNodeName(this.element);
					return result;
				}
			}

			public int SettingCount
			{
				get
				{
					int result = this.GetSettingElements().Count();
					return result;
				}
			}

			public int SubNodeCount
			{
				get
				{
					int result = this.GetSubNodeElements().Count();
					return result;
				}
			}

			public ISettingsNode ParentNode
			{
				get { return this.parent; }
			}

			public string GetValue(string settingName, string defaultValue)
			{
				string result = defaultValue;

				XElement settingElement = this.GetSettingElement(settingName, false);
				if (settingElement != null)
				{
					result = settingElement.GetAttributeValue("Value", defaultValue);
				}

				return result;
			}

			public void SetValue(string settingName, string value)
			{
				XElement settingElement = this.GetSettingElement(settingName, true);
				settingElement.SetAttributeValue("Value", value);
			}

			public int GetValue(string settingName, int defaultValue)
			{
				int result = defaultValue;

				XElement settingElement = this.GetSettingElement(settingName, false);
				if (settingElement != null)
				{
					result = settingElement.GetAttributeValue("Value", defaultValue);
				}

				return result;
			}

			public void SetValue(string settingName, int value)
			{
				this.SetValue(settingName, Convert.ToString(value));
			}

			public bool GetValue(string settingName, bool defaultValue)
			{
				bool result = defaultValue;

				XElement settingElement = this.GetSettingElement(settingName, false);
				if (settingElement != null)
				{
					result = settingElement.GetAttributeValue("Value", defaultValue);
				}

				return result;
			}

			public void SetValue(string settingName, bool value)
			{
				this.SetValue(settingName, Convert.ToString(value));
			}

			public T GetValue<T>(string settingName, T defaultValue) where T : struct
			{
				T result = defaultValue;

				XElement settingElement = this.GetSettingElement(settingName, false);
				if (settingElement != null)
				{
					result = settingElement.GetAttributeValue("Value", defaultValue);
				}

				return result;
			}

			public void SetValue<T>(string settingName, T value) where T : struct
			{
				this.SetValue(settingName, Convert.ToString(value));
			}

			public IList<string> GetSettingNames()
			{
				var result = this.GetSettingElements().Select(e => GetSettingName(e)).ToList();
				return result;
			}

			public void DeleteSetting(string settingName)
			{
				XElement settingElement = this.GetSettingElement(settingName, false);
				if (settingElement != null)
				{
					settingElement.Remove();
				}
			}

			public IList<string> GetSubNodeNames()
			{
				var result = this.GetSubNodeElements().Select(e => GetNodeName(e)).ToList();
				return result;
			}

			public void DeleteSubNode(string nodeNameOrPath)
			{
				XElement subElement = this.GetSubNodeElement(nodeNameOrPath, false);
				if (subElement != null)
				{
					subElement.Remove();
				}
			}

			public ISettingsNode GetSubNode(string nodeNameOrPath, bool createIfNotFound)
			{
				FileSettingsNode result = null;

				XElement subElement = this.GetSubNodeElement(nodeNameOrPath, createIfNotFound);
				if (subElement != null)
				{
					result = new FileSettingsNode(subElement, this);
				}

				return result;
			}

			#endregion

			#region Internal Methods

			internal static XElement CreateNodeElement(string nodeName)
			{
				Conditions.RequireString(nodeName, "nodeName");
				XElement result = new XElement("Settings", new XAttribute("Name", nodeName));
				return result;
			}

			#endregion

			#region Private Methods

			private static string GetNodeName(XElement nodeElement)
			{
				string result = nodeElement.GetAttributeValue("Name");
				return result;
			}

			private static string GetSettingName(XElement settingElement)
			{
				string result = settingElement.GetAttributeValue("Name");
				return result;
			}

			private static IEnumerable<XElement> GetSubNodeElements(XElement element)
			{
				var result = element.Elements("Settings");
				return result;
			}

			private IEnumerable<XElement> GetSubNodeElements()
			{
				var result = GetSubNodeElements(this.element);
				return result;
			}

			private IEnumerable<XElement> GetSettingElements()
			{
				var result = this.element.Elements("Setting");
				return result;
			}

			private XElement GetSubNodeElement(string nodeNameOrPath, bool createIfNotFound)
			{
				Conditions.RequireString(nodeNameOrPath, "nodeNameOrPath");

				XElement result = null;
				XElement currentElement = this.element;

				// Use backslash as the path separator because that's what RegistryKey.OpenSubKey uses,
				// and callers need to be able to switch between the ISettingsStore types without breaking.
				// FWIW, RegistryKey ignores the forward slash character, so we can't split on it.
				string[] nodeNames = nodeNameOrPath.Split('\\');
				foreach (string nodeName in nodeNames)
				{
					result = GetSubNodeElements(currentElement).Where(e => GetNodeName(e) == nodeName).SingleOrDefault();
					if (result == null)
					{
						if (!createIfNotFound)
						{
							break;
						}

						result = CreateNodeElement(nodeName);
						currentElement.Add(result);
					}

					currentElement = result;
				}

				return result;
			}

			private XElement GetSettingElement(string settingName, bool createIfNotFound)
			{
				XElement result = this.GetSettingElements().Where(e => GetSettingName(e) == settingName).SingleOrDefault();
				if (result == null && createIfNotFound)
				{
					result = new XElement("Setting", new XAttribute("Name", settingName));
					this.element.Add(result);
				}

				return result;
			}

			#endregion
		}

		#endregion
	}
}
