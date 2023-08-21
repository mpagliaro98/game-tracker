using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Model
{
    public class RankedObject : TrackerObjectBase
    {
        public static int MaxLengthComment => 30000;

        [Savable()] public string Comment { get; set; } = "";

        public virtual int Rank
        {
            get
            {
                IList<RankedObject> rankedObjects = Module.GetModelObjectList(Settings);
                return rankedObjects.IndexOf(this) + 1;
            }
        }

        [Savable(SaveOnly = true)]
        protected internal int SortOrder
        {
            get
            {
                IList<RankedObject> rankedObjects = Module.GetModelObjectList(Settings);
                return rankedObjects.IndexOf(this) + 1;
            }
        }

        public virtual double Score
        {
            get { return Rank; }
        }

        public virtual double ScoreDisplay
        {
            get { return Score; }
        }

        public virtual bool ShowScore
        {
            get { return true; }
        }

        public RankedObject(Settings settings, TrackerModule module) : base(settings, module) { }

        public RankedObject(RankedObject copyFrom) : base(copyFrom)
        {
            Comment = copyFrom.Comment;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (Comment.Length > MaxLengthComment)
                throw new ValidationException("Comment cannot be longer than " + MaxLengthComment.ToString() + " characters", Comment);
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return module.SaveModelObject(this, conn);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            module.DeleteModelObject(this, conn);
        }

        public void MoveUpOneRank()
        {
            Module.Logger.Log("MoveUpOneRank - " + UniqueID.ToString());
            if (Rank <= 1)
            {
                string message = "Cannot raise rank any further";
                Module.Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            ChangeRank(Rank - 1);
        }

        public void MoveDownOneRank()
        {
            Module.Logger.Log("MoveDownOneRank - " + UniqueID.ToString());
            if (Rank >= Module.TotalNumModelObjects())
            {
                string message = "Cannot lower rank any further";
                Module.Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            ChangeRank(Rank + 1);
        }

        public void ChangeRank(int newRank)
        {
            Module.Logger.Log("ChangeRank - " + UniqueID.ToString());
            if (newRank < 1 || newRank > Module.TotalNumModelObjects())
            {
                string message = "Rank out of range: " + newRank.ToString();
                Module.Logger.Log(typeof(InvalidObjectStateException).Name + ": " + message);
                throw new InvalidObjectStateException(message);
            }
            Module.ChangeModelObjectPositionInList(this, newRank - 1, Settings);
        }
    }
}
