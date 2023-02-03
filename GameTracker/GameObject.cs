﻿using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class GameObject : RatedObjectStatusCategorical
    {
        public static int MaxLengthCompletionCriteria => 1000;
        public static int MaxLengthCompletionComment => 1000;
        public static int MaxLengthTimeSpent => 1000;

        public new CategoryExtensionGame CategoryExtension { get { return (CategoryExtensionGame)base.CategoryExtension; } }

        [Savable("CompletionCriteria")] public string CompletionCriteria { get; set; } = "";
        [Savable("CompletionComment")] public string CompletionComment { get; set; } = "";
        [Savable("TimeSpent")] public string TimeSpent { get; set; } = "";
        [Savable("ReleaseDate")] public DateTime ReleaseDate { get; set; } = DateTime.MinValue;
        [Savable("AcquiredOn")] public DateTime AcquiredOn { get; set; } = DateTime.MinValue;
        [Savable("StartedOn")] public DateTime StartedOn { get; set; } = DateTime.MinValue;
        [Savable("FinishedOn")] public DateTime FinishedOn { get; set; } = DateTime.MinValue;
        [Savable("IsRemaster")] public virtual bool IsRemaster { get; set; } = false;
        [Savable("UseOriginalGameScore")] public virtual bool UseOriginalGameScore { get; set; } = false;
        [Savable("IsPartOfCompilation", SaveOnly = true)] public virtual bool IsPartOfCompilation { get { return _compilation.HasValue(); } }
        [Savable("IsCompilation", SaveOnly = true)] public virtual bool IsCompilation { get { return false; } }
        [Savable("IsUnfinishable")] public bool IsUnfinishable { get; set; } = false;

        public bool HasOriginalGame { get { return _originalGame.HasValue(); } }
        public virtual bool IsUsingOriginalGameScore { get { return IsRemaster && HasOriginalGame && UseOriginalGameScore; } }

        public override double Score
        {
            get
            {
                return CalculateScoreRecursive(new List<UniqueID>());
            }
        }

        public double ScoreMinIfCyclical
        {
            get
            {
                try
                {
                    return Score;
                }
                catch (CyclicalReferenceException)
                {
                    return Settings.MinScore;
                }
            }
        }

        public override double ScoreDisplay
        {
            get
            {
                try
                {
                    return base.ScoreDisplay;
                }
                catch (CyclicalReferenceException ex)
                {
                    Module.Logger.Log(ex.GetType().Name + " in ScoreDisplay: " + ex.Message);
                    return Settings.MinScore;
                }
            }
        }

        public override ScoreRange ScoreRange
        {
            get
            {
                try
                {
                    return base.ScoreRange;
                }
                catch (CyclicalReferenceException)
                {
                    return null;
                }
            }
        }

        [Savable("Platform")] private UniqueID _platform = UniqueID.BlankID();
        public Platform Platform
        {
            get
            {
                if (!_platform.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(Module.GetPlatformList(Settings), _platform);
            }
            set
            {
                _platform = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        [Savable("PlatformPlayedOn")] private UniqueID _platformPlayedOn = UniqueID.BlankID();
        public Platform PlatformPlayedOn
        {
            get
            {
                if (!_platformPlayedOn.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(Module.GetPlatformList(Settings), _platformPlayedOn);
            }
            set
            {
                _platformPlayedOn = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        [Savable("OriginalGame")] private UniqueID _originalGame = UniqueID.BlankID();
        public GameObject OriginalGame
        {
            get
            {
                if (!_originalGame.HasValue()) return null;
                return (GameObject)RatableTracker.Util.Util.FindObjectInList(Module.GetModelObjectList(Settings), _originalGame);
            }
            set
            {
                _originalGame = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        [Savable("Compilation")] private UniqueID _compilation = UniqueID.BlankID();
        public virtual GameCompilation Compilation
        {
            get
            {
                if (!_compilation.HasValue()) return null;
                return (GameCompilation)RatableTracker.Util.Util.FindObjectInList(Module.GetModelObjectList(Settings), _compilation);
            }
            set
            {
                _compilation = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        public bool IncludeInStats
        {
            get
            {
                // has a status with MarkAsFinished = true
                return StatusExtension.Status != null && !StatusExtension.Status.HideScoreOfModelObject;
            }
        }

        public bool IsFinishable
        {
            get
            {
                // has no status or has a status with ExcludeFromStats = false
                return StatusExtension.Status == null || !StatusExtension.Status.ExcludeModelObjectFromStats;
            }
        }

        public bool IsFinished
        {
            get
            {
                // has a status with MarkAsFinished = true and ExcludeFromStats = false
                return IsFinishable && IncludeInStats;
            }
        }

        protected new SettingsGame Settings => (SettingsGame)base.Settings;
        protected new GameModule Module => (GameModule)base.Module;

        public GameObject(SettingsGame settings, GameModule module) : this(settings, module, new CategoryExtensionGame(module.CategoryExtension, settings)) { }

        protected GameObject(SettingsGame settings, GameModule module, CategoryExtensionGame categoryExtension) : base(settings, module, categoryExtension) { }

        public GameObject(GameObject copyFrom) : this(copyFrom, new CategoryExtensionGame(copyFrom.CategoryExtension)) { }

        protected GameObject(GameObject copyFrom, CategoryExtensionGame categoryExtension) : base(copyFrom, new StatusExtension(copyFrom.StatusExtension), categoryExtension)
        {
            CompletionCriteria = copyFrom.CompletionCriteria;
            CompletionComment = copyFrom.CompletionComment;
            TimeSpent = copyFrom.TimeSpent;
            ReleaseDate = copyFrom.ReleaseDate;
            AcquiredOn = copyFrom.AcquiredOn;
            StartedOn = copyFrom.StartedOn;
            FinishedOn = copyFrom.FinishedOn;
            IsRemaster = copyFrom.IsRemaster;
            UseOriginalGameScore = copyFrom.UseOriginalGameScore;
            IsUnfinishable = copyFrom.IsUnfinishable;
            _platform = UniqueID.Copy(copyFrom._platform);
            _platformPlayedOn = UniqueID.Copy(copyFrom._platformPlayedOn);
            _originalGame = UniqueID.Copy(copyFrom._originalGame);
            _compilation = UniqueID.Copy(copyFrom._compilation);
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (CompletionCriteria.Length > MaxLengthCompletionCriteria)
                throw new ValidationException("Completion criteria cannot be longer than " + MaxLengthCompletionCriteria.ToString() + " characters", CompletionCriteria);
            if (CompletionComment.Length > MaxLengthCompletionComment)
                throw new ValidationException("Completion comment cannot be longer than " + MaxLengthCompletionComment.ToString() + " characters", CompletionComment);
            if (TimeSpent.Length > MaxLengthTimeSpent)
                throw new ValidationException("Time spent cannot be longer than " + MaxLengthTimeSpent.ToString() + " characters", TimeSpent);
            try
            {
                var temp = Score;
            }
            catch (CyclicalReferenceException e)
            {
                throw new ValidationException("A game referenced by the Original Game references this one, creating a cyclical loop when trying to calculate score", e.Message, e);
            }
        }

        private void OnModelObjectDeleted(object sender, ModelObjectDeleteArgs args)
        {
            if (args.ObjectType == typeof(GameCompilation))
            {
                if (_compilation.Equals(args.DeletedObject.UniqueID))
                {
                    Compilation = null;
                    SaveWithoutValidation(Module, Settings, args.Connection);
                }
            }
            else if (args.ObjectType == typeof(GameObject))
            {
                if (_originalGame.Equals(args.DeletedObject.UniqueID))
                {
                    OriginalGame = null;
                    SaveWithoutValidation(Module, Settings, args.Connection);
                }
            }
        }

        private void OnPlatformDeleted(object sender, PlatformDeleteArgs args)
        {
            bool changed = false;
            if (_platform.Equals(args.DeletedObject.UniqueID))
            {
                Platform = null;
                changed = true;
            }
            if (_platformPlayedOn.Equals(args.DeletedObject.UniqueID))
            {
                PlatformPlayedOn = null;
                changed = true;
            }
            if (changed)
                SaveWithoutValidation(Module, Settings, args.Connection);
        }

        protected override void PostSave(TrackerModule module, bool isNew, ILoadSaveMethod conn)
        {
            base.PostSave(module, isNew, conn);
            this.Module.DeleteEmptyCompilations(Settings, conn as ILoadSaveMethodGame);
        }

        protected override void PostDelete(TrackerModule module, ILoadSaveMethod conn)
        {
            base.PostDelete(module, conn);
            this.Module.DeleteEmptyCompilations(Settings, conn as ILoadSaveMethodGame);
        }

        protected override void AddEventHandlers()
        {
            base.AddEventHandlers();
            Module.ModelObjectDeleted += OnModelObjectDeleted;
            Module.PlatformDeleted += OnPlatformDeleted;
        }

        protected override void RemoveEventHandlers()
        {
            base.RemoveEventHandlers();
            Module.ModelObjectDeleted -= OnModelObjectDeleted;
            Module.PlatformDeleted -= OnPlatformDeleted;
        }

        private double CalculateScoreRecursive(IList<UniqueID> path)
        {
            if (path.Contains(UniqueID))
                throw new CyclicalReferenceException("Cyclical reference in OriginalGame field: " + string.Join(" -> ", path));
            if (IsUsingOriginalGameScore)
            {
                path.Add(UniqueID);
                return OriginalGame.CalculateScoreRecursive(path);
            }
            else
                return base.Score;
        }
    }
}
