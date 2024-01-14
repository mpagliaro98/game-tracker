using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class SettingsGame : SettingsScore
    {
        [Savable] public bool TreatAllGamesAsOwned { get; set; } = false;
        [Savable] public bool IncludeUnownedGamesInFinishCount { get; set; } = false;
        [Savable] public bool IncludeDLCInStats { get; set; } = false;

        public SettingsGame() : base() { }
    }
}
