using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.ScoreRanges;
using RatableTracker.Rework.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

        public TrackerModuleScores(ILoadSaveHandler<ILoadSaveMethodScores> loadSave, ILogger logger = null) : base(loadSave, logger)
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
                        scoreRange.Save(this, conn);
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

        internal void SaveScoreRange(ScoreRange scoreRange, ILoadSaveMethodScores conn)
        {
            Logger?.Log("SaveScoreRange - " + scoreRange.UniqueID.ToString());

            if (Util.Util.FindObjectInList(ScoreRanges, scoreRange.UniqueID) == null)
            {
                if (ScoreRanges.Count() >= LimitRanges)
                {
                    try
                    {
                        throw new ExceededLimitException("Attempted to exceed limit of " + LimitRanges.ToString() + " for list of score ranges");
                    }
                    catch (ExceededLimitException e)
                    {
                        Logger?.Log(e.GetType().Name + ": " + e.Message);
                        throw;
                    }
                }
                ScoreRanges.Add(scoreRange);
            }

            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.SaveOneScoreRange(scoreRange);
                }
            }
            else
            {
                conn.SaveOneScoreRange(scoreRange);
            }
        }

        internal void DeleteScoreRange(ScoreRange scoreRange, ILoadSaveMethodScores conn)
        {
            Logger?.Log("DeleteScoreRange - " + scoreRange.UniqueID.ToString());
            if (Util.Util.FindObjectInList(ScoreRanges, scoreRange.UniqueID) == null)
            {
                try
                {
                    throw new InvalidObjectStateException("Score range " + scoreRange.Name.ToString() + " has not been saved yet and cannot be deleted");
                }
                catch (InvalidObjectStateException e)
                {
                    Logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
            RemoveReferencesToObject(scoreRange, typeof(ScoreRange));
            ScoreRanges.Remove(scoreRange);
            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.DeleteOneScoreRange(scoreRange);
                }
            }
            else
            {
                conn.DeleteOneScoreRange(scoreRange);
            }
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            foreach (ScoreRange scoreRange in ScoreRanges)
            {
                scoreRange.ApplySettingsChanges(settings);
            }
            using (var conn = _loadSave.NewConnection())
            {
                foreach (ScoreRange scoreRange in ScoreRanges)
                {
                    scoreRange.Save(this, conn);
                }
            }
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
