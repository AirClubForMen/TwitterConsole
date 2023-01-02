using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterConsole.Data
{
    public interface IDataStore
    {
        /// <summary>
        /// Set up a logger 
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Total Number of Tweets Read
        /// </summary>
        int TotalTweets { get; set; }

        /// <summary>
        /// Total Number of Unique Hash Tags
        /// </summary>
        int TotalHashTags { get; set; }

        /// <summary>
        /// Add Hash Tags for a tweet
        /// </summary>
        /// <param name="hashTags">List of HashTags for a tweet</param>
        void AddHashTags(List<string> hashTags);

        /// <summary>
        /// Reset Running Counts
        /// </summary>
        void ResetCounts();

        /// <summary>
        /// Trim the list of running tweets. 
        /// </summary>
        /// <param name="threshold">When Dictionary of Tweets reaches this number</param>
        /// <param name="trimDownTo">Trim the list down to this many items</param>
        void TrimTopXHashTags(int threshold, int trimDownTo);

        /// <summary>
        /// Get the current list of top hash tags. Implementation may vary greatly based on how Data is being stored.
        /// </summary>
        /// <param name="numberRequested">Top X most Popular</param>
        /// <returns>Dictionary with Hash Tags and Counts </returns>
        Dictionary<string, int> GetTopXHashTags(int numberRequested);

        /// <summary>
        /// Get a list of most popular hash tags since a certain time.
        /// </summary>
        /// <param name="numberRequested">Top X most Popular</param>
        /// <param name="since">Most Popular Since this date</param>
        /// <returns>Dictionary with Hash Tags and Counts</returns>
        Dictionary<string, int> GetTopXHashTagsSince(int numberRequested, DateTime since);

        /// <summary>
        /// Get a list of most popular hash tags in a time span.
        /// </summary>
        /// <param name="numberRequested">Top X most Popular</param>
        /// <param name="startTime">Date Range Start</param>
        /// <param name="endTime">Date Range End</param>
        /// <returns>Dictionary with Hash Tags and Counts</returns>
        Dictionary<string, int> GetTopXHashTagsBetween(int numberRequested, DateTime startTime, DateTime endTime);
    }
}
