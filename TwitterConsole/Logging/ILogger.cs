using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterConsole.Logging
{
    /// <summary>
    /// Interface for the logging. It would normally be a log analytics logger  
    /// </summary>
    public interface ILogger
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogInformation(string message);

    }
}
