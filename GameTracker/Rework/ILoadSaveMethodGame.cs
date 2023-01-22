using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Rework
{
    public interface ILoadSaveMethodGame : ILoadSaveMethodScoreStatusCategorical
    {
        void SaveOnePlatform(Platform platform);
        void SaveAllPlatforms(IList<Platform> platform);
        void DeleteOnePlatform(Platform platform);
        IList<Platform> LoadPlatforms(GameModule module);
    }
}
