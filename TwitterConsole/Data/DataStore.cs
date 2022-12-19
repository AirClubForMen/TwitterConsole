using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterConsole.Data
{
    /// <summary>
    /// Since we are not saving the info nywhere I will use this as a place to put the tweet info
    /// </summary>
    public static class DataStore
    {
        public static int TotalTweets { get; set; } = 0;
        public static int TotalHashTags { get; set; } = 0;
        public static Dictionary<string,int> HashTagCounts { get; set; } = new();

        public static void ResetCounts()
        {
            HashTagCounts.Clear();
            TotalTweets = 0;
            TotalHashTags = 0;
        }
    }

}
