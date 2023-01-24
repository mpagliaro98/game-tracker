using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    public interface ILoadSaveMethodScores : ILoadSaveMethod
    {
        void SaveOneScoreRange(ScoreRange scoreRange);
        void SaveAllScoreRanges(IList<ScoreRange> scoreRanges);
        void DeleteOneScoreRange(ScoreRange scoreRange);
        IList<ScoreRange> LoadScoreRanges(TrackerModuleScores module);
    }
}
