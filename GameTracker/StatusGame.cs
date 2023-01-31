using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class StatusGame : Status
    {
        [Savable("UseAsFinished")] public bool UseAsFinished { get; set; } = false;
        [Savable("ExcludeFromStats")] public bool ExcludeFromStats { get; set; } = false;

        public override bool HideScoreOfModelObject
        {
            get { return !UseAsFinished; }
        }

        public override bool ExcludeModelObjectFromStats
        {
            get { return ExcludeFromStats; }
        }

        protected new SettingsGame Settings => (SettingsGame)base.Settings;

        public StatusGame(IModuleStatus module, SettingsGame settings) : base(module, settings) { }

        public StatusGame(StatusGame copyFrom) : base(copyFrom)
        {
            UseAsFinished = copyFrom.UseAsFinished;
            ExcludeFromStats = copyFrom.ExcludeFromStats;
        }
    }
}
