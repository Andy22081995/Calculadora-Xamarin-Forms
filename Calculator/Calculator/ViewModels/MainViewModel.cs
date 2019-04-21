namespace Calculator.ViewModels
{
    public class MainViewModel
    {

        #region ViewModels
        public StartViewModel StartVM { get; set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _instance = this;
            this.StartVM = new StartViewModel();
        }
        #endregion

        #region Singleton
        private static MainViewModel _instance;

        public static MainViewModel GetInstance() => _instance ?? new MainViewModel();
        #endregion
    }
}
