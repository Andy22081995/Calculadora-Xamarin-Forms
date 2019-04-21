namespace Calculator
{
    using Calculator.Controller;
    using Calculator.Views;
    using Xamarin.Forms;

    public partial class App : Application
    {
        private static DBItemController dbcontroller;
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new StartPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume() 
        {
            // Handle when your app resumes
        }

        public static DBItemController DbController
        {
            get
            {
                if (dbcontroller == null)
                {
                    dbcontroller = new DBItemController();
                }
                return dbcontroller;
            }
        }
    }
}
