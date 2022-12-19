using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TwitterConsole.Utilities;

namespace TwitterConsole.ViewModels
{
    public class SettingsVM: ViewModelBase
    { 
        /// <summary>
        /// Set up the access keys, The only one that really matters is the bearer token
        /// </summary>
        public string TwitterApiKey { get { return TwitterConsoleSettings.Default.TwitterApiKey; } set { TwitterConsoleSettings.Default.TwitterApiKey = value; PropChanged(); } }
        public string TwitterApiSecret { get { return TwitterConsoleSettings.Default.TwitterApiSecret; } set { TwitterConsoleSettings.Default.TwitterApiSecret = value; PropChanged(); } }
        public string TwitterBearerToken { get { return TwitterConsoleSettings.Default.TwitterBearerToken; } set { TwitterConsoleSettings.Default.TwitterBearerToken = value; PropChanged(); } }

        /// <summary>
        /// Call to Save the keys
        /// </summary>
        public ICommand SaveApiKeyCommand
        {
            get
            {
                return new DelegateCommand(p => true, SaveApiKeys);
            }
        }
        
        /// <summary>
        /// SAve the keys
        /// </summary>
        /// <param name="obj"></param>
        private void SaveApiKeys(object obj)
        {
            TwitterConsoleSettings.Default.Save();
        }
    }
}
