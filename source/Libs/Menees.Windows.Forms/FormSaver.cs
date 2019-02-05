namespace Menees.Windows.Forms
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Diagnostics;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	#endregion

	/// <summary>
	/// Used to save and load a form's position and state.
	/// </summary>
	[DefaultEvent("LoadSettings")]
	[ToolboxBitmap(typeof(FormSaver), "Images.FormSaver.bmp")]
	public partial class FormSaver : Component
	{
		#region Private Data Members

		private const string FormLayout = "Form Layout";

		private bool autoLoad = true;
		private bool autoSave = true;
		private bool allowLoadMinimized;
		private string nodeName = FormLayout;
		private ContainerControl containerControl;
		private Form form;
		private EventHandler loadHandler;
		private FormClosedEventHandler closedHandler;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public FormSaver()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Creates a new instance in the specified container.
		/// </summary>
		public FormSaver(IContainer container)
		{
			if (container != null)
			{
				container.Add(this);
			}

			this.InitializeComponent();
		}

		/// <summary>
		/// Creates a new instance for the specified container control.
		/// </summary>
		public FormSaver(ContainerControl container)
		{
			this.InitializeComponent();
			this.ContainerControl = container;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Called when settings are being loaded.
		/// </summary>
		[Browsable(true)]
		[Category("Serialization")]
		[Description("Called when settings are being loaded.")]
		public event EventHandler<SettingsEventArgs> LoadSettings;

		/// <summary>
		/// Called when settings are being saved.
		/// </summary>
		[Browsable(true)]
		[Category("Serialization")]
		[Description("Called when settings are being saved.")]
		public event EventHandler<SettingsEventArgs> SaveSettings;

		#endregion

		#region Internal Events

		// Called before any LoadSettings events.
		[Browsable(false)]
		internal event EventHandler<SettingsEventArgs> InternalLoadSettings;

		// Called after any SaveSettings events.
		[Browsable(true)]
		internal event EventHandler<SettingsEventArgs> InternalSaveSettings;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets whether the settings should automatically load when the form loads.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(true)]
		[Category("Behavior")]
		[Description("Whether the settings should automatically load when the form loads.")]
		public bool AutoLoad
		{
			get { return this.autoLoad; }
			set { this.autoLoad = value; }
		}

		/// <summary>
		/// Gets or sets whether the settings should automatically save when the form closes.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(true)]
		[Category("Behavior")]
		[Description("Whether the settings should automatically save when the form closes.")]
		public bool AutoSave
		{
			get { return this.autoSave; }
			set { this.autoSave = value; }
		}

		/// <summary>
		/// Gets or sets whether the form should be allowed to load minimized.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(false)]
		[Category("Behavior")]
		[Description("Whether the form should be allowed to load minimized.")]
		public bool AllowLoadMinimized
		{
			get { return this.allowLoadMinimized; }
			set { this.allowLoadMinimized = value; }
		}

		/// <summary>
		/// Gets or sets the node where settings should be saved.  This can be empty.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(FormLayout)]
		[Category("Behavior")]
		[Description("The node where settings should be saved.  This can be empty.")]
		public string SettingsNodeName
		{
			get { return this.nodeName; }
			set { this.nodeName = value; }
		}

		/// <summary>
		/// Gets or sets the form to save the settings for.  This must be set.
		/// </summary>
		[Browsable(false)]
		[DefaultValue(null)]
		[Category("Helper Objects")]
		[Description("The form to save the settings for.  This must be set.")]
		public ContainerControl ContainerControl
		{
			get
			{
				return this.containerControl;
			}

			set
			{
				this.containerControl = value;
				this.Form = this.containerControl as Form;
			}
		}

		/// <summary>
		/// Gets or sets the site for the current component.
		/// </summary>
		public override ISite Site
		{
			set
			{
				// This is used to automatically set the ContainerControl and Form
				// properties.  I got this code from the ErrorProvider component and
				// from http://www.wiredprairie.us/hardwired,ComponentContainer.aspx.
				base.Site = value;
				if (value != null)
				{
					IDesignerHost host = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if (host != null)
					{
						ContainerControl rootContainer = host.RootComponent as ContainerControl;
						if (rootContainer != null)
						{
							this.ContainerControl = rootContainer;
						}
					}
				}
			}
		}

		#endregion

		#region Private Properties

		private Form Form
		{
			get
			{
				return this.form;
			}

			set
			{
				if (this.form != value)
				{
					if (this.form != null)
					{
						// Detach from old form events
						this.form.Load -= this.loadHandler;
						this.form.FormClosed -= this.closedHandler;
					}

					this.form = value;

					if (this.form != null)
					{
						// Create event handlers
						if (this.loadHandler == null)
						{
							this.loadHandler = this.OnFormLoad;
						}

						if (this.closedHandler == null)
						{
							this.closedHandler = this.OnFormClosed;
						}

						// Attach to new form events
						this.form.Load += this.loadHandler;
						this.form.FormClosed += this.closedHandler;
					}
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Used to explicitly load the form's settings if <see cref="AutoLoad"/> is false.
		/// </summary>
		/// <returns>True if the previous settings were re-loaded;
		/// False if no previous settings existed.</returns>
		public bool Load()
		{
			bool result = false;

			Conditions.RequireState(this.form != null, "Load requires a non-null Form.");

			if (!this.DesignMode)
			{
				using (ISettingsStore store = ApplicationInfo.CreateUserSettingsStore())
				{
					ISettingsNode formLayoutNode = this.GetFormLayoutNode(store, false);
					if (formLayoutNode != null)
					{
						result = true;

						int left = formLayoutNode.GetValue("Left", this.form.Left);
						int top = formLayoutNode.GetValue("Top", this.form.Top);
						int width = formLayoutNode.GetValue("Width", this.form.Width);
						int height = formLayoutNode.GetValue("Height", this.form.Height);
						FormWindowState state = formLayoutNode.GetValue("WindowState", this.form.WindowState);

						this.form.SuspendLayout();
						try
						{
							this.form.Location = new Point(left, top);
							if (this.form.FormBorderStyle == FormBorderStyle.Sizable || this.form.FormBorderStyle == FormBorderStyle.SizableToolWindow)
							{
								this.form.Size = new Size(width, height);
							}

							// If the form's current state isn't Normal, then it was launched from a
							// shortcut or command line START command to be Minimized or Maximized.
							// In those cases, we don't want to override the state.
							if (this.form.WindowState == FormWindowState.Normal &&
								(this.allowLoadMinimized || state != FormWindowState.Minimized))
							{
								this.form.WindowState = state;
							}
						}
						finally
						{
							this.form.ResumeLayout();
						}

						// Make sure the window is somewhere on one of the screens.
						if (!SystemInformation.VirtualScreen.Contains(left, top))
						{
							Point point = SystemInformation.VirtualScreen.Location;
							const int DefaultOffset = 20;
							point.Offset(DefaultOffset, DefaultOffset);
							this.form.DesktopLocation = point;
						}
					}

					// Fire the internal event first
					var eventHandler = this.InternalLoadSettings;
					if (eventHandler != null)
					{
						eventHandler(this, new SettingsEventArgs(store.RootNode));
					}

					eventHandler = this.LoadSettings;
					if (eventHandler != null)
					{
						eventHandler(this, new SettingsEventArgs(store.RootNode));
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Used to explicitly save the form's settings if <see cref="AutoSave"/> is false.
		/// </summary>
		public void Save()
		{
			Conditions.RequireState(this.form != null, "Save requires a non-null Form.");

			if (!this.DesignMode)
			{
				using (ISettingsStore store = ApplicationInfo.CreateUserSettingsStore())
				{
					ISettingsNode formLayoutNode = this.GetFormLayoutNode(store, true);
					if (formLayoutNode != null)
					{
						// If the form is currently minimized or maximized, then pulling the form's current Bounds
						// will return values we don't want to store (e.g., -32000 or full screen).  We always want
						// the Normal window position, so we have to make a Windows API call to get that reliably.
						Rectangle bounds = NativeMethods.GetNormalWindowBounds(this.form);
						formLayoutNode.SetValue("Left", bounds.Left);
						formLayoutNode.SetValue("Top", bounds.Top);
						formLayoutNode.SetValue("Width", bounds.Width);
						formLayoutNode.SetValue("Height", bounds.Height);
						formLayoutNode.SetValue("WindowState", this.form.WindowState);
					}

					// Fire the public event first.
					var eventHandler = this.SaveSettings;
					if (eventHandler != null)
					{
						eventHandler(this, new SettingsEventArgs(store.RootNode));
					}

					eventHandler = this.InternalSaveSettings;
					if (eventHandler != null)
					{
						eventHandler(this, new SettingsEventArgs(store.RootNode));
					}

					// Make sure the settings get saved out.
					store.Save();
				}
			}
		}

		#endregion

		#region Private Methods

		private ISettingsNode GetFormLayoutNode(ISettingsStore store, bool createIfNotFound)
		{
			ISettingsNode result = store.RootNode;

			if (!string.IsNullOrEmpty(this.nodeName))
			{
				result = result.GetSubNode(this.nodeName, createIfNotFound);
			}

			return result;
		}

		private void OnFormLoad(object sender, EventArgs e)
		{
			if (this.autoLoad)
			{
				this.Load();
			}
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.autoSave)
			{
				this.Save();
			}
		}

		#endregion
	}
}
