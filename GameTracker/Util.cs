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

        public static int LevenshteinDistance(string str1, string str2)
        {
            int n = str1.Length;
            int m = str2.Length;
            int[,] d = new int[n + 1, m + 1];

            // Verify arguments.
            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Initialize arrays.
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            // Begin looping.
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    // Compute cost.
                    int cost = (str2[j - 1] == str1[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            // Return cost.
            return d[n, m];
        }
    }
}
