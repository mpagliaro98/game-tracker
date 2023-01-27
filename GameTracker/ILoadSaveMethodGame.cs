using RatableTracker.Interfaces;
using RatableTracker.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public interface ILoadSaveMethodGame : ILoadSaveMethodScoreStatusCategorical
    {
        void SaveOnePlatform(Platform platform);
        void SaveAllPlatforms(IList<Platform> platform);
        void DeleteOnePlatform(Platform platform);
        IList<Platform> LoadPlatforms(GameModule module, SettingsGame settings);
    }
}
