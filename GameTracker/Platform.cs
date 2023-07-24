using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class Platform : TrackerObjectBase, IColorContainer
    {
        public static int MaxLengthAbbreviation => 10;

        [Savable()] public Color Color { get; set; } = new Color();
        [Savable()] public string Abbreviation { get; set; } = "";
        [Savable()] public int ReleaseYear { get; set; } = 0;
        [Savable()] public int AcquiredYear { get; set; } = 0;

        public int NumGames => Module.GetNumGamesByPlatform(this, Settings);
        public double AverageScore => Module.GetAverageScoreOfGamesByPlatform(this, Settings);
        public double HighestScore => Module.GetHighestScoreFromGamesByPlatform(this, Settings);
        public double LowestScore => Module.GetLowestScoreFromGamesByPlatform(this, Settings);
        public double FinishPercent => Module.GetProportionGamesFinishedByPlatform(this, Settings);

        protected new GameModule Module => (GameModule)base.Module;
        protected new SettingsGame Settings => (SettingsGame)base.Settings;

        public Platform(GameModule module, SettingsGame settings) : base(settings, module) { }

        public Platform(Platform copyFrom) : base(copyFrom)
        {
            Color = copyFrom.Color;
            Abbreviation = copyFrom.Abbreviation;
            ReleaseYear = copyFrom.ReleaseYear;
            AcquiredYear = copyFrom.AcquiredYear;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (Abbreviation.Length > MaxLengthAbbreviation)
                throw new ValidationException("Abbreviation cannot be longer than " + MaxLengthAbbreviation.ToString() + " characters", Abbreviation);
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return this.Module.SavePlatform(this, (ILoadSaveMethodGame)conn);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            this.Module.DeletePlatform(this, (ILoadSaveMethodGame)conn);
        }
    }
}
