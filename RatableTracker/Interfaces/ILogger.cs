﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        string MostRecentLogs();
    }
}
