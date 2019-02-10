namespace DiffViewLib
{
    using DiffViewLib.Enums;
    using ICSharpCode.AvalonEdit;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// 
    /// </summary>
    public class DiffView : TextEditor
    {
        #region fields
        public static readonly DependencyProperty LineDiffsProperty =
            DependencyProperty.Register("LineDiffs", typeof(Dictionary<int, DiffContext>),
                typeof(DiffView), new PropertyMetadata(null));

        private readonly DiffLineBackgroundRenderer2 _DiffBackgroundRenderer;
        #endregion fields

        #region ctors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static DiffView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DiffView),
                new FrameworkPropertyMetadata(typeof(DiffView)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public DiffView()
            : base()
        {
            _DiffBackgroundRenderer = new DiffLineBackgroundRenderer2(this);
            this.TextArea.TextView.BackgroundRenderers.Add(_DiffBackgroundRenderer);
        }
        #endregion ctors

        #region methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        #endregion methods

        #region properties
        public Dictionary<int, DiffContext> LineDiffs
        {
            get { return (Dictionary<int, DiffContext>)GetValue(LineDiffsProperty); }
            set { SetValue(LineDiffsProperty, value); }
        }
        #endregion properties
    }
}
