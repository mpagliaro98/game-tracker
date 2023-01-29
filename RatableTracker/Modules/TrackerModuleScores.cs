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
        protected IList<ScoreRelationship> ScoreRelationships { get; init; }

        public delegate void ScoreRangeDeleteHandler(object sender, ScoreRangeDeleteArgs args);
        public event ScoreRangeDeleteHandler ScoreRangeDeleted;

        protected new ILoadSaveHandler<ILoadSaveMethodScores> LoadSave => (ILoadSaveHandler<ILoadSaveMethodScores>)base.LoadSave;

        public TrackerModuleScores(ILoadSaveHandler<ILoadSaveMethodScores> loadSave) : this(loadSave, new Logger()) { }

        public TrackerModuleScores(ILoadSaveHandler<ILoadSaveMethodScores> loadSave, Logger logger) : base(loadSave, logger)
        {
            IList<ScoreRelationship> list = new List<ScoreRelationship>
            {
                new ScoreRelationshipAbove(),
                new ScoreRelationshipBelow(),
                new ScoreRelationshipBetween()
            };
            ScoreRelationships = list;
        }

        protected override void LoadDataConsecutively(Settings settings, ILoadSaveMethod conn)
        {
            base.LoadDataConsecutively(settings, conn);
            try
            {
                LoadTrackerObjectList(ref _scoreRanges, conn, (conn) => ((ILoadSaveMethodScores)conn).LoadScoreRanges(this, (SettingsScore)settings));
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException("Settings for loading score ranges must be of type SettingsScore or more derived", e);
            }
        }

        protected override IList<Task> LoadDataCreateTaskList(Settings settings, ILoadSaveMethod conn)
        {
            var list = base.LoadDataCreateTaskList(settings, conn);
            list.Add(Task.Run(() => LoadTrackerObjectList(ref _scoreRanges, conn, (conn) => ((ILoadSaveMethodScores)conn).LoadScoreRanges(this, (SettingsScore)settings))));
            return list;
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
                (obj) => ScoreRangeDeleted?.Invoke(this, new ScoreRangeDeleteArgs(obj, obj.GetType(), conn)), () => ScoreRangeDeleted == null ? 0 : ScoreRangeDeleted.GetInvocationList().Length);
        }

        public IList<ScoreRelationship> GetScoreRelationshipList()
        {
            return new List<ScoreRelationship>(ScoreRelationships);
        }

        public int TotalNumScoreRelationships()
        {
            return ScoreRelationships.Count;
        }

        public int GetRankOfScore(double score, SettingsScore settings)
        {
            return GetRankOfScore(score, settings, GetModelObjectList());
        }

        public int GetRankOfScore(double score, SettingsScore settings, IList<RankedObject> objects)
        {
            var list = objects.OrderByDescending(obj => obj.ScoreDisplay).ToList();
            int rank = 0;
            for (; rank < list.Count; rank++)
            {
                if (score >= list[rank].ScoreDisplay)
                    break;
            }
            return rank += 1;
        }
    }
}
