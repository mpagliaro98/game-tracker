using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ScoreRanges
{
    public class ScoreRange : IKeyable, ISavable
    {
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";
        public IEnumerable<double> ValueList { get; set; } = new List<double>();
        public Color Color { get; set; } = new Color();

        private UniqueID _scoreRelationship = new UniqueID(false);
        public ScoreRelationship ScoreRelationship
        {
            get
            {
                if (!_scoreRelationship.HasValue()) return null;
                return TrackerModule.FindObjectInList(module.GetScoreRelationshipList(), _scoreRelationship);
            }
            set
            {
                _scoreRelationship = value.UniqueID;
            }
        }

        public UniqueID UniqueID => new UniqueID();

        protected readonly TrackerModuleScores module;

        public ScoreRange(TrackerModuleScores module)
        {
            this.module = module;
        }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation<ValueContainer> sr)
        {
            // TODO load into representation (use attributes?)
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation<ValueContainer> sr)
        {
            // TODO get from representation (use attributes?)
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
