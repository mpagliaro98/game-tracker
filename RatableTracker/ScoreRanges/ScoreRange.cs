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
using System.Xml.Linq;

namespace RatableTracker.ScoreRanges
{
    public class ScoreRange : TrackerObjectBase
    {
        public IList<double> ValueList { get; set; } = new List<double>();
        public Color Color { get; set; } = new Color();

        private UniqueID _scoreRelationship = UniqueID.BlankID();
        public ScoreRelationship ScoreRelationship
        {
            get
            {
                if (!_scoreRelationship.HasValue()) return null;
                return Util.Util.FindObjectInList(Module.GetScoreRelationshipList(), _scoreRelationship);
            }
            set
            {
                _scoreRelationship = value.UniqueID;
            }
        }

        protected new TrackerModuleScores Module => (TrackerModuleScores)base.Module;
        protected new SettingsScore Settings => (SettingsScore)base.Settings;

        public ScoreRange(TrackerModuleScores module, SettingsScore settings) : base(settings, module) { }

        public ScoreRange(ScoreRange copyFrom) : base(copyFrom)
        {
            ValueList = new List<double>(copyFrom.ValueList);
            Color = copyFrom.Color;
            _scoreRelationship = UniqueID.Copy(copyFrom._scoreRelationship);
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            ScoreRelationship sr = ScoreRelationship;
            if (sr == null)
                throw new ValidationException("A score relationship is required");
            if (sr.NumValuesRequired != ValueList.Count())
                throw new ValidationException("The " + sr.Name + " relationship requires " + sr.NumValuesRequired.ToString() + " values, but " + ValueList.Count().ToString() + " were given", "list{" + string.Join(",", ValueList) + "}");
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return this.Module.SaveScoreRange(this, (ILoadSaveMethodScores)conn);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            this.Module.DeleteScoreRange(this, (ILoadSaveMethodScores)conn);
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
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
    }
}
