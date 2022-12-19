using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterConsole.Logging
{
    /// <summary>
    /// Console implementation of the ILogger Interface. 
    /// </summary>
    public class ConsoleLogger:ILogger
    {
        public ConsoleLogger() { } 
        public void LogError(string message) {
            Console.Write("Error - " + message+"\n");
        }
        public void LogWarning(string message)
        {
            Console.Write("Warning - " + message + "\n");
        }
        public void LogInformation(string message)
        {
            Console.Write("Information - " + message + "\n");
        }
    }
}
