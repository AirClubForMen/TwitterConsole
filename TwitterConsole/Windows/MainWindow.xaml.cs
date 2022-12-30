// the only reference needed for ViewModels so we will keep it here
using TwitterConsole.Data;
using TwitterConsole.Listener;
using TwitterConsole.ViewModels;


namespace TwitterConsole.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Set up a logger to log errors. Normally it would be a LogAnalytics logger but for now we will log errors to the console.
        private ConsoleLogger logger = new();

        // Reference to a Basic Data Store
        private BasicDataStore dataStore;

        TwitterListener listener;

        #region Constructor

        /// <summary>
        /// Constructor - Set up the Data Context
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Set up the datastore
            dataStore=new BasicDataStore(logger);

            // Set up the View Model
            this.DataContext= new MainVM(logger, dataStore);

            // if Setup has not run yet go to it on startup
            if(String.IsNullOrEmpty(TwitterConsoleSettings.Default.TwitterApiKey))
            {
                OpenSettingsWindow();
            }

        }
        #endregion Constructor

        #region Private Variables

        // Set up a reference to the View Model
        private MainVM ViewModel { get { return this.DataContext as MainVM; } }

        /// <summary>
        /// Memnu selection to open the Settings Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenSettingsWindow();
        }

        /// <summary>
        /// Open the settings Window
        /// </summary>
        private void OpenSettingsWindow()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        #endregion Private Variables

        #region User Events

        /// <summary>
        /// Start and stop button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartStopHandler_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsListening)
                await ViewModel.StartListening();
            else
                ViewModel.StopListening();

        }



        #endregion


    }
}
