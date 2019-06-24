namespace AehnlichViewLib.Themes
{
    using System.Windows;

    public static class ResourceKeys
    {
        #region Accent Keys
        /// <summary>
        /// Accent Color Key - This Color key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

        /// <summary>
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys

        #region Brush Keys
        /// <summary>
        /// Styles the thumb of the vertical color spectrum slider when mouse is over.
        /// </summary>
        public static readonly ComponentResourceKey VerticalSLideThumbMouseOverBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "VerticalSLideThumbMouseOverBrushKey");

        public static readonly ComponentResourceKey VerticalSLideThumbMouseOverBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "VerticalSLideThumbMouseOverBorderBrushKey");

        public static readonly ComponentResourceKey VerticalSLideThumbBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "VerticalSLideThumbBorderBrushKey");

        /// <summary>
        /// Styles the foreground color of the thumb of the vertical color spectrum slider.
        /// </summary>
        public static readonly ComponentResourceKey VerticalSLideThumbForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "VerticalSLideThumbForegroundBrushKey");

        public static readonly ComponentResourceKey ButtonNormalOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonNormalOuterBorderKey");

        public static readonly ComponentResourceKey ControlNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundKey");
        public static readonly ComponentResourceKey ControlNormalForegroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundKey");
        public static readonly ComponentResourceKey ControlNormalForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundBrushKey");
        public static readonly ComponentResourceKey ControlNormalBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundBrushKey");
        #endregion Brush Keys

        #region TextEditor BrushKeys
        public static readonly ComponentResourceKey EditorBackground = new ComponentResourceKey(typeof(ResourceKeys), "EditorBackground");
        public static readonly ComponentResourceKey EditorForeground = new ComponentResourceKey(typeof(ResourceKeys), "EditorForeground");
        public static readonly ComponentResourceKey EditorLineNumbersForeground = new ComponentResourceKey(typeof(ResourceKeys), "EditorLineNumbersForeground");
        public static readonly ComponentResourceKey EditorSelectionBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorSelectionBrush");
        public static readonly ComponentResourceKey EditorSelectionBorder = new ComponentResourceKey(typeof(ResourceKeys), "EditorSelectionBorder");
        public static readonly ComponentResourceKey EditorNonPrintableCharacterBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorNonPrintableCharacterBrush");
        public static readonly ComponentResourceKey EditorLinkTextForegroundBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorLinkTextForegroundBrush");
        public static readonly ComponentResourceKey EditorLinkTextBackgroundBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorLinkTextBackgroundBrush");
        #endregion TextEditor BrushKeys

        #region DiffView Currentline Keys
        /// <summary>
        /// Gets the background color for highlighting for the currently highlighed line.
        /// </summary>
        public static readonly ComponentResourceKey EditorCurrentLineBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "EditorCurrentLineBackgroundBrushKey");

        /// <summary>
        /// Gets the border color for highlighting for the currently highlighed line.
        /// </summary>
        public static readonly ComponentResourceKey EditorCurrentLineBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "EditorCurrentLineBorderBrushKey");

        /// <summary>
        /// Gets the border thickness for highlighting for the currently highlighed line.
        /// </summary>
        public static readonly ComponentResourceKey EditorCurrentLineBorderThicknessKey = new ComponentResourceKey(typeof(ResourceKeys), "EditorCurrentLineBorderThicknessKey");
        #endregion DiffView Currentline Keys

        #region ICONs

        public static readonly ComponentResourceKey ICON_DeletedKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_DeletedKey");
        public static readonly ComponentResourceKey ICON_AddedKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_AddedKey");
        public static readonly ComponentResourceKey ICON_ChangedKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ChangedKey");

        /// <summary>
        /// Defines the icon that is shown for an Open File UI function (eg: Image on Button).
        /// </summary>
        public static readonly ComponentResourceKey ICON_OpenFileKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_OpenFileKey");

        /// <summary>
        /// Defines the icon that is shown for a copy text selection UI function (eg: Image on Button).
        /// </summary>
        public static readonly ComponentResourceKey ICON_CopyKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_CopyKey");

        public static readonly ComponentResourceKey ICON_FindKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_FindKey");
        public static readonly ComponentResourceKey ICON_NextKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_NextKey");
        public static readonly ComponentResourceKey ICON_PreviousKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_PreviousKey");

        /// <summary>
        /// Defines the icon that is shown for a refresh UI function (eg: Image on Button).
        /// </summary>
        public static readonly ComponentResourceKey ICON_RefreshKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_RefreshKey");

        #region Goto Diff Icons
        public static readonly ComponentResourceKey ICON_GotoTopKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoTopKey");
        public static readonly ComponentResourceKey ICON_GotoNextKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoNextKey");
        public static readonly ComponentResourceKey ICON_GotoPrevKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoPrevKey");
        public static readonly ComponentResourceKey ICON_GotoBottomKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoBottomKey");
        #endregion Goto Diff Icons

        public static readonly ComponentResourceKey ICON_CloseKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_CloseKey");

        public static readonly ComponentResourceKey ICON_GotoLineKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoLineKey");

        #region Arrow Icons
        public static readonly ComponentResourceKey ICON_ArrowUpKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ArrowUpKey");
        public static readonly ComponentResourceKey ICON_ArrowDownKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ArrowDownKey");
        public static readonly ComponentResourceKey ICON_ArrowLeftKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ArrowLeftKey");
        public static readonly ComponentResourceKey ICON_ArrowRightKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ArrowRightKey");
        #endregion Arrow Icons

        #region Browse Folder Icons
        public static readonly ComponentResourceKey ICON_FolderUpKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_FolderUpKey");
        public static readonly ComponentResourceKey ICON_FolderDownKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_FolderDownKey");
        #endregion Browse Folder Icons

        /// <summary>
        /// Interactive option in AvalonEdit based view for switching highlighting ON/OFF
        /// </summary>
        public static readonly ComponentResourceKey ICON_HighlightingKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_HighlightingKey");

        public static readonly ComponentResourceKey ICON_SettingsKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_SettingsKey");
        #endregion ICONs

        #region Diff Colors
        #region Background
        public static readonly ComponentResourceKey ColorBackgroundContextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundContextBrushKey");
        public static readonly ComponentResourceKey ColorBackgroundDeletedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundDeletedBrushKey");
        public static readonly ComponentResourceKey ColorBackgroundAddedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundAddedBrushKey");
        #endregion Background

        #region Foreground
        public static readonly ComponentResourceKey ColorForegroundContextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorForegroundContextBrushKey");
        public static readonly ComponentResourceKey ColorForegroundDeletedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorForegroundDeletedBrushKey");
        public static readonly ComponentResourceKey ColorForegroundAddedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorForegroundAddedBrushKey");
        #endregion Foreground

        public static readonly ComponentResourceKey ColorBackgroundImaginaryDeletedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundImaginaryDeletedBrushKey");
        public static readonly ComponentResourceKey ColorBackgroundImaginaryAddedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundImaginaryAddedBrushKey");
        #endregion Diff Colors

        #region Grid Theming
        #region Grid Styles
        /// <summary>
        /// Gets the default style to be applied for the data grid.
        /// </summary>
        public static readonly ComponentResourceKey DefaultDataGridStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "DefaultDataGridStyleKey");

        /// <summary>
        /// Gets the default style to be applied for the header column gripper
        /// which can be used to resize a column.
        /// </summary>
        public static readonly ComponentResourceKey DefaultColumnHeaderGripperStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "DefaultColumnHeaderGripperStyleKey");

        /// <summary>
        /// Gets the default style to be applied for data grid header cells.
        /// </summary>
        public static readonly ComponentResourceKey DefaultDataGridHeaderStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "DefaultDataGridHeaderStyleKey");

        /// <summary>
        /// Gets the default style to be applied for data grid cells.
        /// </summary>
        public static readonly ComponentResourceKey DefaultDataGridCellStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "DefaultDataGridCellStyleKey");
        #endregion Grid Styles

        #region DataGridHeader Keys
        /// <summary>
        /// Gets the Brush key of the border color
        /// of the vertical Data Grid Header line between each header item.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderBorderBrushKey");

        /// <summary>
        /// Gets the background color of the datagrid header row.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderBackgroundBrushKey");

        /// <summary>
        /// Gets the foreground color of the datagrid header row.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderForegroundBrushKey");

        /// <summary>
        /// Gets the foreground color of the sort arrow display in the datagrid header row.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderSortArrowForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderSortArrowForegroundBrushKey");

        /// <summary>
        /// Gets the FontFamily to be used for text displays inside the DataGrid control.
        /// <seealso cref="DefaultFontFamily"/>
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderFontFamily = new ComponentResourceKey(typeof(ResourceKeys), "DefaultFontFamily");

        /// <summary>
        /// Gets the font size of the text displayed in the DataGrid header row.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderFontSize = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderFontSize");
        #endregion DataGridHeader Keys

        public static readonly ComponentResourceKey DataGridBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridBorderBrushKey");

        /// <summary>
        /// Gets the Brush key of the border color
        /// between each column in the data display area of the DataGrid.
        /// </summary>
        public static readonly ComponentResourceKey VerticalGridLinesBrush = new ComponentResourceKey(typeof(ResourceKeys), "VerticalGridLinesBrush");

        /// <summary>
        /// Gets the Brush key of the border color
        /// between each row in the data display area of the DataGrid.
        /// </summary>
        public static readonly ComponentResourceKey HorizontalGridLinesBrush = new ComponentResourceKey(typeof(ResourceKeys), "HorizontalGridLinesBrush");

        public static readonly ComponentResourceKey ControlBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlBorderBrushKey");

        public static readonly ComponentResourceKey CellForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "CellForegroundBrushKey");
        public static readonly ComponentResourceKey CellBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "CellBackgroundBrushKey");

        /// <summary>
        /// Gets the foreground brush key of a datagrid cell that is selected and focused.
        /// </summary>
        public static readonly ComponentResourceKey SelectedFocusedCellForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedFocusedCellForegroundBrushKey");

        /// <summary>
        /// Gets the background brush key of a datagrid cell that is selected and focused.
        /// </summary>
        public static readonly ComponentResourceKey SelectedFocusedCellBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedFocusedCellBackgroundBrushKey");

        /// <summary>
        /// Gets the foreground brush key of a datagrid cell that is selected but not focused.
        /// </summary>
        public static readonly ComponentResourceKey SelectedCellForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedCellForegroundBrushKey");

        /// <summary>
        /// Gets the background brush key of a datagrid cell that is selected but not focused.
        /// </summary>
        public static readonly ComponentResourceKey SelectedCellBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedCellBackgroundBrushKey");

        public static readonly ComponentResourceKey CellBorderBrushKey                = new ComponentResourceKey(typeof(ResourceKeys), "CellBorderBrushKey");
        public static readonly ComponentResourceKey SelectedCellBorderBrushKey        = new ComponentResourceKey(typeof(ResourceKeys), "SelectedCellBorderBrushKey");
        public static readonly ComponentResourceKey SelectedFocusedCellBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedFocusedCellBorderBrushKey");
        #endregion Grid Theming

        /***
                public static readonly ComponentResourceKey ControlDisabledBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlDisabledBackgroundKey");
                public static readonly ComponentResourceKey ControlNormalBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBorderKey");
                public static readonly ComponentResourceKey ControlMouseOverBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlMouseOverBorderKey");

                public static readonly ComponentResourceKey ControlSelectedBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlSelectedBorderKey");
                public static readonly ComponentResourceKey ControlFocusedBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlFocusedBorderKey");

                public static readonly ComponentResourceKey ButtonNormalInnerBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonNormalInnerBorderKey");
                public static readonly ComponentResourceKey ButtonNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonNormalBackgroundKey");

                public static readonly ComponentResourceKey ButtonMouseOverBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonMouseOverBackgroundKey");
                public static readonly ComponentResourceKey ButtonMouseOverOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonMouseOverOuterBorderKey");
                public static readonly ComponentResourceKey ButtonMouseOverInnerBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonMouseOverInnerBorderKey");

                public static readonly ComponentResourceKey ButtonPressedOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonPressedOuterBorderKey");
                public static readonly ComponentResourceKey ButtonPressedInnerBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonPressedInnerBorderKey");
                public static readonly ComponentResourceKey ButtonPressedBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonPressedBackgroundKey");

                public static readonly ComponentResourceKey ButtonFocusedOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonFocusedOuterBorderKey");
                public static readonly ComponentResourceKey ButtonFocusedInnerBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonFocusedInnerBorderKey");
                public static readonly ComponentResourceKey ButtonFocusedBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonFocusedBackgroundKey");

                public static readonly ComponentResourceKey ButtonDisabledOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonDisabledOuterBorderKey");
                public static readonly ComponentResourceKey ButtonInnerBorderDisabledKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonInnerBorderDisabledKey");

                public static readonly ComponentResourceKey PanelBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "PanelBackgroundBrushKey");

                #endregion //Brush Keys

                public static readonly ComponentResourceKey GlyphNormalForegroundKey = new ComponentResourceKey(typeof(ResourceKeys), "GlyphNormalForegroundKey");
                public static readonly ComponentResourceKey GlyphDisabledForegroundKey = new ComponentResourceKey(typeof(ResourceKeys), "GlyphDisabledForegroundKey");

                public static readonly ComponentResourceKey SpinButtonCornerRadiusKey = new ComponentResourceKey(typeof(ResourceKeys), "SpinButtonCornerRadiusKey");

                public static readonly ComponentResourceKey SpinnerButtonStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "SpinnerButtonStyleKey");

                /// <summary>
                /// Styles the border of a PopUp Control when it is open.
                /// </summary>
                public static readonly ComponentResourceKey PopUpOpenBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "PopUpOpenBorderBrushKey");

                /// <summary>
                /// Styles the foreground color of the color canvas thumb of the color slider.
                /// </summary>
                public static readonly ComponentResourceKey CanvasSLideThumbForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "CanvasSLideThumbForegroundBrushKey");

                /// <summary>
                /// Styles the border color of the color canvas thumb of the color slider.
                /// </summary>
                public static readonly ComponentResourceKey CanvasSLideThumbBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "CanvasSLideThumbBorderBrushKey");
        ***/
    }
}
