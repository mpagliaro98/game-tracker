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

        public Color Color { get; set; } = new Color();
        public string Abbreviation { get; set; } = "";
        public int ReleaseYear { get; set; } = 0;
        public int AcquiredYear { get; set; } = 0;

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

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("Color", new ValueContainer(Color));
            sr.SaveValue("Abbreviation", new ValueContainer(Abbreviation));
            sr.SaveValue("ReleaseYear", new ValueContainer(ReleaseYear));
            sr.SaveValue("AcquiredYear", new ValueContainer(AcquiredYear));
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "Color":
                        Color = sr.GetValue(key).GetColor();
                        break;
                    case "Abbreviation":
                        Abbreviation = sr.GetValue(key).GetString();
                        break;
                    case "ReleaseYear":
                        ReleaseYear = sr.GetValue(key).GetInt();
                        break;
                    case "AcquiredYear":
                        AcquiredYear = sr.GetValue(key).GetInt();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
