using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Rework.ScoreRanges
{
    public class ScoreRange : SavableObject, IKeyable
    {
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";
        public IList<double> ValueList { get; set; } = new List<double>();
        public Color Color { get; set; } = new Color();

        private UniqueID _scoreRelationship = UniqueID.BlankID();
        public ScoreRelationship ScoreRelationship
        {
            get
            {
                if (!_scoreRelationship.HasValue()) return null;
                return Util.Util.FindObjectInList(module.GetScoreRelationshipList(), _scoreRelationship);
            }
            set
            {
                _scoreRelationship = value.UniqueID;
            }
        }

        public UniqueID UniqueID { get; private set; } = UniqueID.NewID();

        protected readonly TrackerModuleScores module;

        public ScoreRange(TrackerModuleScores module)
        {
            this.module = module;
        }

        public void Validate()
        {
            try
            {
                ValidateFields();
            }
            catch (ValidationException e)
            {
                module.Logger?.Log(e.GetType().Name + ": " + e.Message + " - invalid value: " + e.InvalidValue.ToString());
                throw;
            }
            PostValidate();
        }

        protected virtual void ValidateFields()
        {
            if (Name == "")
                throw new ValidationException("A name is required", Name);
            if (Name.Length > MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + MaxLengthName.ToString() + " characters", Name);
            ScoreRelationship sr = ScoreRelationship;
            if (sr == null)
                throw new ValidationException("A score relationship is required");
            if (sr.NumValuesRequired != ValueList.Count())
                throw new ValidationException("The " + sr.Name + " relationship requires " + sr.NumValuesRequired.ToString() + " values, but " + ValueList.Count().ToString() + " were given", "list{" + string.Join(",", ValueList) + "}");
        }

        protected virtual void PostValidate() { }

        public void Save()
        {
            module.SaveScoreRange(this);
        }

        public virtual void PostSave() { }

        public void Delete()
        {
            module.DeleteScoreRange(this);
        }

        public virtual void PostDelete() { }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            return false;
        }

        public virtual void ApplySettingsChanges(Settings settings)
        {
            if (settings is SettingsScore settingsScore)
            {
                IList<double> newValueList = new List<double>();
                foreach (double value in ValueList)
                {
                    newValueList.Add(settingsScore.ScaleValueToNewMinMaxRange(value));
                }
                ValueList = newValueList;
            }
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            sr.SaveValue("ValueList", new ValueContainer(ValueList));
            sr.SaveValue("Color", new ValueContainer(Color));
            sr.SaveValue("ScoreRelationship", new ValueContainer(_scoreRelationship));
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
                    case "ValueList":
                        ValueList = sr.GetValue(key).GetDoubleList().ToList();
                        break;
                    case "Color":
                        Color = sr.GetValue(key).GetColor();
                        break;
                    case "ScoreRelationship":
                        _scoreRelationship = sr.GetValue(key).GetUniqueID();
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ScoreRange)) return false;
            ScoreRange other = (ScoreRange)obj;
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
