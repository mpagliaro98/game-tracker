using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModuleScores : TrackerModule
    {
        protected IList<ScoreRange> ScoreRanges => new List<ScoreRange>();
        protected IList<ScoreRelationship> ScoreRelationships => new List<ScoreRelationship>();

        public TrackerModuleScores(ILoadSaveMethod loadSave) : base(loadSave)
        {
            ScoreRelationships.Add(new ScoreRelationshipAbove());
            ScoreRelationships.Add(new ScoreRelationshipBelow());
            ScoreRelationships.Add(new ScoreRelationshipBetween());
        }

        public IList<ScoreRange> GetScoreRangeList()
        {
            return ScoreRanges;
        }

        public IList<ScoreRelationship> GetScoreRelationshipList()
        {
            return ScoreRelationships;
        }
    }
}
