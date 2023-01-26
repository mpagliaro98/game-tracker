using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class LoadSaveMethodJSONGame : LoadSaveMethodJSON, ILoadSaveMethodGame
    {
        protected const string PLATFORMS_FILE = "platforms.json";

        protected IList<SavableRepresentation> platforms = null;

        protected bool platformsChanged = false;

        protected new GameTrackerFactory factory => (GameTrackerFactory)base.factory;

        public LoadSaveMethodJSONGame(IFileHandler fileHandler, GameTrackerFactory factory) : base(fileHandler, factory) { }

        public LoadSaveMethodJSONGame(IFileHandler fileHandler, GameTrackerFactory factory, Logger logger) : base(fileHandler, factory, logger) { }

        protected void EnsurePlatformsAreLoaded()
        {
            EnsureFileContentIsLoaded(PLATFORMS_FILE, ref platforms, InterpretBytesToSRList);
        }

        protected void SavePlatformsIfLoaded()
        {
            SaveFileContentIfLoaded(PLATFORMS_FILE, platforms, SRListToBytes, platformsChanged);
        }

        public override void Dispose()
        {
            if (!operationCanceled)
            {
                SavePlatformsIfLoaded();
            }
            base.Dispose();
        }

        public void DeleteOnePlatform(Platform platform)
        {
            DeleteOne(EnsurePlatformsAreLoaded, ref platforms, platform, ref platformsChanged);
        }

        public IList<Platform> LoadPlatforms(GameModule module)
        {
            return LoadAll(EnsurePlatformsAreLoaded, ref platforms, (s) => factory.GetPlatform(s, module));
        }

        public void SaveAllPlatforms(IList<Platform> platform)
        {
            SaveAll(EnsurePlatformsAreLoaded, ref platforms, platform, ref platformsChanged);
        }

        public void SaveOnePlatform(Platform platform)
        {
            SaveOne(EnsurePlatformsAreLoaded, ref platforms, platform, ref platformsChanged);
        }
    }
}
