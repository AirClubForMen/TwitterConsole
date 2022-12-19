
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using TwitterConsole.Data;
using TwitterConsole.Listener;
using TwitterConsole.Logging;
using static TwitterConsole.Listener.TwitterListener;

namespace TwitterConsole.ViewModels
{
    public class MainVM : ViewModelBase
    {
        #region Local Variables
        // Use ILogger and allow the main window to determine the implementation 
        ILogger Logger { get; set; }

        // reference to the listener
        TwitterListener listener;

        #endregion

        #region Setup
        /// <summary>
        /// Constructor with the logger passes in
        /// </summary>
        /// <param name="logger"></param>
        public MainVM(ILogger logger)
        {
            this.Logger = logger;
        }

        #endregion

        #region  methods

        /// <summary>
        /// Start the listener
        /// </summary>
        /// <returns></returns>
        public async Task StartListening()
        {
            // Log to whatever logger is currently being used
            Logger.LogInformation("Starting Listener");

            // Set up the listener with the Keys, 
            listener = new TwitterListener(Logger, TwitterConsoleSettings.Default.TwitterApiKey, TwitterConsoleSettings.Default.TwitterApiSecret, TwitterConsoleSettings.Default.TwitterBearerToken);

            // Need a way to call back from the listenere if something errors out
            listener.ListenerStoppedEvent += ListenerStopped;

            // Set up initial variables
            StartTime = DateTime.Now;
            IsListening = true;

            // reset the counts in the data store
            DataStore.ResetCounts();

            // start the screen refresher, async void will run in its own thread
            ScreenRefresher();

            // start the listener asyncronously
            await listener.StartListener();
        }

        /// <summary>
        /// Stop the listener, force the listener to stop, will send back the Listener Stopped event when done
        /// </summary>
        public void StopListening()
        {
            listener.StopListener();
        }

        /// <summary>
        /// Delegate that gets fired when a listenter stops
        /// </summary>
        private void ListenerStopped()
        {
            Logger.LogInformation("Stopping Listener");

            // make sure to unhook the event or it will fire twice on a restart
            listener.ListenerStoppedEvent -= ListenerStopped;

            // let this class know w are no longer listening
            IsListening = false;
        }

        /// <summary>
        /// Method to refresh the scren every 2 seconds
        /// </summary>
        /// 
        int refreshTime = 2;
        private async void ScreenRefresher()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(refreshTime));

            while (await timer.WaitForNextTickAsync())
            {
                // refresh the values that have changed
                Refresh();
                // keep it small so it doesn;t get huge and slow things down
                if (DataStore.HashTagCounts.Count > 1000)
                {
                    DataStore.HashTagCounts = new Dictionary<string, int>(DataStore.HashTagCounts.OrderByDescending(x => x.Value).Take(100));
                    Logger.LogInformation("Cleaning up list of Hash Tag Counts");
                }
                // stop refreshing and leave
                if(!IsListening)
                    {timer.Dispose(); return; }
            }
        }

        /// <summary>
        /// Refresh the values that need refreshing
        /// </summary>
        public void Refresh()
        {
            PropChanged("TotalTweets");
            PropChanged("TotalHashTags");
            PropChanged("TweetsPerSecond");
            PropChanged("RunningTimeString");
            PropChanged("RankedHashTags");

        }
        #endregion Methods


        #region public Variables

        /// <summary>
        /// Are you currently listening, if so update the screen
        /// </summary>
        public bool IsListening
        {
            get { return listening; }
            set
            {
                listening = value;
                PropChanged("StartStopText");
                PropChanged("CurrentStatus");
                PropChanged("StartTimeString");
                PropChanged("RunningTimeString");
            }
        }
        bool listening = false;

        /// <summary>
        /// Text on the Start/Stop button
        /// </summary>
        
        public string StartStopText
        {
            get { return IsListening ? "Stop Listener" : "Start Listener"; }
        }
        /// <summary>
        /// Current status in the textBox
        /// </summary>
        public string CurrentStatus { get { return IsListening ? "Started" : "Stopped"; } }

        /// <summary>
        /// Time Listener was Started
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public string StartTimeString { get { return StartTime.ToShortTimeString(); } }

        /// <summary>
        ///  Running Time
        /// </summary>
        public int RunningTimeString
        {
            get
            {
                if (StartTime == DateTime.MinValue) return 0;
                return (int) (DateTime.Now - StartTime).TotalSeconds;
            }
        }

       /// <summary>
       /// Total Number of Tweets
       /// </summary>
        public int TotalTweets
        {
            get { return DataStore.TotalTweets; }
        }
        /// <summary>
        /// Total Number of HashTags
        /// </summary>
        public int TotalHashTags
        {
            get { return DataStore.TotalHashTags; }
        }

        /// <summary>
        /// Tweets per Second
        /// </summary>
        public string TweetsPerSecond
        {
            get
            {
                if (StartTime == DateTime.MinValue) return "";
                var secondsRunning = (DateTime.Now - StartTime).TotalSeconds;
                if (secondsRunning == 0) return "";
                var perSecond = TotalTweets / secondsRunning;
                return perSecond.ToString("n2");
            }

        }

        /// <summary>
        /// List of Ranked Hash Tags
        /// </summary>
        public ObservableCollection<HashTagRanking> RankedHashTags
        {
            get
            {
                var rankedTweets = new List<HashTagRanking>();
                int ranking = 1;

                // grab the top 10 and put them in the list
                foreach (var hashTag in DataStore.HashTagCounts.OrderByDescending(x => x.Value).Take(10))
                {
                    rankedTweets.Add(new HashTagRanking() { HashTagIndex = ranking++, HashTagValue = hashTag.Key, HashTagCount = hashTag.Value });
                }
                return new ObservableCollection<HashTagRanking>(rankedTweets);
            }

        }

        /// <summary>
        /// Class that holds the ranking list
        /// </summary>
        public class HashTagRanking
        {
            public int HashTagIndex { get; set; }
            public string HashTagValue { get; set; }
            public int HashTagCount { get; set; }
        }
        #endregion public properties

    }
}
