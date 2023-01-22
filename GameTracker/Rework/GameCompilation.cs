using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Rework
{
    public class GameCompilation : GameObject
    {
        public override double Score
        {
            get
            {
                // TODO potentially average categories by weight instead of flat average
                return GamesInCompilation().Select((obj) => obj.Score).Average();
            }
        }

        public override bool IsCompilation { get { return true; } }
        public override GameCompilation Compilation { get { return null; } set { } }

        public GameCompilation(SettingsGame settings, GameModule module) : base(settings, module) { }

        public IList<GameObject> GamesInCompilation()
        {
            return module.GetModelObjectList().OfType<GameObject>().Where((obj) => obj.Compilation != null && obj.Compilation.Equals(this)).ToList();
        }

        public int NumGamesInCompilation()
        {
            return GamesInCompilation().Count;
        }
    }
}
