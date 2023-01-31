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
    public class Platform : TrackerObjectBase
    {
        public static int MaxLengthAbbreviation => 10;

        [Savable("Color")] public Color Color { get; set; } = new Color();
        [Savable("Abbreviation")] public string Abbreviation { get; set; } = "";
        [Savable("ReleaseYear")] public int ReleaseYear { get; set; } = 0;
        [Savable("AcquiredYear")] public int AcquiredYear { get; set; } = 0;

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
