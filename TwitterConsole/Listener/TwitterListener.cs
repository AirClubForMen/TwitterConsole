using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using TwitterConsole.Data;

namespace TwitterConsole.Listener
{
    /// <summary>
    /// Class to listent for messages on Twitter
    /// </summary>
    public class TwitterListener
    {
        #region Private Variables
        private static ILogger Logger;
        private string ApiKey = string.Empty;
        private string ApiSecret = string.Empty;
        private string BearerToken = string.Empty;
        private bool EnglishOnly = true;
        #endregion

        #region Delegates
        // set this up so we can stop the listener in the view model if there is an error in here
        public delegate void ListenerStopped();
        public event ListenerStopped ListenerStoppedEvent;
        #endregion

        #region Constructor

        /// <summary>
        /// Listener Constructor
        /// </summary>
        /// <param name="logger">Intance of a logger interface</param>
        /// <param name="apiKey">API Key</param>
        /// <param name="apiSecret">API Secret</param>
        /// <param name="bearerToken">Bearer token</param>
        /// <param name="englishOnly"></param>
        public TwitterListener(ILogger logger, string apiKey, string apiSecret, string bearerToken, bool englishOnly=true)
        {
            // assign the logger
            Logger = logger;
            this.ApiSecret = apiSecret;
            this.ApiKey = apiKey;
            this.BearerToken = bearerToken;
            this.EnglishOnly = englishOnly;
        }

        #endregion constructor

        #region Public Variables
            public static bool KeepListenerAlive = false;

        #endregion


        #region methods

        /// <summary>
        /// StopListener - Set the variable and let the listener do the rest
        /// </summary>
        public void StopListener()
        {
            KeepListenerAlive = false;
        }

        /// <summary>
        /// Start the listener
        /// </summary>
        /// <returns></returns>

        public async Task StartListener()
        {
            // set up the authorization
            var auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ApiKey,
                    ConsumerSecret = ApiSecret,

                },
                BearerToken = BearerToken
            };

            // Set up the Twitter Context
            var twitterCtx = new TwitterContext(auth);

            // set up some local variables
            int retries = 3;
            int count = 0;

            // set up a cancel token
            var cancelTokenSrc = new CancellationTokenSource();

            // Let the listener know it is active
            KeepListenerAlive = true;

            // loop to handle retries
            do
            {
                try
                { // set the Twitter Context to read a stream
                    await
                        (from strm in twitterCtx.Streaming
                                                .WithCancellation(cancelTokenSrc.Token)
                         where strm.Type == StreamingType.Sample &&
                               strm.TweetFields == TweetField.Entities

                         // performance takes a big hit if all fields are returned   
                         //strm.TweetFields == TweetField.AllFieldsExceptPermissioned 
                         select strm)
                        .StartAsync(async strm =>
                        {
                            await HandleStreamResponse(strm);
                            if (!KeepListenerAlive) cancelTokenSrc.Cancel();
                        });

                    retries = 0;
                }
                catch (IOException ex)
                {
                    // Twitter might have closed the stream,
                    // which they do sometimes. You should
                    // restart the stream, but be sure to
                    // read Twitter documentation on stream
                    // back-off strategies to prevent your
                    // app from being blocked.
                    Logger.LogError(ex.ToString());
                    retries--;
                }
                catch (OperationCanceledException)
                {
                    // something happened, Cancel out and notify the vm that something listener is stopped
                    Logger.LogError("Stream cancelled.");
                    retries = 0;
                    KeepListenerAlive = false;
                }
                catch (TwitterQueryException tqe) when (tqe.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    // This implementation was copied from the samples that the user had
                    int millisecondsToDelay = 1000 * (4 - retries);
                    retries--;

                    string message = retries >= 0 ?
                        $"Tried to reconnect too quickly. Delaying for {millisecondsToDelay} milliseconds..."
                        :
                        "Too many retries. Stopping query.";

                    // log an error
                    Logger.LogError(message);
                    if (retries == 0)
                    {
                        KeepListenerAlive = false;
                        continue;
                    }

                    // wait for a couple of seconds and retry
                    await Task.Delay(millisecondsToDelay);
                }
            } while (KeepListenerAlive);

            // fire off a stopped event back to the view model.

            if (ListenerStoppedEvent != null) ListenerStoppedEvent();
        }

        /// <summary>
        /// Deserialize the sctream and update the counters
        /// </summary>
        /// <param name="strm">Stream of the tweat read in</param>
        /// <returns></returns>
        async Task HandleStreamResponse(StreamContent strm)
        {
            if (strm.HasError)
            {
                Logger.LogError($"Error during streaming: {strm.ErrorMessage}");
                return;
            }
            //var content = strm.Content;
            Tweet? tweet = strm?.Entity?.Tweet;
            if (tweet == null)
            {
                Logger.LogError($"\nNull Tweet");
                return;
            }
            // would like to filter beforehand 
            //if (EnglishOnly && tweet.Language != "en")
            //    return;
            DataStore.TotalTweets += 1;

            // Make sure Entities and/or HashTags don't come in as null and there are some hashTags
            if (tweet.Entities == null || tweet.Entities.Hashtags == null || tweet.Entities.Hashtags.Count == 0) return;

            // increment counts and dictionaries
            foreach (var hashTag in tweet.Entities.Hashtags)
            {
                if (DataStore.HashTagCounts.ContainsKey(hashTag.Tag))
                    DataStore.HashTagCounts[hashTag.Tag] += 1;
                else
                    DataStore.HashTagCounts.Add(hashTag.Tag,1);
                
                DataStore.TotalHashTags += 1;
            }

            return;
        }
#endregion Methods
    }

}
