using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public class Logger : ILogger
    {
        protected const string LOG_DIRECTORY = "logs";
        protected const int LOG_DELETE_THRESHOLD_DAYS = 14;

        private readonly static object fileLock = new object();

        protected readonly IFileHandler fileHandler;
        protected readonly IPathController pathController;
        protected readonly string logFileName;

        public Logger(IFileHandler fileHandler, IPathController pathController)
        {
            this.fileHandler = fileHandler;
            this.pathController = pathController;
            logFileName = "log_" + DateTime.UtcNow.ToString("MM-dd-yyyy_HH-mm-ss-fff") + ".log";

            // TODO delete logs that are more than a few days old

            // TODO log system info like OS version, .NET version, framework version, etc
        }

        public void Log(string message)
        {
            Thread thread = new Thread(LogToFileThreadSafe);
            thread.Start(message);
        }

        private string LogFileRelativePath()
        {
            return pathController.Combine(LOG_DIRECTORY, logFileName);
        }

        private void LogToFileThreadSafe(object input)
        {
            string message = (string)input;
            message = "\nUTC " + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + message;
            lock (fileLock)
            {
                fileHandler.AppendFile(LogFileRelativePath(), Util.TextEncoding.GetBytes(message));
#if DEBUG
                Debug.WriteLine(message.Trim());
#endif
            }
        }
    }
}
