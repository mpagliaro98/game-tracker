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
        public const string LOG_DIRECTORY = "logs";
        protected const int LOG_DELETE_THRESHOLD_DAYS = 14;

        private readonly static object fileLock = new object();

        protected readonly IFileHandler fileHandler;
        protected readonly string logFileName;

        public Logger(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
            logFileName = "log_" + DateTime.UtcNow.ToString("MM-dd-yyyy_HH-mm-ss-fff") + ".log";

            LogSystemInfoOnLoggerStart();

            // delete logs that are more than the specified number of days old
            foreach (FileInfo file in this.fileHandler.GetFilesInCurrentDirectory(this))
            {
                if (file.CreatedOnUTC <= DateTime.UtcNow.AddDays(-1 * LOG_DELETE_THRESHOLD_DAYS))
                {
                    Log("Deleting old log file: " + file.Name + " - creation date " + file.CreatedOnUTC.ToString("G") + " UTC over " + LOG_DELETE_THRESHOLD_DAYS.ToString() + " days old");
                    this.fileHandler.DeleteFile(file.Name, this);
                }
            }
        }

        protected virtual void LogSystemInfoOnLoggerStart()
        {
            Log("OS INFO - " + Environment.OSVersion.VersionString + "\nCLR INFO - " + Environment.Version.ToString() + "\nFRAMEWORK INFO - " + Util.FrameworkVersion.ToString(), false);
        }

        public void Log(string message)
        {
            Thread thread = new Thread(() => LogToFileThreadSafe(message, false));
            thread.Start();
        }

        private void Log(string message, bool newlineAtStart)
        {
            Thread thread = new Thread(() => LogToFileThreadSafe(message, !newlineAtStart));
            thread.Start();
        }

        private void LogToFileThreadSafe(string input, bool firstLine)
        {
            input = (!firstLine ? "\n" : "") + "UTC " + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + input;
            lock (fileLock)
            {
                fileHandler.AppendFile(logFileName, Util.TextEncoding.GetBytes(input));
#if DEBUG
                Debug.WriteLine(input.Trim());
#endif
            }
        }
    }
}
