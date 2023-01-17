using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.ObjAddOns;
using RatableTracker.Rework.ScoreRanges;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class LoadSaveMethodJSON : ILoadSaveMethodScoreStatusCategorical
    {
        // TODO in each function, check if file was already loaded in (ex. games.json)
        // if not, load it in and store it as a protected class attribute
        // each function should know how to interpret the list of bytes from the file
        // games.json should be read as a string, translated to a list of savable representations

        protected readonly IFileHandler loadSaveLocation;

        public LoadSaveMethodJSON(IFileHandler loadSaveLocation)
        {
            this.loadSaveLocation = loadSaveLocation;
        }

        public void DeleteOneModelObject(RankedObject rankedObject)
        {
            throw new NotImplementedException();
        }

        public IList<RankedObject> LoadModelObjects()
        {
            throw new NotImplementedException();
        }

        public void SaveAllModelObjects(IList<RankedObject> rankedObjects)
        {
            throw new NotImplementedException();
        }

        public void SaveOneModelObject(RankedObject rankedObject)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            // TODO any files that were loaded, save them back to where they came from, close any open file connections
            // extend this in child classes
        }

        public void SaveOneScoreRange(ScoreRange scoreRange)
        {
            throw new NotImplementedException();
        }

        public void SaveAllScoreRanges(IList<ScoreRange> scoreRanges)
        {
            throw new NotImplementedException();
        }

        public void DeleteOneScoreRange(ScoreRange scoreRange)
        {
            throw new NotImplementedException();
        }

        public IList<ScoreRange> LoadScoreRanges()
        {
            throw new NotImplementedException();
        }

        public void SaveSettings(Settings settings)
        {
            throw new NotImplementedException();
        }

        public Settings LoadSettings()
        {
            throw new NotImplementedException();
        }

        public void SaveOneCategory(RatingCategory ratingCategory)
        {
            throw new NotImplementedException();
        }

        public void SaveAllCategories(IList<RatingCategory> ratingCategories)
        {
            throw new NotImplementedException();
        }

        public void DeleteOneCategory(RatingCategory ratingCategory)
        {
            throw new NotImplementedException();
        }

        public IList<RatingCategory> LoadCategories()
        {
            throw new NotImplementedException();
        }

        public void SaveOneStatus(Status status)
        {
            throw new NotImplementedException();
        }

        public void SaveAllStatuses(IList<Status> statuses)
        {
            throw new NotImplementedException();
        }

        public void DeleteOneStatus(Status status)
        {
            throw new NotImplementedException();
        }

        public IList<Status> LoadStatuses()
        {
            throw new NotImplementedException();
        }
    }
}
