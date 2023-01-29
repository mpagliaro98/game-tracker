using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IList<Platform> GetPlatformList()
        {
            return GetPlatformList(null, null);
        }

        public IList<Platform> GetPlatformList(FilterPlatforms filterOptions)
        {
            return GetPlatformList(filterOptions, null);
        }

        public IList<Platform> GetPlatformList(SortPlatforms sortOptions)
        {
            return GetPlatformList(null, sortOptions);
        }

        public IList<Platform> GetPlatformList(FilterPlatforms filterOptions, SortPlatforms sortOptions)
        {
            return GetTrackerObjectList(Platforms, filterOptions, sortOptions);
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
            return GetGamesOnPlatform(platform, new FilterGames(this, settings));
        }

        public IList<GameObject> GetGamesOnPlatform(Platform platform, FilterGames filterOptions)
        {
            filterOptions.Platform = platform;
            return GetModelObjectList(filterOptions).OfType<GameObject>().ToList();
        }

        public IList<GameObject> GetFinishableGamesOnPlatform(Platform platform, SettingsGame settings)
        {
            return GetGamesOnPlatform(platform, settings).Where(obj => obj.StatusExtension.Status == null || !obj.StatusExtension.Status.ExcludeModelObjectFromStats).ToList();
        }

        public IList<GameObject> GetFinishedGamesOnPlatform(Platform platform, SettingsGame settings)
        {
            return GetGamesOnPlatform(platform, settings).Where(obj => obj.StatusExtension.Status != null && !obj.StatusExtension.Status.HideScoreOfModelObject).ToList();
        }

        public int GetNumGamesFinishableByPlatform(Platform platform, SettingsGame settings)
        {
            return GetFinishableGamesOnPlatform(platform, settings).Count;
        }

        public double GetNumGamesFinishedByPlatform(Platform platform, SettingsGame settings)
        {
            return GetFinishedGamesOnPlatform(platform, settings).Count;
        }

        public double GetProportionGamesFinishedByPlatform(Platform platform, SettingsGame settings)
        {
            int numFinishable = GetNumGamesFinishableByPlatform(platform, settings);
            if (numFinishable <= 0) numFinishable = 1;
            return GetNumGamesFinishedByPlatform(platform, settings) / numFinishable;
        }

        //public IEnumerable<TListedObj> GetFinishedGamesExcludeStatsOnPlatform(Platform platform)
        //{
        //    return ListedObjects.Where(ro => ro.RefPlatform.HasReference() && ro.RefPlatform.IsReferencedObject(platform))
        //        .Where(ro => ro.RefStatus.HasReference() && FindStatus(ro.RefStatus).UseAsFinished && !FindStatus(ro.RefStatus).ExcludeFromStats);
        //}

        //public int GetNumGamesByPlatform(Platform platform)
        //{
        //    return GetGamesOnPlatform(platform).Count();
        //}

        //public double GetAverageScoreOfGamesByPlatform(Platform platform)
        //{
        //    var games = GetFinishedGamesOnPlatform(platform);
        //    return games.Count() <= 0 ? Settings.MinScore : games.Average(ro => GetScoreOfObject(ro));
        //}

        //public double GetHighestScoreFromGamesByPlatform(Platform platform)
        //{
        //    var games = GetFinishedGamesOnPlatform(platform);
        //    return games.Count() <= 0 ? Settings.MinScore : games.Max(ro => GetScoreOfObject(ro));
        //}

        //public double GetLowestScoreFromGamesByPlatform(Platform platform)
        //{
        //    var games = GetFinishedGamesOnPlatform(platform);
        //    return games.Count() <= 0 ? Settings.MinScore : games.Min(ro => GetScoreOfObject(ro));
        //}

        //public IEnumerable<TListedObj> GetTopGamesByPlatform(Platform platform, int numToGet)
        //{
        //    return GetFinishedGamesOnPlatform(platform)
        //        .OrderByDescending(ro => GetScoreOfObject(ro))
        //        .Take(numToGet);
        //}

        //public IEnumerable<TListedObj> GetBottomGamesByPlatform(Platform platform, int numToGet)
        //{
        //    return GetFinishedGamesOnPlatform(platform)
        //        .OrderBy(ro => GetScoreOfObject(ro))
        //        .Take(numToGet);
        //}

        //public int GetRankOfScoreByPlatform(double score, Platform platform)
        //{
        //    return GetRankOfScore(score, ListedObjects, game => game.RefPlatform.IsReferencedObject(platform), null);
        //}

        //public int GetRankOfScoreByPlatform(double score, Platform platform, TListedObj obj)
        //{
        //    return GetRankOfScore(score, ListedObjects, game => game.RefPlatform.IsReferencedObject(platform), obj);
        //}
    }
}
