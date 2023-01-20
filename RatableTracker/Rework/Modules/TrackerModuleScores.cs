using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.ScoreRanges;
using RatableTracker.Rework.Util;
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

        protected IList<ScoreRange> ScoreRanges { get; private set; } = new List<ScoreRange>();
        protected IList<ScoreRelationship> ScoreRelationships => new List<ScoreRelationship>();

        protected readonly new ILoadSaveHandler<ILoadSaveMethodScores> _loadSave;

        public TrackerModuleScores(ILoadSaveHandler<ILoadSaveMethodScores> loadSave) : base(loadSave)
        {
            ScoreRelationships.Add(new ScoreRelationshipAbove());
            ScoreRelationships.Add(new ScoreRelationshipBelow());
            ScoreRelationships.Add(new ScoreRelationshipBetween());
        }

        public override void LoadData(Settings settings)
        {
            base.LoadData(settings);
            using (var conn = _loadSave.NewConnection())
            {
                ScoreRanges = conn.LoadScoreRanges(this);
            }
        }

        public void TransferToNewModule(TrackerModuleScores newModule, Settings settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected void TransferToNewModule(ILoadSaveMethodScores connCurrent, ILoadSaveMethodScores connNew, Settings settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllScoreRanges(connCurrent.LoadScoreRanges(this));
        }

        public override void RemoveReferencesToObject(IKeyable obj, Type type)
        {
            base.RemoveReferencesToObject(obj, type);
            using (var conn = _loadSave.NewConnection())
            {
                foreach (ScoreRange scoreRange in ScoreRanges)
                {
                    if (scoreRange.RemoveReferenceToObject(obj, type))
                    {
                        conn.SaveOneScoreRange(scoreRange);
                    }
                }
            }
        }

        public IList<ScoreRange> GetScoreRangeList()
        {
            return new List<ScoreRange>(ScoreRanges);
        }

        public int TotalNumScoreRanges()
        {
            return ScoreRanges.Count;
        }

        internal void SaveScoreRange(ScoreRange scoreRange)
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
            scoreRange.PostSave();
        }

        internal void DeleteScoreRange(ScoreRange scoreRange)
        {
            // TODO throw unique exception
            if (Util.Util.FindObjectInList(ScoreRanges, scoreRange.UniqueID) == null)
                throw new Exception("Score range " + scoreRange.Name.ToString() + " has not been saved yet and cannot be deleted");
            RemoveReferencesToObject(scoreRange, typeof(ScoreRange));
            ScoreRanges.Remove(scoreRange);
            using (var conn = _loadSave.NewConnection())
            {
                conn.DeleteOneScoreRange(scoreRange);
            }
            scoreRange.PostDelete();
        }

        public IList<ScoreRelationship> GetScoreRelationshipList()
        {
            return new List<ScoreRelationship>(ScoreRelationships);
        }

        public int TotalNumScoreRelationships()
        {
            return ScoreRelationships.Count;
        }
    }
}
