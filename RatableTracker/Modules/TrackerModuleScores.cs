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

        private IList<ScoreRange> _scoreRanges = new List<ScoreRange>();
        protected IList<ScoreRange> ScoreRanges { get { return _scoreRanges; } private set { _scoreRanges = value; } }
        protected static IList<ScoreRelationship> ScoreRelationships => new List<ScoreRelationship>();

        public delegate void ScoreRangeDeleteHandler(object sender, ScoreRangeDeleteArgs args);
        public event ScoreRangeDeleteHandler ScoreRangeDeleted;

        protected new ILoadSaveHandler<ILoadSaveMethodScores> LoadSave => (ILoadSaveHandler<ILoadSaveMethodScores>)base.LoadSave;

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
            LoadTrackerObjectList(ref _scoreRanges, (conn) => ((ILoadSaveMethodScores)conn).LoadScoreRanges(this, (SettingsScore)settings));
        }

        public void TransferToNewModule(TrackerModuleScores newModule, SettingsScore settings)
        {
            using var connCurrent = LoadSave.NewConnection();
            using var connNew = newModule.LoadSave.NewConnection();
            TransferToNewModule(connCurrent, connNew, settings);
        }

        protected void TransferToNewModule(ILoadSaveMethodScores connCurrent, ILoadSaveMethodScores connNew, SettingsScore settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllScoreRanges(connCurrent.LoadScoreRanges(this, settings));
        }

        public IList<ScoreRange> GetScoreRangeList()
        {
            return GetTrackerObjectList(ScoreRanges, null, null);
        }

        public int TotalNumScoreRanges()
        {
            return ScoreRanges.Count;
        }

        internal bool SaveScoreRange(ScoreRange scoreRange, ILoadSaveMethodScores conn)
        {
            return SaveTrackerObject(scoreRange, ref _scoreRanges, LimitRanges, conn.SaveOneScoreRange);
        }

        internal void DeleteScoreRange(ScoreRange scoreRange, ILoadSaveMethodScores conn)
        {
            DeleteTrackerObject(scoreRange, ref _scoreRanges, conn.DeleteOneScoreRange,
                (obj) => ScoreRangeDeleted?.Invoke(this, new ScoreRangeDeleteArgs(obj, obj.GetType(), conn)), ScoreRangeDeleted == null ? 0 : ScoreRangeDeleted.GetInvocationList().Length);
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
