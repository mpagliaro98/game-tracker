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
    public class ScoreRange : TrackerObjectBase, IColorContainer
    {
        [Savable()] public IList<double> ValueList { get; set; }
        [Savable()] public Color Color { get; set; } = new Color();

        [Savable("ScoreRelationship")] private UniqueID _scoreRelationship = UniqueID.BlankID();
        public ScoreRelationship ScoreRelationship
        {
            get
            {
                if (!_scoreRelationship.HasValue()) return null;
                return Util.Util.FindObjectInList(Module.GetScoreRelationshipList(), _scoreRelationship);
            }
            set
            {
                _scoreRelationship = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        protected new TrackerModuleScores Module => (TrackerModuleScores)base.Module;
        protected new SettingsScore Settings => (SettingsScore)base.Settings;

        public ScoreRange(TrackerModuleScores module, SettingsScore settings) : base(settings, module)
        {
            ValueList = new List<double>() { settings.MinScore };
        }

        public ScoreRange(ScoreRange copyFrom) : base(copyFrom)
        {
            ValueList = new List<double>(copyFrom.ValueList);
            Color = copyFrom.Color;
            _scoreRelationship = UniqueID.Copy(copyFrom._scoreRelationship);
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            foreach (double val in ValueList)
            {
                if (val < Settings.MinScore || val > Settings.MaxScore)
                    throw new ValidationException("All values must be between " + Settings.MinScore.ToString() + " and " + Settings.MaxScore.ToString(), "list{" + string.Join(",", ValueList) + "}");
            }
            ScoreRelationship sr = ScoreRelationship;
            if (sr == null)
                throw new ValidationException("A score relationship is required");
            if (sr.NumValuesRequired != ValueList.Count)
                throw new ValidationException("The " + sr.Name + " relationship requires " + sr.NumValuesRequired.ToString() + " values, but " + ValueList.Count.ToString() + " were given", "list{" + string.Join(",", ValueList) + "}");
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return this.Module.SaveScoreRange(this, (ILoadSaveMethodScores)conn);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            this.Module.DeleteScoreRange(this, (ILoadSaveMethodScores)conn);
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

        private void OnSettingsChanged(object sender, Events.SettingsChangeArgs args)
        {
            Settings settings = (Settings)sender;
            if (settings is SettingsScore settingsScore)
            {
                IList<double> newValueList = new List<double>();
                foreach (double value in ValueList)
                {
                    newValueList.Add(settingsScore.ScaleValueToNewMinMaxRange(value));
                }
                ValueList = newValueList;
                SaveWithoutValidation(Module, Settings, args.Connection);
            }
        }
    }
}
