namespace Aehnlich.Views
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for DirDiffView.xaml
    /// </summary>
    public partial class DirDiffView : UserControl
    {
        public DirDiffView()
        {
            InitializeComponent();
        }

        #region Column Width A B Synchronization
        private void MainSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            DiffDir.ColumnWidthA = this.TopColumnA.Width;
            DiffDir.ColumnWidthB = this.TopColumnB.Width;
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            DiffDir.ColumnWidthA = this.TopColumnA.Width;
            DiffDir.ColumnWidthB = this.TopColumnB.Width;
        }

        private void DiffDir_ColumnWidthChanged(object sender, AehnlichViewLib.Events.ColumnWidthChangedEvent e)
        {
            this.TopColumnA.Width = e.ColumnWidthA;
            this.TopColumnB.Width = e.ColumnWidthB;
        }
        #endregion Column Width A B Synchronization
    }
}
