using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public class TrackerModuleScores : TrackerModule
    {
        public virtual int LimitRanges => 20;

        protected IList<ScoreRange> ScoreRanges { get; private set; } = new List<ScoreRange>();
        protected IList<ScoreRelationship> ScoreRelationships => new List<ScoreRelationship>();

        public delegate void ScoreRangeDeleteHandler(object sender, ScoreRangeDeleteArgs args);
        public event ScoreRangeDeleteHandler ScoreRangeDeleted;

        protected new ILoadSaveHandler<ILoadSaveMethodScores> _loadSave => (ILoadSaveHandler<ILoadSaveMethodScores>)base._loadSave;

        public TrackerModuleScores(ILoadSaveHandler<ILoadSaveMethodScores> loadSave) : this(loadSave, new Logger()) { }

        public TrackerModuleScores(ILoadSaveHandler<ILoadSaveMethodScores> loadSave, Logger logger) : base(loadSave, logger)
        {
            ScoreRelationships.Add(new ScoreRelationshipAbove());
            ScoreRelationships.Add(new ScoreRelationshipBelow());
            ScoreRelationships.Add(new ScoreRelationshipBetween());
        }

        public override void LoadData(Settings settings)
        {
            base.LoadData(settings);
            ScoreRanges.ForEach(obj => obj.Dispose());
            using (var conn = _loadSave.NewConnection())
            {
                ScoreRanges = conn.LoadScoreRanges(this, (SettingsScore)settings);
            }
            ScoreRanges.ForEach(obj => obj.InitAdditionalResources());
        }

        public void TransferToNewModule(TrackerModuleScores newModule, SettingsScore settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected void TransferToNewModule(ILoadSaveMethodScores connCurrent, ILoadSaveMethodScores connNew, SettingsScore settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllScoreRanges(connCurrent.LoadScoreRanges(this, settings));
        }

        public IList<ScoreRange> GetScoreRangeList()
        {
            return new List<ScoreRange>(ScoreRanges);
        }

        public int TotalNumScoreRanges()
        {
            return ScoreRanges.Count;
        }

        internal bool SaveScoreRange(ScoreRange scoreRange, ILoadSaveMethodScores conn)
        {
            Logger.Log("SaveScoreRange - " + scoreRange.UniqueID.ToString());
            bool isNew = false;
            if (Util.Util.FindObjectInList(ScoreRanges, scoreRange.UniqueID) == null)
            {
                if (ScoreRanges.Count() >= LimitRanges)
                {
                    string message = "Attempted to exceed limit of " + LimitRanges.ToString() + " for list of score ranges";
                    Logger.Log(typeof(ExceededLimitException).Name + ": " + message);
                    throw new ExceededLimitException(message);
                }
                ScoreRanges.Add(scoreRange);
                scoreRange.InitAdditionalResources();
                isNew = true;
            }
            else
            {
                var old = ScoreRanges.Replace(scoreRange);
                if (old != scoreRange)
                {
                    old.Dispose();
                    scoreRange.InitAdditionalResources();
                }
            }
            conn.SaveOneScoreRange(scoreRange);
            return isNew;
        }

        internal void DeleteScoreRange(ScoreRange scoreRange, ILoadSaveMethodScores conn)
        {
            Logger.Log("DeleteScoreRange - " + scoreRange.UniqueID.ToString());
            if (Util.Util.FindObjectInList(ScoreRanges, scoreRange.UniqueID) == null)
            {
                string message = "Score range " + scoreRange.Name.ToString() + " has not been saved yet and cannot be deleted";
                Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            ScoreRanges.Remove(scoreRange);
            scoreRange.Dispose();
            conn.DeleteOneScoreRange(scoreRange);
            Logger.Log("Score range deleted - invoking event ScoreRangeDeleted on " + (ScoreRangeDeleted == null ? "0" : ScoreRangeDeleted.GetInvocationList().Length.ToString()) + " delegates");
            ScoreRangeDeleted?.Invoke(this, new ScoreRangeDeleteArgs(scoreRange, scoreRange.GetType(), conn));
        }

        public override void ApplySettingsChanges(Settings settings, ILoadSaveMethod conn)
        {
            base.ApplySettingsChanges(settings, conn);
            foreach (ScoreRange scoreRange in ScoreRanges)
            {
                scoreRange.ApplySettingsChanges(settings);
                scoreRange.Save(this, conn);
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
