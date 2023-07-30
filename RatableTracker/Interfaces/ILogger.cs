using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    public interface ILogger : IDisposable
    {
        void Log(string message);
        IEnumerable<RatableTracker.Util.FileInfo> EnumerateLogFiles();
        string GetLogFileContents(string logFileName);
    }
}
