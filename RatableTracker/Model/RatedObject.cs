using RatableTracker.Util;
using RatableTracker.Modules;
using RatableTracker.ScoreRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.LoadSave;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;

namespace RatableTracker.Model
{
    public class RatedObject : RankedObject
    {
        public override double Score
        {
            get { return ManualScore; }
        }

        [Savable()] public double ManualScore { get; set; } = 0;

        public virtual ScoreRange ScoreRange { get { return Util.Util.GetScoreRange(Score, Module); } }
        public virtual ScoreRange ScoreRangeDisplay { get { return Util.Util.GetScoreRange(ScoreDisplay, Module); } }

        public override int Rank
        {
            get
            {
                IList<RankedObject> rankedObjects = Module.GetModelObjectList(Settings).OrderByDescending(obj => obj.ScoreDisplay).ToList();
                return rankedObjects.IndexOf(this) + 1;
            }
        }

        // Re-declared as a different derived type
        protected new SettingsScore Settings => (SettingsScore)base.Settings;
        protected new TrackerModuleScores Module => (TrackerModuleScores)base.Module;

        public RatedObject(SettingsScore settings, TrackerModuleScores module) : base(settings, module)
        {
            ManualScore = settings.MinScore;
        }

        public RatedObject(RatedObject copyFrom) : base(copyFrom)
        {
            ManualScore = copyFrom.ManualScore;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (ManualScore < Settings.MinScore || ManualScore > Settings.MaxScore)
                throw new ValidationException("Score must be between " + Settings.MinScore.ToString() + " and " + Settings.MaxScore.ToString(), ManualScore);
        }

        protected override void AddEventHandlers()
        {
            base.AddEventHandlers();
            Settings.SettingsChanged += OnSettingsChanged;
        }

        protected override void RemoveEventHandlers()
        {
            base.RemoveEventHandlers();
            Settings.SettingsChanged -= OnSettingsChanged;
        }

        protected virtual void OnSettingsChanged(object sender, Events.SettingsChangeArgs args)
        {
            Settings settings = (Settings)sender;
            if (settings is SettingsScore settingsScore)
            {
                ManualScore = settingsScore.ScaleValueToNewMinMaxRange(ManualScore);
                SaveWithoutValidation(Module, Settings, args.Connection);
            }
        }
    }
}
