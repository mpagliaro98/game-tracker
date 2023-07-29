using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public static class Util
    {
        public static string StatusUsageToString(this StatusUsage statusUsage)
        {
            return statusUsage switch
            {
                StatusUsage.AllGames => "All games",
                StatusUsage.FinishableGamesOnly => "Games with a start/end only",
                StatusUsage.UnfinishableGamesOnly => "Games with no start/end only",
                _ => "",
            };
        }
    }
}
