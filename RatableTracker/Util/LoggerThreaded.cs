using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RatableTracker.Util
{
    public class LoggerThreaded : ILogger
    {
        public const string LOG_DIRECTORY = "logs";
        protected const int LOG_DELETE_THRESHOLD_DAYS = 14;

        private readonly static object fileLock = new object();

        private int logCounter = 0;

        protected readonly IFileHandler fileHandler;
        protected readonly string logFileName;

        public LoggerThreaded(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
            logFileName = "log_" + DateTime.UtcNow.ToString("MM-dd-yyyy_HH-mm-ss") + ".log";

            LogSystemInfoOnLoggerStart();

            // delete logs that are more than the specified number of days old
            IList<FileInfo> files = this.fileHandler.GetFilesInCurrentDirectory();
            Log("Checking for old log files over " + LOG_DELETE_THRESHOLD_DAYS.ToString() + " days old - " + files.Count.ToString() + " total files found");
            foreach (FileInfo file in files)
            {
                if (file.CreatedOnUTC <= DateTime.UtcNow.AddDays(-1 * LOG_DELETE_THRESHOLD_DAYS))
                {
                    Log("Deleting old log file: " + file.Name + " - creation date " + file.CreatedOnUTC.ToString("G") + " UTC");
                    this.fileHandler.DeleteFile(file.Name);
                }
            }
        }

        protected virtual void LogSystemInfoOnLoggerStart()
        {
            Log("OS INFO - " + Environment.OSVersion.VersionString + "\nCLR INFO - " + Environment.Version.ToString() + "\nFRAMEWORK VERSION - " + Util.FrameworkVersion.ToString(), false);
        }

        public void Log(string message)
        {
            Log(message, true);
        }

        private void Log(string message, bool newlineAtStart)
        {
            Thread thread = new Thread(() => LogToFileThreadSafe((++logCounter).ToString("D5") + " - " + message, !newlineAtStart));
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

        public string MostRecentLogs()
        {
            byte[] fileContent;
            lock (fileLock)
            {
                fileContent = fileHandler.LoadFile(logFileName);
            }
            return Util.TextEncoding.GetString(fileContent);
        }
    }
}
