using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;

namespace GameTracker.Model
{
    public abstract class LoadSaveEngineGame : LoadSaveEngineCompletable
    {
        public abstract IEnumerable<Platform> LoadPlatforms(RatingModule parentModule);
        public abstract void SavePlatforms(IEnumerable<Platform> platforms);
    }
}
