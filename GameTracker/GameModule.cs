using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTracker.Filtering;

namespace GameTracker
{
    public class GameModule : TrackerModuleScoreStatusCategorical
    {
        public virtual int LimitPlatforms => 1000;

        private IList<Platform> _platforms = new List<Platform>();
        protected IList<Platform> Platforms { get { return _platforms; } private set { _platforms = value; } }

        public delegate void PlatformDeleteHandler(object sender, PlatformDeleteArgs args);
        public event PlatformDeleteHandler PlatformDeleted;

        protected new ILoadSaveHandler<ILoadSaveMethodGame> LoadSave => (ILoadSaveHandler<ILoadSaveMethodGame>)base.LoadSave;

        public GameModule(ILoadSaveHandler<ILoadSaveMethodGame> loadSave) : this(loadSave, new Logger()) { }

        public GameModule(ILoadSaveHandler<ILoadSaveMethodGame> loadSave, Logger logger) : base(loadSave, logger) { }

        protected override void LoadDataConsecutively(Settings settings, ILoadSaveMethod conn)
        {
            base.LoadDataConsecutively(settings, conn);
            LoadTrackerObjectList(ref _platforms, conn, (conn) => ((ILoadSaveMethodGame)conn).LoadPlatforms(this, (SettingsGame)settings));
        }

        protected override IList<Task> LoadDataCreateTaskList(Settings settings, ILoadSaveMethod conn)
        {
            var list = base.LoadDataCreateTaskList(settings, conn);
            list.Add(Task.Run(() => LoadTrackerObjectList(ref _platforms, conn, (conn) => ((ILoadSaveMethodGame)conn).LoadPlatforms(this, (SettingsGame)settings))));
            return list;
        }

        public void TransferToNewModule(GameModule newModule, SettingsGame settings)
        {
            using var connCurrent = LoadSave.NewConnection();
            using var connNew = newModule.LoadSave.NewConnection();
            TransferToNewModule(connCurrent, connNew, settings);
        }

        protected void TransferToNewModule(ILoadSaveMethodGame connCurrent, ILoadSaveMethodGame connNew, SettingsGame settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllPlatforms(connCurrent.LoadPlatforms(this, settings));
        }

        public IList<Platform> GetPlatformList(SettingsGame settings)
        {
            return GetPlatformList(null, null, settings);
        }

        public IList<Platform> GetPlatformList(FilterEngine filterEngine, SettingsGame settings)
        {
            return GetPlatformList(filterEngine, null, settings);
        }

        public IList<Platform> GetPlatformList(SortEngine sortEngine, SettingsGame settings)
        {
            return GetPlatformList(null, sortEngine, settings);
        }

        public IList<Platform> GetPlatformList(FilterEngine filterEngine, SortEngine sortEngine, SettingsGame settings)
        {
            return GetTrackerObjectList(Platforms, filterEngine, sortEngine, (conn) => ((ILoadSaveMethodGame)conn).LoadPlatformsAndFilter(this, settings, filterEngine, sortEngine));
        }

        public int TotalNumPlatforms()
        {
            return Platforms.Count;
        }

        internal bool SavePlatform(Platform platform, ILoadSaveMethodGame conn)
        {
            return SaveTrackerObject(platform, ref _platforms, LimitPlatforms, conn.SaveOnePlatform);
        }

        internal void DeletePlatform(Platform platform, ILoadSaveMethodGame conn)
        {
            DeleteTrackerObject(platform, ref _platforms, conn.DeleteOnePlatform,
                (obj) => PlatformDeleted?.Invoke(this, new PlatformDeleteArgs(obj, obj.GetType(), conn)), () => PlatformDeleted == null ? 0 : PlatformDeleted.GetInvocationList().Length);
        }

        public IList<GameCompilation> GetEmptyCompilations()
        {
            return ModelObjects.OfType<GameCompilation>().Where((obj) => obj.NumGamesInCompilation() <= 0).ToList();
        }

        public void DeleteEmptyCompilations(SettingsGame settings, ILoadSaveMethodGame conn)
        {
            foreach (var comp in GetEmptyCompilations())
            {
                comp.Delete(this, settings, conn);
            }
        }

        public IList<GameObject> GetGamesOnPlatform(Platform platform, SettingsGame settings)
        {
            return GetGamesOnPlatform(platform, settings, new FilterEngine());
        }

        public IList<GameObject> GetGamesOnPlatform(Platform platform, SettingsGame settings, FilterEngine filterEngine)
        {
            filterEngine.Filters.Add(new FilterSegment() { FilterOption = new FilterOptionGameOnlyNonCompilations() });
            if (!settings.IncludeDLCInStats)
            {
                filterEngine.Filters.Add(new FilterSegment() { FilterOption = new FilterOptionGameDLC(), Negate = true });
            }
            filterEngine.Filters.Add(new FilterSegment() { FilterOption = new FilterOptionGamePlatform(), FilterValues = platform.UniqueID.ToString() });
            filterEngine.Operator = FilterOperator.And;
            return GetModelObjectList<GameObject>(filterEngine, settings);
        }

        public IList<GameObject> GetGamesIncludeInStats(SettingsGame settings)
        {
            return GetModelObjectList(settings).OfType<GameObject>().Where(obj => !obj.IsCompilation && (settings.IncludeDLCInStats || !obj.IsDLC) && obj.IncludeInStats).ToList();
        }

        public IList<GameObject> GetGamesOnPlatformIncludeInStats(Platform platform, SettingsGame settings)
        {
            return GetGamesOnPlatform(platform, settings).Where(obj => obj.IncludeInStats).ToList();
        }

        public IList<GameObject> GetFinishableGamesOnPlatform(Platform platform, SettingsGame settings)
        {
            return GetGamesOnPlatform(platform, settings).Where(obj => obj.IsFinishable).ToList();
        }

        public IList<GameObject> GetFinishedGamesOnPlatform(Platform platform, SettingsGame settings)
        {
            return GetGamesOnPlatform(platform, settings).Where(obj => obj.IsFinished).ToList();
        }

        public int GetNumGamesFinishableByPlatform(Platform platform, SettingsGame settings)
        {
            return GetFinishableGamesOnPlatform(platform, settings).Count;
        }

        public int GetNumGamesFinishedByPlatform(Platform platform, SettingsGame settings)
        {
            return GetFinishedGamesOnPlatform(platform, settings).Count;
        }

        public double GetProportionGamesFinishedByPlatform(Platform platform, SettingsGame settings)
        {
            int numFinishable = GetNumGamesFinishableByPlatform(platform, settings);
            if (numFinishable <= 0) numFinishable = 1;
            return (double)GetNumGamesFinishedByPlatform(platform, settings) / numFinishable;
        }

        public int GetNumGamesByPlatform(Platform platform, SettingsGame settings)
        {
            return GetGamesOnPlatform(platform, settings).Count;
        }

        public double GetAverageScoreOfGamesByPlatform(Platform platform, SettingsGame settings)
        {
            var games = GetGamesOnPlatformIncludeInStats(platform, settings);
            return games.Count <= 0 ? settings.MinScore : games.Average(ro => ro.ScoreDisplay);
        }

        public double GetHighestScoreFromGamesByPlatform(Platform platform, SettingsGame settings)
        {
            var games = GetGamesOnPlatformIncludeInStats(platform, settings);
            return games.Count <= 0 ? settings.MinScore : games.Max(ro => ro.ScoreDisplay);
        }

        public double GetLowestScoreFromGamesByPlatform(Platform platform, SettingsGame settings)
        {
            var games = GetGamesOnPlatformIncludeInStats(platform, settings);
            return games.Count <= 0 ? settings.MinScore : games.Min(ro => ro.ScoreDisplay);
        }

        public IList<GameObject> GetTopGamesByPlatform(Platform platform, SettingsGame settings, int numToGet)
        {
            return GetGamesOnPlatformIncludeInStats(platform, settings).OrderByDescending(ro => ro.ScoreDisplay).Take(numToGet).ToList();
        }

        public IList<GameObject> GetBottomGamesByPlatform(Platform platform, SettingsGame settings, int numToGet)
        {
            return GetGamesOnPlatformIncludeInStats(platform, settings).OrderBy(ro => ro.ScoreDisplay).Take(numToGet).ToList();
        }

        public override int GetRankOfScore(double score, SettingsScore settings)
        {
            return GetRankOfScore(score, GetGamesIncludeInStats((SettingsGame)settings).Cast<RankedObject>().ToList());
        }

        public int GetRankOfScoreByPlatform(double score, Platform platform, SettingsGame settings)
        {
            return GetRankOfScore(score, GetGamesOnPlatformIncludeInStats(platform, settings).Cast<RankedObject>().ToList());
        }
    }
}
