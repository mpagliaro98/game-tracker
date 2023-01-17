using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.ScoreRanges;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModuleScores : TrackerModule
    {
        public virtual int LimitRanges => 20;

        private IList<ScoreRange> _scoreRanges = new List<ScoreRange>();
        protected IList<ScoreRange> ScoreRanges => _scoreRanges;
        protected IList<ScoreRelationship> ScoreRelationships => new List<ScoreRelationship>();

        protected readonly new ILoadSaveHandler<ILoadSaveMethodScores> _loadSave;

        public TrackerModuleScores(ILoadSaveHandler<ILoadSaveMethodScores> loadSave) : base(loadSave)
        {
            ScoreRelationships.Add(new ScoreRelationshipAbove());
            ScoreRelationships.Add(new ScoreRelationshipBelow());
            ScoreRelationships.Add(new ScoreRelationshipBetween());
        }

        public override void Init()
        {
            base.Init();
            using (var conn = _loadSave.NewConnection())
            {
                _scoreRanges = conn.LoadScoreRanges();
            }
        }

        public IList<ScoreRange> GetScoreRangeList()
        {
            return ScoreRanges;
        }

        public int TotalNumScoreRanges()
        {
            return ScoreRanges.Count;
        }

        public void SaveScoreRange(ScoreRange scoreRange)
        {
            // TODO throw unique exception
            scoreRange.Validate();
            if (Util.Util.FindObjectInList(ScoreRanges, scoreRange.UniqueID) == null)
            {
                if (ScoreRanges.Count() >= LimitRanges)
                    throw new Exception("Attempted to exceed limit of " + LimitRanges.ToString() + " for list of score ranges");
                ScoreRanges.Add(scoreRange);
            }
            using (var conn = _loadSave.NewConnection())
            {
                conn.SaveOneScoreRange(scoreRange);
            }
        }

        public void DeleteScoreRange(ScoreRange scoreRange)
        {
            // TODO throw unique exception
            if (Util.Util.FindObjectInList(ScoreRanges, scoreRange.UniqueID) == null)
                throw new Exception("Score range " + scoreRange.Name.ToString() + " has not been saved yet and cannot be deleted");
            using (var conn = _loadSave.NewConnection())
            {
                conn.DeleteOneScoreRange(scoreRange);
            }
        }

        public IList<ScoreRelationship> GetScoreRelationshipList()
        {
            return ScoreRelationships;
        }

        public int TotalNumScoreRelationships()
        {
            return ScoreRelationships.Count;
        }

        public void ScaleScoresToNewRange(double oldMin, double oldMax, double newMin, double newMax)
        {
            // TODO scale all object scores, save all
            // make it overridable so child classes can also scale category values
        }
    }
}
