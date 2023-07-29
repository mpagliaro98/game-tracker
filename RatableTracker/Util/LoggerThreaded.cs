using RatableTracker.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RatableTracker.Util
{
    public class LoggerThreaded : ILogger
    {
        public const string LOG_DIRECTORY = "logs";
        protected const int LOG_DELETE_THRESHOLD_DAYS = 14;

        private readonly static object fileLock = new();

        private int logCounter = 0;

        private readonly BlockingCollection<string> logQueue = new BlockingCollection<string>();
        private Task logTask;

        protected readonly IFileHandler fileHandler;
        protected readonly string logFileName;

        public LoggerThreaded(IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
            logFileName = "log_" + DateTime.UtcNow.ToString("MM-dd-yyyy_HH-mm-ss") + ".log";

            InitLogThread();
            LogSystemInfoOnLoggerStart();
            DeleteOldLogFiles();
        }

        private void InitLogThread()
        {
            logTask = Task.Run(LogProcess);
            Debug.WriteLine("Logger started with task ID " + logTask.Id.ToString());
        }

        public void Dispose()
        {
            Debug.WriteLine("LoggerThreaded dispose");

            // tell the queue to finish, wait for the thread to stop
            logQueue.CompleteAdding();
            logTask.GetAwaiter().GetResult();

            // clean up the queue
            logQueue.Dispose();
            Debug.WriteLine("LoggerThreaded disposal completed");
        }

        protected virtual void LogSystemInfoOnLoggerStart()
        {
            try
            {
                var fwAssembly = Assembly.GetExecutingAssembly();
                Log("\nOS INFO - " + Environment.OSVersion.VersionString + "\nCLR INFO - " + Environment.Version.ToString() + "\nFRAMEWORK - " + fwAssembly.FullName, false);
            }
            catch (Exception ex)
            {
                Log("Unable to log Ratable Tracker version info due to an error: " + ex.GetType().Name + " - " + ex.Message);
            }
        }

        private void DeleteOldLogFiles()
        {
            // delete logs that are more than the specified number of days old
            IList<FileInfo> files = fileHandler.GetFilesInCurrentDirectory();
            Log("Checking for old log files over " + LOG_DELETE_THRESHOLD_DAYS.ToString() + " days old - " + files.Count.ToString() + " total files in directory");
            foreach (FileInfo file in files)
            {
                if (file.CreatedOnUTC <= DateTime.UtcNow.AddDays(-1 * LOG_DELETE_THRESHOLD_DAYS))
                {
                    Log("Deleting old log file: " + file.Name + " - creation date " + file.CreatedOnUTC.ToString("G") + " UTC");
                    fileHandler.DeleteFile(file.Name);
                }
            }
        }

        public void Log(string message)
        {
            Log(message, true);
        }

        private void Log(string message, bool newlineAtStart)
        {
            string input = (newlineAtStart ? "\n" : "") + "[UTC " + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + (++logCounter).ToString("D5") + "] - " + message;
            logQueue.Add(input);
        }

        private void LogProcess()
        {
            try
            {
                while (true)
                {
                    string message = logQueue.Take();
                    if (message != null)
                    {
                        lock (fileLock)
                        {
                            fileHandler.AppendFile(logFileName, Util.TextEncoding.GetBytes(message));
#if DEBUG
                            Debug.WriteLine(message.Trim());
#endif
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // completed adding
                Debug.WriteLine("Logger queue completed adding, exiting the log thread");
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
