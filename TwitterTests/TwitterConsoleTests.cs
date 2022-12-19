

using TwitterConsole.Data;
using TwitterConsole.Listener;
using TwitterConsole.Logging;

namespace TwitterTests
{
    [TestClass]
    public class TwitterConsoleTests
    {
        /// <summary>
        /// Test cases to verify starting and stopping of the Service Busses 
        /// </summary>
        TwitterListener listener;

        [TestMethod]
        // just check the starting of a listener
        public async Task StartListener()
        {
            StartItUp();
            Thread.Sleep(2000);
            Assert.IsTrue(TwitterListener.KeepListenerAlive);
            listener.StopListener();
        }
        [TestMethod]
        // Start and stop a listener
        public async Task StartAndStopListener()
        {
            StartItUp();
            Thread.Sleep(2000);
            Assert.IsTrue(TwitterListener.KeepListenerAlive);
            listener.StopListener();
            Thread.Sleep(1000);
            Assert.IsFalse(TwitterListener.KeepListenerAlive);
        }

        [TestMethod]

        
        // make sure tweets are being collected
        public async Task CheckListeningData()
        {
            StartItUp();

            // give it 5 seconds to start up and collect data
            Thread.Sleep(5000);
            Assert.IsTrue(TwitterListener.KeepListenerAlive);
            Assert.IsTrue(DataStore.TotalTweets>0);

            listener.StopListener();
            Thread.Sleep(1000);
            Assert.IsFalse(TwitterListener.KeepListenerAlive);
        }

        public async void StartItUp()
        {
            var logger = new ConsoleLogger();
            var apiKey = "DdfG1vF2UD0LKYn7D7yIK6R0k";
            var apiSecret = "lFpIBIY4zobXxpwoxKHRM5CBuCAW3dOeO4dnCu7BVgCmDt1DLa";
            var token = "AAAAAAAAAAAAAAAAAAAAAOsxkgEAAAAAr8sfjNxLVURpjWjPINNILQOZlVU%3D0rIZkFvsnQG5eVtUmSQV13z9NNaQnHJu0RBnEV6ZzC9JdqTifG";
            listener = new TwitterListener(logger, apiKey, apiSecret, token);
            await listener.StartListener();
        }
    }
}