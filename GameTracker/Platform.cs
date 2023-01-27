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
    public class Platform : SaveDeleteObject, IDisposable
    {
        public static int MaxLengthName => 200;
        public static int MaxLengthAbbreviation => 10;

        public string Name { get; set; } = "";
        public Color Color { get; set; } = new Color();
        public string Abbreviation { get; set; } = "";
        public int ReleaseYear { get; set; } = 0;
        public int AcquiredYear { get; set; } = 0;

        public override UniqueID UniqueID { get; protected set; } = UniqueID.NewID();

        protected readonly GameModule module;

        public Platform(GameModule module)
        {
            this.module = module;
        }

        public Platform(Platform copyFrom) : this(copyFrom.module)
        {
            UniqueID = UniqueID.Copy(copyFrom.UniqueID);
            Name = copyFrom.Name;
            Color = copyFrom.Color;
            Abbreviation = copyFrom.Abbreviation;
            ReleaseYear = copyFrom.ReleaseYear;
            AcquiredYear = copyFrom.AcquiredYear;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (Name == "")
                throw new ValidationException("A name is required", Name);
            if (Name.Length > MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + MaxLengthName.ToString() + " characters", Name);
            if (Abbreviation.Length > MaxLengthAbbreviation)
                throw new ValidationException("Abbreviation cannot be longer than " + MaxLengthAbbreviation.ToString() + " characters", Abbreviation);
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return this.module.SavePlatform(this, (ILoadSaveMethodGame)conn);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            this.module.DeletePlatform(this, (ILoadSaveMethodGame)conn);
        }

        public void Dispose()
        {
            RemoveEventHandlers();
        }

        protected virtual void RemoveEventHandlers() { }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
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
                    case "UniqueID":
                        UniqueID = sr.GetValue(key).GetUniqueID();
                        break;
                    case "Name":
                        Name = sr.GetValue(key).GetString();
                        break;
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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Platform)) return false;
            Platform other = (Platform)obj;
            return UniqueID.Equals(other.UniqueID);
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
