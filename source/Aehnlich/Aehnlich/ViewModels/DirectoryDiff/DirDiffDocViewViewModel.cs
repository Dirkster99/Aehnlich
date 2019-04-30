namespace Aehnlich.ViewModels.Documents
{
    internal class DirDiffDocViewViewModel : Base.ViewModelBase
    {
        #region fields
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirDiffDocViewViewModel()
        {
            DirDiffDoc = AehnlichDirViewModelLib.ViewModels.Factory.ConstructAppViewModel();
        }
        #endregion ctors

        #region properties
        public AehnlichDirViewModelLib.Interfaces.IAppViewModel DirDiffDoc { get; }
        #endregion properties
        
        #region methods
        public void Initilize(string leftDirPath, string rightDirPath)
        {
            DirDiffDoc.Initialize(leftDirPath, rightDirPath);
        }
        #endregion methods
    }
}
