using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterConsole.Data
{
    /// <summary>
    /// Basic Data Store. Does not save to Disk or summarize. Only has the functionality to give the TopX tweets
    /// </summary>
    public class BasicDataStore:IDataStore
    {
        /// <summary>
        /// Reference to a logger
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Constructor with a logger attached
        /// </summary>
        /// <param name="logger"></param>
        public BasicDataStore(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Total Number of Tweets Read
        /// </summary>
        public int TotalTweets { get; set; } = 0;

        /// <summary>
        /// Total Number of Unique Hash Tags
        /// </summary>
        public int TotalHashTags { get; set; } = 0;

        /// <summary>
        /// Dictionary of Hash Tags with their total Counts
        /// </summary>
        private Dictionary<string,int> HashTagCounts { get; set; } = new();

        /// <summary>
        /// Reset Running counts
        /// </summary>
        public void ResetCounts()
        {
            HashTagCounts.Clear();
            TotalTweets = 0;
            TotalHashTags = 0;
        }


        /// <summary>
        /// Add Tweet - Add to Dictionary
        /// </summary>
        /// <param name="tweet"></param>
        public void AddTweet(Tweet tweet)
        {
            // would like to filter beforehand 
            TotalTweets += 1;

            // Make sure Entities and/or HashTags don't come in as null and there are some hashTags
            if (tweet.Entities == null || tweet.Entities.Hashtags == null || tweet.Entities.Hashtags.Count == 0) return;

            // increment counts and dictionaries
            foreach (var hashTag in tweet.Entities.Hashtags)
            {
                if (HashTagCounts.ContainsKey(hashTag.Tag))
                    HashTagCounts[hashTag.Tag] += 1;
                else
                    HashTagCounts.Add(hashTag.Tag, 1);

                TotalHashTags += 1;
            }

        }

        // <summary>
        /// Trim the list of running tweets. 
        /// </summary>
        /// <param name="threshold">When Dictionary of Tweets reaches this number</param>
        /// <param name="trimDownTo">Trim the list down to this many items</param>
        public void TrimTopXHashTags(int threshold, int trimDownTo)
        {
            // keep it small so it doesn;t get huge and slow things down
            if (HashTagCounts.Count > threshold)
            {
                HashTagCounts = new Dictionary<string, int>(HashTagCounts.OrderByDescending(x => x.Value).Take(trimDownTo));
                Logger.LogInformation("Cleaning up list of Hash Tag Counts");
            }
        }


        /// <summary>
        /// Get the current list of top hash tags. Implementation may vary greatly based on how Data is being stored.
        /// </summary>
        /// <param name="numberRequested">Top X most Popular</param>
        /// <returns>Dictionary with Hash Tags and Counts </returns>
        public Dictionary<string, int> GetTopXHashTags(int numberRequested)
        {
            return new Dictionary<string, int>(HashTagCounts.OrderByDescending(x => x.Value).Take(numberRequested));
        }

        /// <summary>
        /// Get a list of most popular hash tags since a certain time.
        /// </summary>
        /// <param name="numberRequested">Top X most Popular</param>
        /// <param name="since">Most Popular Since this date</param>
        /// <returns>Dictionary with Hash Tags and Counts</returns>
        public Dictionary<string, int> GetTopXHashTagsSince(int numberRequested, DateTime since)
        {
            // since we are not summarizing or saving the individual tweets we can't give this information.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a list of most popular hash tags in a time span.
        /// </summary>
        /// <param name="numberRequested">Top X most Popular</param>
        /// <param name="startTime">Date Range Start</param>
        /// <param name="endTime">Date Range End</param>
        /// <returns>Dictionary with Hash Tags and Counts</returns>
        public Dictionary<string, int> GetTopXHashTagsBetween(int numberRequested, DateTime startTime, DateTime endTime)
        {
            // since we are not summarizing or saving the individual tweets we can't give this information.
            throw new NotImplementedException();

        }


    }

}
