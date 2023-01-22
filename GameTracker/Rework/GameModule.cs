using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Rework
{
    public class GameModule : TrackerModuleScoreStatusCategorical
    {
        public virtual int LimitPlatforms => 1000;

        protected IList<Platform> Platforms { get; private set; } = new List<Platform>();

        protected readonly new ILoadSaveHandler<ILoadSaveMethodGame> _loadSave;

        public GameModule(ILoadSaveHandler<ILoadSaveMethodGame> loadSave) : this(loadSave, new Logger()) { }

        public GameModule(ILoadSaveHandler<ILoadSaveMethodGame> loadSave, Logger logger) : base(loadSave, logger) { }

        public override void LoadData(Settings settings)
        {
            base.LoadData(settings);
            using (var conn = _loadSave.NewConnection())
            {
                Platforms = conn.LoadPlatforms(this);
            }
        }

        public void TransferToNewModule(GameModule newModule, Settings settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected void TransferToNewModule(ILoadSaveMethodGame connCurrent, ILoadSaveMethodGame connNew, Settings settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllPlatforms(connCurrent.LoadPlatforms(this));
        }

        public override void RemoveReferencesToObject(IKeyable obj, Type type)
        {
            base.RemoveReferencesToObject(obj, type);
            using (var conn = _loadSave.NewConnection())
            {
                foreach (Platform platform in Platforms)
                {
                    if (platform.RemoveReferenceToObject(obj, type))
                    {
                        platform.Save(this, conn);
                    }
                }
            }
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
            try
            {
                IList<Platform> list = new List<Platform>(Platforms);
                if (filterOptions != null) list = filterOptions.ApplyFilters(list);
                if (sortOptions != null) list = sortOptions.ApplySorting(list);
                return list;
            }
            catch (ListManipulationException e)
            {
                Logger.Log(e.GetType().Name + ": " + e.Message + " - value " + e.InvalidValue.ToString());
                throw;
            }
        }

        public int TotalNumPlatforms()
        {
            return Platforms.Count;
        }

        internal bool SavePlatform(Platform platform, ILoadSaveMethodGame conn)
        {
            Logger.Log("SavePlatform - " + platform.UniqueID.ToString());

            bool isNew = false;
            if (RatableTracker.Rework.Util.Util.FindObjectInList(Platforms, platform.UniqueID) == null)
            {
                if (Platforms.Count() >= LimitPlatforms)
                {
                    string message = "Attempted to exceed limit of " + LimitPlatforms.ToString() + " for list of platforms";
                    Logger.Log(typeof(ExceededLimitException).Name + ": " + message);
                    throw new ExceededLimitException(message);
                }
                Platforms.Add(platform);
                isNew = true;
            }

            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.SaveOnePlatform(platform);
                }
            }
            else
            {
                conn.SaveOnePlatform(platform);
            }
            return isNew;
        }

        internal void DeletePlatform(Platform platform, ILoadSaveMethodGame conn)
        {
            Logger.Log("DeletePlatform - " + platform.UniqueID.ToString());
            if (RatableTracker.Rework.Util.Util.FindObjectInList(Platforms, platform.UniqueID) == null)
            {
                string message = "Platform " + platform.Name.ToString() + " has not been saved yet and cannot be deleted";
                Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            Platforms.Remove(platform);
            if (conn == null)
            {
                using (var connNew = _loadSave.NewConnection())
                {
                    connNew.DeleteOnePlatform(platform);
                }
            }
            else
            {
                conn.DeleteOnePlatform(platform);
            }
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            foreach (Platform platform in Platforms)
            {
                platform.ApplySettingsChanges(settings);
            }
            using (var conn = _loadSave.NewConnection())
            {
                foreach (Platform platform in Platforms)
                {
                    platform.Save(this, conn);
                }
            }
        }

        public IList<GameCompilation> GetEmptyCompilations()
        {
            return ModelObjects.OfType<GameCompilation>().Where((obj) => obj.NumGamesInCompilation() <= 0).ToList();
        }

        public void DeleteEmptyCompilations()
        {
            foreach (var comp in GetEmptyCompilations())
            {
                comp.Delete(this);
            }
        }

        // TODO functions for platform stats
        public IList<GameObject> GetGamesOnPlatform(Platform platform, SettingsGame settings)
        {
            return GetGamesOnPlatform(platform, new FilterGames(this, settings));
        }

        public IList<GameObject> GetGamesOnPlatform(Platform platform, FilterGames filterOptions)
        {
            filterOptions.Platform = platform;
            return GetModelObjectList(filterOptions).OfType<GameObject>().ToList();
        }
    }
}
