using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface ILoadSaveMethodScores : ILoadSaveMethod
    {
        void SaveOneScoreRange(ScoreRange scoreRange);
        void SaveAllScoreRanges(IList<ScoreRange> scoreRanges);
        void DeleteOneScoreRange(ScoreRange scoreRange);
        IList<ScoreRange> LoadScoreRanges(TrackerModuleScores module);
    }
}
