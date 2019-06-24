namespace Aehnlich.ViewModels.Themes
{
    using MLib.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implements a model that keeps track of all elements belonging to a WPF Theme
    /// including highlighting theme, display name, required resources (XAML) and so forth.
    /// </summary>
    public class ThemeDefinitionViewModel : ViewModels.Base.ViewModelBase, IThemeInfo
    {
        #region fields
        private bool _IsSelected;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="themeSources"></param>
        public ThemeDefinitionViewModel(string themeName,
                               List<Uri> themeSources,
                               string highlightingThemeName)
            : this()
        {
            DisplayName = themeName;

            if (themeSources != null)
            {
                foreach (var item in themeSources)
                    ThemeSources.Add(new Uri(item.OriginalString, UriKind.Relative));
            }

            HighlightingThemeName = highlightingThemeName;
        }

        /// <summary>
        /// Copy constructor from <see cref="IThemeInfo"/> parameter.
        /// </summary>
        /// <param name="theme"></param>
        public ThemeDefinitionViewModel(IThemeInfo theme)
            : this()
        {
            this.DisplayName = theme.DisplayName;
            this.ThemeSources = new List<Uri>(theme.ThemeSources);
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected ThemeDefinitionViewModel()
        {
            DisplayName = string.Empty;
            ThemeSources = new List<Uri>();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the displayable (localized) name for this theme.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the Uri sources for this theme.
        /// </summary>
        public List<Uri> ThemeSources { get; private set; }

        /// <summary>
        /// Gets the name of the associated Highlighting Theme for AvalonEdit.
        /// 
        /// This highlighting theme should be configured such that it matches the
        /// themeing colors of the overall WPF theme that is defined in this object.
        /// </summary>
        public string HighlightingThemeName { get; private set; }

        /// <summary>
        /// Determines whether this theme is currently selected or not.
        /// </summary>
        public bool IsSelected
        {
            get { return _IsSelected; }

            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds additional resource file references into the existing theme definition.
        /// </summary>
        /// <param name="additionalResource"></param>
        public void AddResources(List<Uri> additionalResource)
        {
            foreach (var item in additionalResource)
                ThemeSources.Add(item);
        }
        #endregion methods
    }
}
