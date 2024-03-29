﻿using RatableTracker.ListManipulation;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    public interface ILoadSaveMethod : IDisposable
    {
        void SaveOneModelObject(RankedObject rankedObject);
        void SaveAllModelObjects(IList<RankedObject> rankedObjects);
        void DeleteOneModelObject(RankedObject rankedObject);
        IList<RankedObject> LoadModelObjects(Settings settings, TrackerModule module);
        IList<T> LoadModelObjectsAndFilter<T>(Settings settings, TrackerModule module, FilterEngine filterEngine, SortEngine sortEngine) where T : RankedObject;
        void SaveSettings(Settings settings);
        Settings LoadSettings();
        void SetCancel(bool cancel);
    }
}
