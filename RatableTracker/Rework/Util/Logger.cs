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
    public sealed class Logger
    {
        private readonly ILogger logger = null;

        public Logger() { }

        public Logger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Log(string message)
        {
            logger?.Log(message);
        }

        public string MostRecentLogs()
        {
            return logger == null ? "No logger has been attached" : logger.MostRecentLogs();
        }
    }
}
